namespace OrbitSpace.WebApi.Identity;

public interface IApplicationUserProvider
{
    string UserId { get; }
    string UserEmail { get; }
}