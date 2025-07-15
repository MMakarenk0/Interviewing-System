namespace CandidateService.BLL.Validations;

public class ValidationAppException : Exception
{
    public ValidationAppException(string message) : base(message) { }
}
