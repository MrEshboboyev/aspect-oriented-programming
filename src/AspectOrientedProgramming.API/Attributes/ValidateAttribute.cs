namespace AspectOrientedProgramming.API.Attributes;

/// <summary>
/// Attribute to mark methods whose arguments should be validated
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ValidateAttribute : Attribute
{
    public ValidateAttribute()
    {
    }
}
