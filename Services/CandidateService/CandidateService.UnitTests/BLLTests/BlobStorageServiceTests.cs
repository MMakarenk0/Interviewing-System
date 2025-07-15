using CandidateService.BLL.Services;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CandidateService.UnitTests.BLLTests;

public class BlobStorageServiceTests
{
    [Fact]
    public async Task UploadOrReplaceResumeAsync_ShouldThrow_WhenFileNotPdf()
    {
        // Arrange
        var configuration = Substitute.For<IConfiguration>();
        configuration.GetConnectionString("AzureBlobStorage").Returns("UseDevelopmentStorage=true");
        configuration["AzureBlobStorage:ContainerName"].Returns("resumes");

        var service = new BlobStorageService(configuration);

        var stream = new MemoryStream();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UploadOrReplaceResumeAsync(stream, "resume.docx", "profiles/123"));

        Assert.Equal("Only PDF resumes are allowed.", ex.Message);
    }

}

