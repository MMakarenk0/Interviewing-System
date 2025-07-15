namespace CandidateService.BLL.Validations;

public class InvalidOperationAppException : Exception
{
    public InvalidOperationAppException(string message) : base(message) { }
}