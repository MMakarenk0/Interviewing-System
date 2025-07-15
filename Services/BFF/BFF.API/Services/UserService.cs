using BFF.API.Clients;
using BFF.API.Models;
using BFF.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace BFF.API.Services;

public class UserService : IUserService
{
    private readonly IIdentityApi _identityApi;
    private readonly ICandidateServiceApi _candidateApi;

    public UserService(
        IIdentityApi identityApi,
        ICandidateServiceApi candidateApi)
    {
        _identityApi = identityApi;
        _candidateApi = candidateApi;
    }

    public async Task<PaginatedResult<AggregatedUserDto>> GetUsersWithCandidateProfiles(AggregatedUserQueryParameters query)
    {
        List<Guid> candidateUserIds = null;

        if (HasCandidateFilters(query) && (string.IsNullOrEmpty(query.Role) || query.Role == "Candidate"))
        {
            var candidateQuery = new CandidateProfileQuery
            {
                Position = query.Position,
                TechStack = query.TechStack
            };

            var (op, years) = ParseYearsFilter(query.YearsOfExperienceRaw);
            candidateQuery.YearsOfExperienceOperator = op;
            candidateQuery.YearsOfExperienceValue = years;

            var candidateProfiles = await _candidateApi.FilterCandidateProfiles(candidateQuery);
            candidateUserIds = candidateProfiles.Select(p => p.UserId).ToList();

            if (candidateUserIds.Count == 0)
            {
                return new PaginatedResult<AggregatedUserDto>
                {
                    TotalCount = 0,
                    Items = new List<AggregatedUserDto>()
                };
            }
        }

        var identityQuery = new UserQueryParameters
        {
            Email = query.Email,
            Role = query.Role,
            FirstName = query.FirstName,
            LastName = query.LastName,
            LockoutEnabled = query.LockoutEnabled,
            Page = query.Page,
            PageSize = query.PageSize,
            CandidateUserIds = candidateUserIds
        };

        var userPage = await _identityApi.GetUsersAsync(identityQuery);

        var orderedUsers = userPage.Items
            .OrderByDescending(u => u.Role == "Candidate" && candidateUserIds?.Contains(u.Id) == true)
            .ToList();

        var candidateIds = orderedUsers
            .Where(u => u.Role == "Candidate")
            .Select(u => u.Id)
            .ToList();

        var profiles = candidateIds.Any()
            ? await _candidateApi.GetCandidateProfilesByIdsAsync(new IdListRequest { Ids = candidateIds })
            : new List<CandidateProfileDto>();

        var profileDict = profiles.ToDictionary(p => p.UserId);

        var result = orderedUsers.Select(u => new AggregatedUserDto
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role,
            FirstName = u.FirstName,
            LastName = u.LastName,
            CandidateProfile = profileDict.GetValueOrDefault(u.Id)
        }).ToList();

        return new PaginatedResult<AggregatedUserDto>
        {
            TotalCount = userPage.TotalCount,
            Items = result
        };
    }


    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        return await _identityApi.GetUserByIdAsync(id);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto model)
    {
        return await _identityApi.CreateUserAsync(model);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _identityApi.DeleteUserAsync(id);
    }

    public async Task UpdateUserRoleAsync(UpdateUserRoleDto model)
    {
        await _identityApi.UpdateUserRoleAsync(model);
    }

    private bool HasCandidateFilters(AggregatedUserQueryParameters query)
    {
        return !string.IsNullOrEmpty(query.Position) ||
               !string.IsNullOrEmpty(query.YearsOfExperienceRaw) ||
               !string.IsNullOrEmpty(query.TechStack);
    }

    private (string? op, int? value) ParseYearsFilter(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return (null, null);

        var match = Regex.Match(raw.Trim(), @"^(<=|>=|=|<|>)?\s*(\d+)$");
        if (!match.Success) return (null, null);

        var op = match.Groups[1].Value;
        var val = int.TryParse(match.Groups[2].Value, out var intVal) ? intVal : (int?)null;

        return (op, val);
    }

}
