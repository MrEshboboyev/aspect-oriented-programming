namespace AspectOrientedProgramming.API.Attributes;

/// <summary>
/// Attribute to mark methods that should be cached
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute : Attribute
{
    /// <summary>
    /// Duration in minutes for which the result should be cached
    /// </summary>
    public int DurationInMinutes { get; set; } = 5;

    public CacheAttribute()
    {
    }

    public CacheAttribute(int durationInMinutes)
    {
        DurationInMinutes = durationInMinutes;
    }
}
