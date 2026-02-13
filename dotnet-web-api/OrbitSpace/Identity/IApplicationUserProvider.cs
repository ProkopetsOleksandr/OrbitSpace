namespace OrbitSpace.WebApi.Identity;

public interface IApplicationUserProvider
{
    Guid UserId { get; }
    string UserEmail { get; }
}
