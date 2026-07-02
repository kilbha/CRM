using CRM.Application.Features.Customers.CreateCustomer;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.Common.Authorization;
using CRM.Application.Features.Customers.UpdateCustomer;
using CRM.Application.Common.Models;
using CRM.Application.Features.Customers.GetCustomers;
using CRM.Application.Features.Customers.ActivateCustomer;
using CRM.Application.Features.Customers.DeactivateCustomer;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpPost]    
    // [AllowAnonymous]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    [ProducesResponseType(typeof(CreateCustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request)
    {
        
        var response = await _customerService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCustomerRequest request)
    {
        await _customerService.UpdateAsync(id, request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _customerService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    [ProducesResponseType(
        typeof(PagedResponse<CustomerListItem>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetCustomersRequest request)
    {
        var response =
            await _customerService.GetAllAsync(request);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    [ProducesResponseType(
        typeof(ActivateCustomerResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id)
    {
        var response = await _customerService.ActivateAsync(
            new ActivateCustomerRequest
            {
                CustomerId = id
            });

        return Ok(response);
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = AuthorizationPolicies.RequireSales)]
    [ProducesResponseType(
        typeof(DeactivateCustomerResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var response = await _customerService.DeactivateAsync(
            new DeactivateCustomerRequest
            {
                CustomerId = id
            });

        return Ok(response);
    }

}