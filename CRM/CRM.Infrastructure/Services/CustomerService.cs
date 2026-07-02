using CRM.Application.Interfaces.Services;
using CRM.Application.Features.Customers.CreateCustomer;
using CRM.Application.Features.Customers.UpdateCustomer;
using CRM.Application.Features.Customers.GetCustomers;
using CRM.Domain.Entities;
using CRM.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using CRM.Application.Interfaces.Generators;
using CRM.Domain.ValueObjects;
using CRM.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using CRM.Shared.Exceptions;

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



    public async Task UpdateAsync(
    Guid id,
    UpdateCustomerRequest request)
    {
        var customer = await GetCustomerOrThrowAsync(id);

        await EnsureEmailIsAvailableAsync(
            customer,
            request.Email);

        UpdateCustomer(customer, request);

        await _repository.SaveChangesAsync();

        _logger.LogInformation(
            "Customer {CustomerCode} updated successfully.",
            customer.CustomerCode);
    }


    private async Task EnsureEmailIsAvailableAsync(
        Customer customer,
        string email)
    {
        if (customer.Email.Equals(email,
            StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (await _repository.ExistsByEmailAsync(email))
        {
            throw new Exception(
                $"Customer with email '{email}' already exists.");
        }
    }

    private static void UpdateCustomer(
    Customer customer,
    UpdateCustomerRequest request)
    {
        customer.UpdateBasicInformation(
            request.Name,
            request.Email,
            request.Phone,
            request.Company,
            request.Website);

        customer.ChangeAddress(
            new Address(
                request.Address.Street,
                request.Address.City,
                request.Address.State,
                request.Address.Country,
                request.Address.PostalCode));

        customer.AssignTo(request.AssignedUserId);

        customer.ChangeStatus(request.Status);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await GetCustomerOrThrowAsync(id);

        customer.MarkAsDeleted();

        await _repository.SaveChangesAsync();

        _logger.LogInformation(
            "Customer {CustomerCode} deleted.",
            customer.CustomerCode);
    }


    public async Task<PagedResponse<CustomerListItem>> GetAllAsync(
    GetCustomersRequest request)
    {

    
        var query = _repository.GetQueryable();        

        query = ApplySearch(query, request);

        query = ApplyFilters(query, request);

        query = ApplySorting(query, request);

        var totalCount = await query.CountAsync();

        query = ApplyPaging(query, request);

        var customers = await ProjectCustomersAsync(query);

        return BuildPagedResponse(
            customers,
            request,
            totalCount);
        }


    private static IQueryable<Customer> ApplySearch(
        IQueryable<Customer> query,
        GetCustomersRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Search))
        {
            return query;
        }
        
        
        return query.Where(c =>
            c.Name.Contains(request.Search) ||
            c.Company.Contains(request.Search) ||
            c.Email.Contains(request.Search) ||
            c.CustomerCode.Contains(request.Search));
    }

    private static IQueryable<Customer> ApplyFilters(
        IQueryable<Customer> query,
        GetCustomersRequest request)
    {
        
        if (request.Status.HasValue)
        {
            query = query.Where(c =>
                c.Status == request.Status.Value);
        }

        if (request.Source.HasValue)
        {
            query = query.Where(c =>
                c.Source == request.Source.Value);
        }

        if (request.AssignedUserId.HasValue)
        {
            query = query.Where(c =>
                c.AssignedUserId == request.AssignedUserId.Value);
        }

        return query;
    }

    private static IQueryable<Customer> ApplySorting(
        IQueryable<Customer> query,
        GetCustomersRequest request)
    {
        
        return request.SortBy.ToLower() switch
        {
            "name" => request.Descending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name),

            "company" => request.Descending
                ? query.OrderByDescending(c => c.Company)
                : query.OrderBy(c => c.Company),

            _ => request.Descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt)
        };
    }

    private static IQueryable<Customer> ApplyPaging(
        IQueryable<Customer> query,
        GetCustomersRequest request)
    {
        
        return query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
    }

    private static Task<List<CustomerListItem>> ProjectCustomersAsync(
    IQueryable<Customer> query)
    {
        
        return query
            .Select(c => new CustomerListItem
            {
                Id = c.Id,
                CustomerCode = c.CustomerCode,
                Name = c.Name,
                Company = c.Company,
                Email = c.Email,
                Phone = c.Phone,
                Status = c.Status,
                Source = c.Source
            })
            .ToListAsync();
    }


    private static PagedResponse<CustomerListItem> BuildPagedResponse(
        IReadOnlyList<CustomerListItem> customers,
        GetCustomersRequest request,
        int totalCount)
    {
        return new PagedResponse<CustomerListItem>
        {
            Items = customers,

            Page = request.Page,

            PageSize = request.PageSize,

            TotalCount = totalCount,

            TotalPages = (int)Math.Ceiling(
                totalCount / (double)request.PageSize)
        };
    }
}