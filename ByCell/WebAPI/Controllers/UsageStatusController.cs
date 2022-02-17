using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("usagestatuses")]
    [ApiController]
    public class UsageStatusController : ControllerBase
    {
        private readonly IUsageStatusService _usageStatusService;

        public UsageStatusController(IUsageStatusService usageStatusService)
        {
            _usageStatusService = usageStatusService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _usageStatusService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _usageStatusService.GetById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] string name)
        {
            var result = _usageStatusService.Create(name);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] string name)
        {
            var result = _usageStatusService.Edit(id, name);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _usageStatusService.Delete(id);
            if (result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
