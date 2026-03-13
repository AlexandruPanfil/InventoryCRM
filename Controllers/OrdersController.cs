using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryCRM.Dtos.Orders;
using InventoryCRM.Models;
using InventoryCRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersAsync(
            [FromQuery] Guid? customerId,
            [FromQuery] string? customerName)
        {
            List<Order> orders;
            if (customerId.HasValue)
            {
                orders = await _orderService.GetOrdersByCustomerIdAsync(customerId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(customerName))
            {
                orders = await _orderService.FindOrdersByCustomerNameAsync(customerName.Trim());
            }
            else
            {
                orders = await _orderService.GetAllOrdersAsync();
            }

            return Ok(orders.Select(MapOrder));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderResponse>> GetOrderAsync(Guid id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(MapOrder(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrderAsync(OrderCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                ModelState.AddModelError(nameof(request.Description), "Description cannot be empty.");
                return ValidationProblem(ModelState);
            }

            var now = DateTime.UtcNow;
            var order = new Order
            {
                CustomersId = request.CustomerId,
                Description = request.Description.Trim(),
                WorkerId = request.WorkerId,
                CreatedAt = now,
                UpdatedAt = now
            };

            try
            {
                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    order.SetStatus(request.Status.Trim());
                }

                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, MapOrder(createdOrder));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderAsync(Guid id, OrderUpdateRequest request)
        {
            try
            {
                var description = request.Description?.Trim();
                var status = request.Status?.Trim();
                var updatedOrder = await _orderService.UpdateOrderAsync(id, description, status, request.WorkerId);
                if (updatedOrder == null)
                {
                    return NotFound();
                }

                return Ok(MapOrder(updatedOrder));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrderAsync(Guid id)
        {
            var existing = await _orderService.GetOrderAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }

        private static OrderResponse MapOrder(Order order)
        {
            var unitAssignments = order.UnitAssignment?
                .Select(u => new UnitAssignmentSummary(u.Id, u.Name, u.Quantity, u.Status))
                .ToList() ?? new List<UnitAssignmentSummary>();

            return new OrderResponse(
                order.Id,
                order.OrderNumber,
                order.Identifier,
                order.Description,
                order.Status,
                order.CreatedAt,
                order.UpdatedAt,
                order.CustomersId,
                order.Customers?.Name ?? string.Empty,
                order.WorkerId,
                order.Worker?.Workername,
                unitAssignments);
        }
    }
}
