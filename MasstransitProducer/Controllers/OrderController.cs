using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShareModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitProducer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<OrderController> _logger;
        private readonly IBus _bus;

        public OrderController(ILogger<OrderController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await _bus.Publish(new Order
            {
                Id = 1,
                Name = $"#GetOrder-{Guid.NewGuid()}"
            });
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> CreateOrder(int id)
        {
            await _bus.Publish(new OrderCreate
            {
                OrderName = $"create-order-{id}"
            });
            return Ok();
        }

        [HttpPost("tiki")]
        public async Task<IActionResult> CreateTikiOrder()
        {
            await _bus.Publish(new OrderTiki
            {
                OrderName = $"create-order-{Guid.NewGuid()}",

            });
            return Ok();
        }


    }
}
