namespace AspectOrientedProgramming.API.Attributes;

/// <summary>
/// Attribute to mark methods that require authorization
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Roles required to access the method
    /// </summary>
    public string? Roles { get; set; }

    /// <summary>
    /// Policies required to access the method
    /// </summary>
    public string? Policy { get; set; }

    public AuthorizeAttribute()
    {
    }

    public AuthorizeAttribute(string roles)
    {
        Roles = roles;
    }
}
