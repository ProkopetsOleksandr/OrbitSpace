namespace OrbitSpace.WebApi.Identity;

public class ApplicationUser(IApplicationUserProvider provider)
{
    private Guid? _id;
    public Guid Id => _id ??= provider.UserId;

    private string? _email;
    public string Email => _email ??= provider.UserEmail;
}
