using CRM.Application.Interfaces.Services;
using CRM.Application.Features.Customers.CreateCustomer;
using CRM.Domain.Entities;
using CRM.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using CRM.Application.Interfaces.Generators;
using CRM.Domain.ValueObjects;
namespace CRM.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    private readonly ICustomerCodeGenerator _customerCodeGenerator;

    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository repository,
        ICustomerCodeGenerator customerCodeGenerator,
        ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
        _customerCodeGenerator = customerCodeGenerator;
    }

    public async Task<CreateCustomerResponse> CreateAsync(
    CreateCustomerRequest request)
    {
        await EnsureEmailIsUniqueAsync(request.Email);

        var customerCode =
            await _customerCodeGenerator.GenerateAsync();

        var customer = CreateCustomer(request, customerCode);

        await SaveCustomerAsync(customer);

        return CreateResponse(customer);
    }

    private static Customer CreateCustomer(
        CreateCustomerRequest request,
        string customerCode)
    {
        var address = new Address(
            request.Address.Street,
            request.Address.City,
            request.Address.State,
            request.Address.Country,
            request.Address.PostalCode);

        return Customer.Create(
            customerCode,
            request.Name,
            request.Email,
            request.Phone,
            request.Company,
            request.Website,
            request.Source,
            request.AssignedUserId,
            address);
    }

    private async Task SaveCustomerAsync(Customer customer)
    {
        await _repository.AddAsync(customer);

        await _repository.SaveChangesAsync();

        _logger.LogInformation(
            "Customer {CustomerCode} created successfully.",
            customer.CustomerCode);
    }

    private static CreateCustomerResponse CreateResponse(
    Customer customer)
{
    return new CreateCustomerResponse
    {
        Id = customer.Id,
        CustomerCode = customer.CustomerCode,
        Name = customer.Name
    };
}

    private async Task EnsureEmailIsUniqueAsync(string email)
    {
        var existingCustomer = await _repository.GetByEmailAsync(email);

        if (existingCustomer != null)
        {
            throw new InvalidOperationException(
                $"A customer with email '{email}' already exists.");
        }
    }


    public async Task<GetCustomerResponse> GetByIdAsync(Guid id)
    {
        var customer = await GetCustomerOrThrowAsync(id);

        return GetResponse(customer);
    }

    private async Task<Customer> GetCustomerOrThrowAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer is null)
        {
            throw new Exception("Customer not found.");
        }

        return customer;
    }

    private static GetCustomerResponse GetResponse(Customer customer)
    {
        return new GetCustomerResponse
        {
            Id = customer.Id,
            CustomerCode = customer.CustomerCode,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Company = customer.Company,
            Website = customer.Website,
            Status = customer.Status,
            Source = customer.Source,
            AssignedUserId = customer.AssignedUserId,
            Address = new AddressDto
            {
                Street = customer.Address.Street,
                City = customer.Address.City,
                State = customer.Address.State,
                Country = customer.Address.Country,
                PostalCode = customer.Address.PostalCode
            }
        };
    }

}