namespace OrbitSpace.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();   
}