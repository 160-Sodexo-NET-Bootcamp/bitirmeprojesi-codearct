using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("offerconfirms")]
    [ApiController]
    public class OfferConfirmController : ControllerBase
    {
        private readonly IOfferConfirmService _offerConfirmService;

        public OfferConfirmController(IOfferConfirmService offerConfirmService)
        {
            _offerConfirmService = offerConfirmService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _offerConfirmService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _offerConfirmService.GetById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] string name)
        {
            var result = _offerConfirmService.Create(name);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] string name)
        {
            var result = _offerConfirmService.Edit(id, name);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _offerConfirmService.Delete(id);
            if (result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
