namespace OrbitSpace.WebApi.Identity;

public class ApplicationUser
{
    private readonly IApplicationUserProvider _provider;

    public ApplicationUser(IApplicationUserProvider provider)
    {
        _provider = provider;
    }
    
    private string? _id { get; set; }
    public string Id => _id ??= _provider.UserId;
    
    private string? _email { get; set; }
    public string Email => _email ??= _provider.UserEmail;
}