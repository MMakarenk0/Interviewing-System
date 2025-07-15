namespace CandidateService.BLL.Services.Interfaces;

public interface IBlobStorageService
{
    Task DeleteFileAsync(string blobFileName);
    Task<string> GetResumeSasUriAsync(string blobFileName);
    Task<string> UploadOrReplaceResumeAsync(Stream fileStream, string originalFileName, string path);
}