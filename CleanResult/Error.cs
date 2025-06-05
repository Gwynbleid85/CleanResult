namespace CleanResult;

/// <summary>
/// Structure representing an error.
/// </summary>
public struct Error
{
    /// <summary>
    /// Error message describing the error.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Error code representing the type of error.
    /// </summary>
    public int Code { get; set; }
}