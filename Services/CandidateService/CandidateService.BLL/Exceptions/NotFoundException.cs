namespace CandidateService.BLL.Validations;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, Guid key)
        : base($"Entity '{entityName}' with key '{key}' was not found.") { }
}
