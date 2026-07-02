using CRM.Domain.Entities;

namespace CRM.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);

    Task<Customer?> GetByIdAsync(Guid id);

    Task<Customer?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);

    Task SaveChangesAsync();

    IQueryable<Customer> GetQueryable();

    void UpdateAsync(Customer customer);
}