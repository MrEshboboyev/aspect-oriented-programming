namespace AspectOrientedProgramming.API.Attributes;

/// <summary>
/// Attribute to mark methods that should be logged
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class LogAttribute : Attribute
{
    /// <summary>
    /// Log level for the method execution
    /// </summary>
    public string LogLevel { get; set; } = "Information";

    public LogAttribute()
    {
    }

    public LogAttribute(string logLevel)
    {
        LogLevel = logLevel;
    }
}
