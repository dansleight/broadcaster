using System;

namespace Broadcaster.SpaModels;

public class HttpValidationError
{
    #region Properties

    public List<ValidationError>? Detail { get; set; }

    #endregion

    #region Constructors

    public HttpValidationError() { }

    public HttpValidationError(string field, string message)
    {
        Detail = new();
        Detail.Add(new ValidationError(field, message));
    }

    #endregion
}

public class ValidationError
{
    #region Properties

    public string Field { get; set; } = null!;
    public string Message { get; set; } = null!;

    #endregion

    #region Constructors

    public ValidationError() { }

    public ValidationError(string field, string message)
    {
        Field = field;
        Message = message;
    }

    #endregion
}