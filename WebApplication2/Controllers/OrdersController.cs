
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        // Managers or Admins can list all orders
        [HttpGet]
        [Authorize(Policy = "RequireManagerOrAdmin")]
        public IActionResult GetAllOrders()
        {
            // Example response — replace with your actual database call
            var orders = new[] { "Order-001", "Order-002", "Order-003" };
            ret
