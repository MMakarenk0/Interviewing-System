using AutoMapper;
using CandidateService.BLL.Models;
using CandidateService.BLL.Services;
using CandidateService.BLL.Services.Interfaces;
using CandidateService.DAL.Repositories;
using CandidateService.DAL.UoF;
using DAL_Core.Entities;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace CandidateService.UnitTests.BLLTests;

public class CandidateProfileServiceTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    private readonly CandidateProfileService _service;


    public CandidateProfileServiceTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _blobStorageService = Substitute.For<IBlobStorageService>();
        _service = new CandidateProfileService(_unitOfWork, _mapper, _blobStorageService);
    }

    [Fact]
    public async Task GetCandidateProfileByIdAsync_ShouldReturnProfile_WhenExists()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var candidateProfile = new CandidateProfile
        {
            Id = candidateId,
            ProfileResumeBlobPath = "path/to/resume.pdf"
        };

        var candidateRepo = Substitute.For<ICandidateProfileRepository>();
        candidateRepo.GetAsync(candidateId).Returns(candidateProfile);

        _mapper.Map<CandidateProfileDto>(Arg.Any<CandidateProfile>())
            .Returns(ci => new CandidateProfileDto { Id = ci.Arg<CandidateProfile>().Id });

        _unitOfWork.CandidateProfileRepository.Returns(candidateRepo);

        _blobStorageService
            .GetResumeSasUriAsync("path/to/resume.pdf")
            .Returns("https://sas.url/resume.pdf");

        var service = new CandidateProfileService(_unitOfWork, _mapper, _blobStorageService);

        // Act
        var result = await service.GetCandidateProfileByIdAsync(candidateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(candidateId, result.Id);
        Assert.Equal("https://sas.url/resume.pdf", result.ProfileResumeUrl);
    }

    [Fact]
    public async Task GetCandidateProfileByIdAsync_ShouldThrow_WhenNotExists()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        _unitOfWork.CandidateProfileRepository.GetAsync(candidateId).Returns((CandidateProfile)null);
        var service = new CandidateProfileService(_unitOfWork, _mapper, _blobStorageService);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetCandidateProfileByIdAsync(candidateId));
    }

    [Fact]
    public async Task GetAllCandidateProfilesAsync_ShouldReturnDtosWithUrls()
    {
        // Arrange
        var profiles = new List<CandidateProfile>
        {
            new CandidateProfile { Id = Guid.NewGuid(), ProfileResumeBlobPath = "blob/path" }
        };

        var dtos = new List<CandidateProfileDto>
        {
            new CandidateProfileDto { Id = profiles[0].Id, ProfileResumeUrl = "blob/path" }
        };

        var candidateRepo = Substitute.For<ICandidateProfileRepository>();
        candidateRepo.GetAllAsync().Returns(profiles);
        _unitOfWork.CandidateProfileRepository.Returns(candidateRepo);

        _mapper.Map<IList<CandidateProfileDto>>(profiles).Returns(dtos);
        _blobStorageService.GetResumeSasUriAsync("blob/path").Returns("https://sas/resume.pdf");

        // Act
        var result = await _service.GetAllCandidateProfilesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("https://sas/resume.pdf", result[0].ProfileResumeUrl);
    }

    [Fact]
    public async Task CreateCandidateProfileAsync_ShouldCreateProfileAndReturnId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fileMock = Substitute.For<IFormFile>();
        fileMock.FileName.Returns("cv.pdf");
        fileMock.OpenReadStream().Returns(new MemoryStream());

        var dto = new CreateCandidateProfileDto
        {
            Id = id,
            ProfileResumeFile = fileMock
        };

        var repo = Substitute.For<ICandidateProfileRepository>();
        repo.GetAsync(id).Returns((CandidateProfile)null);
        _unitOfWork.CandidateProfileRepository.Returns(repo);

        var mapped = new CandidateProfile { Id = id };
        _mapper.Map<CandidateProfile>(dto).Returns(mapped);

        _blobStorageService
            .UploadOrReplaceResumeAsync(Arg.Any<Stream>(), "cv.pdf", $"candidates/{id}")
            .Returns("candidates/path/cv.pdf");

        // Act
        var result = await _service.CreateCandidateProfileAsync(dto);

        // Assert
        Assert.Equal(id, result);
        await repo.Received().CreateAsync(mapped);
        await _unitOfWork.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteCandidateProfileAsync_ShouldDeleteProfileAndFile()
    {
        // Arrange
        var id = Guid.NewGuid();
        var profile = new CandidateProfile { Id = id, ProfileResumeBlobPath = "path/blob" };

        var repo = Substitute.For<ICandidateProfileRepository>();
        repo.GetAsync(id).Returns(profile);
        _unitOfWork.CandidateProfileRepository.Returns(repo);

        // Act
        await _service.DeleteCandidateProfileAsync(id);

        // Assert
        await _blobStorageService.Received().DeleteFileAsync("path/blob");
        await repo.Received().DeleteAsync(id);
        await _unitOfWork.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateCandidateProfileAsync_ShouldUpdateProfileAndReplaceResume()
    {
        // Arrange
        var id = Guid.NewGuid();
        var profile = new CandidateProfile
        {
            Id = id,
            ProfileResumeBlobPath = "old/path",
            Applications = new List<Application>()
        };

        var file = Substitute.For<IFormFile>();
        file.FileName.Returns("new_cv.pdf");
        file.OpenReadStream().Returns(new MemoryStream());

        var dto = new CreateCandidateProfileDto
        {
            Id = id,
            ProfileResumeFile = file,
            ApplicationIds = new List<Guid>()
        };

        var repo = Substitute.For<ICandidateProfileRepository>();
        repo.GetCandidateProfileWithApplicationsAsync(id).Returns(profile);
        _unitOfWork.CandidateProfileRepository.Returns(repo);
        _unitOfWork.ApplicationRepository.Returns(Substitute.For<IApplicationRepository>());

        _mapper.When(m => m.Map(dto, profile)).Do(_ => { /* skip */ });

        _blobStorageService.UploadOrReplaceResumeAsync(
            Arg.Any<Stream>(), "new_cv.pdf", $"profiles/{id}.pdf"
        ).Returns("profiles/updated.pdf");

        // Act
        var result = await _service.UpdateCandidateProfileAsync(dto);

        // Assert
        Assert.Equal(id, result);
        Assert.Equal("profiles/updated.pdf", profile.ProfileResumeBlobPath);
        await _unitOfWork.Received().SaveChangesAsync();
    }
}
