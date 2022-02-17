using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Business.DTOs.OfferDTOs;

namespace WebAPI.Controllers
{
    [Route("offers")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet("product/{productId}")]
        public IActionResult GetAllByProductId(int productId)
        {
            var result = _offerService.GetAllByProductId(productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("user/current")]
        public IActionResult GetAllByUserId()
        {
            var result = _offerService.GetAllByUserId();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _offerService.GetById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult SendOffer(int productId, [FromBody] SendOfferDto createOfferDto)
        {
            var result = _offerService.SendOffer(productId, createOfferDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{offerId}/reoffer")]
        public IActionResult ChangeOffer(int offerId, [FromBody]SendOfferDto updateOfferDto)
        {
            var result = _offerService.ChangeOffer(offerId, updateOfferDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{offerId}/confirm")]
        public IActionResult ConfirmOffer(int offerId, bool confirmStatus)
        {
            var result = _offerService.ConfirmOffer(offerId, confirmStatus);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{offerId}/back")]
        public IActionResult GetBackOffer(int offerId)
        {
            var result = _offerService.GetBackOffer(offerId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
