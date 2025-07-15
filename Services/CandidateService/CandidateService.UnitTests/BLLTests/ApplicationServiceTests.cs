using AutoMapper;
using CandidateService.BLL.Clients;
using CandidateService.BLL.Models;
using CandidateService.BLL.Services;
using CandidateService.BLL.Services.Interfaces;
using CandidateService.DAL.Repositories;
using CandidateService.DAL.Repositories.Interfaces;
using CandidateService.DAL.UoF;
using DAL_Core.Entities;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace CandidateService.UnitTests.BLLTests;

public class ApplicationServiceTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IPositionClient _positionClient;
    private readonly ApplicationService _service;

    public ApplicationServiceTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _blobStorageService = Substitute.For<IBlobStorageService>();
        _positionClient = Substitute.For<IPositionClient>();

        _service = new ApplicationService(_unitOfWork, _mapper, _blobStorageService, _positionClient, null);
    }

    [Fact]
    public async Task GetApplicationByIdAsync_ShouldReturnApplicationDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var app = new Application
        {
            Id = id,
            ResumeBlobPath = "resume/path.pdf",
            PositionId = Guid.NewGuid()
        };

        var cached = new CachedPosition { Id = app.PositionId, Title = "QA", UpdatedAt = DateTime.UtcNow, IsActive = true };
        var dto = new ApplicationDto { Id = id, PositionId = app.PositionId, ResumeUrl = "resume/path.pdf" };

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetWithCandidateProfileAsync(id).Returns(app);
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        var cachedRepo = Substitute.For<ICachedPositionRepository>();
        cachedRepo.GetAsync(app.PositionId).Returns(cached);
        _unitOfWork.CachedPositionRepository.Returns(cachedRepo);

        _mapper.Map<ApplicationDto>(app).Returns(dto);
        _blobStorageService.GetResumeSasUriAsync("resume/path.pdf").Returns("https://sas/path.pdf");

        // Act
        var result = await _service.GetApplicationByIdAsync(id);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal("https://sas/path.pdf", result.ResumeUrl);
        Assert.Equal("QA", result.PositionTitle);
    }

    [Fact]
    public async Task GetAllApplicationsAsync_ShouldReturnApplicationDtos()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var app = new Application { Id = Guid.NewGuid(), ResumeBlobPath = "path", PositionId = positionId };
        var dto = new ApplicationDto { Id = app.Id, ResumeUrl = "path", PositionId = positionId };

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetAllWithCandidateProfileAsync().Returns(new List<Application> { app });
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        var cachedRepo = Substitute.For<ICachedPositionRepository>();
        cachedRepo.GetAsync(positionId).Returns(new CachedPosition { Id = positionId, Title = "Developer", UpdatedAt = DateTime.UtcNow, IsActive = true });
        _unitOfWork.CachedPositionRepository.Returns(cachedRepo);

        _mapper.Map<IList<ApplicationDto>>(Arg.Any<IList<Application>>()).Returns(new List<ApplicationDto> { dto });
        _blobStorageService.GetResumeSasUriAsync("path").Returns("https://sas/path");

        // Act
        var result = await _service.GetAllApplicationsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("https://sas/path", result[0].ResumeUrl);
        Assert.Equal("Developer", result[0].PositionTitle);
    }

    [Fact]
    public async Task CreateApplicationAsync_ShouldCreateWithUploadedResume()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new CreateApplicationDto
        {
            Id = id,
            PositionId = Guid.NewGuid(),
            ResumeFile = Substitute.For<IFormFile>(),
            CandidateProfileId = Guid.NewGuid()
        };

        model.ResumeFile.FileName.Returns("cv.pdf");
        model.ResumeFile.OpenReadStream().Returns(new MemoryStream());

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetAsync(id).Returns((Application)null);
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        var position = new CachedPosition { Id = model.PositionId, UpdatedAt = DateTime.UtcNow, IsActive = true };
        var cachedRepo = Substitute.For<ICachedPositionRepository>();
        cachedRepo.GetAsync(model.PositionId).Returns(position);
        _unitOfWork.CachedPositionRepository.Returns(cachedRepo);

        _mapper.Map<Application>(model).Returns(new Application { Id = id });

        _blobStorageService
            .UploadOrReplaceResumeAsync(Arg.Any<Stream>(), "cv.pdf", $"applications/{id}.pdf")
            .Returns("applications/path/cv.pdf");

        var positionClientResult = new PositionDto { Id = model.PositionId, IsActive = true };
        _positionClient.GetPositionByIdAsync(model.PositionId).Returns(positionClientResult);

        var profileRepo = Substitute.For<ICandidateProfileRepository>();
        _unitOfWork.CandidateProfileRepository.Returns(profileRepo);

        // Act
        var result = await _service.CreateApplicationAsync(model);

        // Assert
        Assert.Equal(id, result);
        await appRepo.Received().CreateAsync(Arg.Is<Application>(a => a.ResumeBlobPath == "applications/path/cv.pdf"));
        await _unitOfWork.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateApplicationAsync_ShouldUpdateAndReplaceResume()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new CreateApplicationDto
        {
            Id = id,
            PositionId = Guid.NewGuid(),
            ResumeFile = Substitute.For<IFormFile>()
        };
        model.ResumeFile.FileName.Returns("cv.pdf");
        model.ResumeFile.OpenReadStream().Returns(new MemoryStream());

        var application = new Application
        {
            Id = id,
            PositionId = model.PositionId,
            ResumeBlobPath = "old/path",
            CandidateProfileId = Guid.NewGuid()
        };

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetAsync(id).Returns(application);
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        var cachedRepo = Substitute.For<ICachedPositionRepository>();
        cachedRepo.GetAsync(model.PositionId).Returns(new CachedPosition { Id = model.PositionId, UpdatedAt = DateTime.UtcNow, IsActive = true });
        _unitOfWork.CachedPositionRepository.Returns(cachedRepo);

        _positionClient.GetPositionByIdAsync(model.PositionId).Returns(new PositionDto { Id = model.PositionId, IsActive = true });
        _mapper.When(m => m.Map(model, application)).Do(_ => { });

        _blobStorageService
            .UploadOrReplaceResumeAsync(Arg.Any<Stream>(), "cv.pdf", $"applications/{id}.pdf")
            .Returns("applications/newpath.pdf");

        var profileRepo = Substitute.For<ICandidateProfileRepository>();
        _unitOfWork.CandidateProfileRepository.Returns(profileRepo);

        // Act
        var result = await _service.UpdateApplicationAsync(model);

        // Assert
        Assert.Equal(id, result);
        Assert.Equal("applications/newpath.pdf", application.ResumeBlobPath);
        await _unitOfWork.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteApplicationAsync_ShouldDeleteWithResume()
    {
        // Arrange
        var id = Guid.NewGuid();
        var application = new Application
        {
            Id = id,
            ResumeBlobPath = "applications/resume.pdf",
            CandidateProfileId = Guid.NewGuid()
        };

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetAsync(id).Returns(application);
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        // Act
        await _service.DeleteApplicationAsync(id);

        // Assert
        await _blobStorageService.Received().DeleteFileAsync("applications/resume.pdf");
        await appRepo.Received().DeleteAsync(id);
        await _unitOfWork.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task IsApplicationOwnedByUserAsync_ShouldReturnTrueIfOwned()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();

        var application = new Application
        {
            Id = appId,
            CandidateProfileId = profileId
        };

        var profile = new CandidateProfile
        {
            Id = profileId,
            UserId = userId
        };

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetWithCandidateProfileAsync(appId).Returns(new Application
        {
            Id = appId,
            CandidateProfileId = profileId,
            CandidateProfile = new CandidateProfile
            {
                Id = profileId,
                UserId = userId
            }
        });
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        var profileRepo = Substitute.For<ICandidateProfileRepository>();
        profileRepo.GetAsync(profileId).Returns(profile);
        _unitOfWork.CandidateProfileRepository.Returns(profileRepo);

        // Act
        var result = await _service.IsApplicationOwnedByUserAsync(appId, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsApplicationOwnedByUserAsync_ShouldReturnFalseIfNotOwned()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var profileId = Guid.NewGuid();

        var application = new Application
        {
            Id = appId,
            CandidateProfileId = profileId
        };

        var profile = new CandidateProfile
        {
            Id = profileId,
            UserId = anotherUserId // not matching
        };

        var appRepo = Substitute.For<IApplicationRepository>();
        appRepo.GetAsync(appId).Returns(application);
        _unitOfWork.ApplicationRepository.Returns(appRepo);

        var profileRepo = Substitute.For<ICandidateProfileRepository>();
        profileRepo.GetAsync(profileId).Returns(profile);
        _unitOfWork.CandidateProfileRepository.Returns(profileRepo);

        // Act
        var result = await _service.IsApplicationOwnedByUserAsync(appId, userId);

        // Assert
        Assert.False(result);
    }
}
