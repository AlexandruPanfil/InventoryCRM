using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryCRM.Dtos.Customers;
using InventoryCRM.Models;
using InventoryCRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetCustomersAsync([FromQuery] string? name)
        {
            var customers = string.IsNullOrWhiteSpace(name)
                ? await _customerService.GetAllCustomersAsync()
                : await _customerService.FindCustomerByNameAsync(name.Trim());

            return Ok(customers.Select(MapCustomer));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(MapCustomer(customer));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CustomerCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var customer = new Customer
            {
                IDNO = request.IDNO.Trim(),
                IBAN = request.IBAN.Trim(),
                CodTVA = request.CodTVA,
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Email = request.Email.Trim(),
                Description = request.Description.Trim()
            };

            var created = await _customerService.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomerAsync), new { id = created.Id }, MapCustomer(created));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomerAsync(Guid id, CustomerUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updatedCustomer = new Customer
            {
                IDNO = request.IDNO.Trim(),
                IBAN = request.IBAN.Trim(),
                CodTVA = request.CodTVA,
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Email = request.Email.Trim(),
                Description = request.Description.Trim()
            };

            var result = await _customerService.UpdateCustomerAsync(id, updatedCustomer);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(MapCustomer(result));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static CustomerResponse MapCustomer(Customer customer)
        {
            return new CustomerResponse(
                customer.Id,
                customer.IDNO,
                customer.IBAN,
                customer.CodTVA,
                customer.Name,
                customer.Address,
                customer.PhoneNumber,
                customer.Email,
                customer.Description);
        }
    }
}
