namespace AspectOrientedProgramming.API.Attributes;

/// <summary>
/// Attribute to mark methods that should be monitored for performance
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PerformanceAttribute : Attribute
{
    /// <summary>
    /// Threshold in milliseconds above which a warning should be logged
    /// </summary>
    public int WarningThresholdMs { get; set; } = 1000;

    public PerformanceAttribute()
    {
    }

    public PerformanceAttribute(int warningThresholdMs)
    {
        WarningThresholdMs = warningThresholdMs;
    }
}
