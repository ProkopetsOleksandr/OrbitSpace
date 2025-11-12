namespace OrbitSpace.WebApi.Identity;

public class ApplicationUser(IApplicationUserProvider provider)
{
    private string? _id { get; set; }
    public string Id => _id ??= provider.UserId;
    
    private string? _email { get; set; }
    public string Email => _email ??= provider.UserEmail;
}