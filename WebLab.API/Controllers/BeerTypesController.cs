using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLab.API.Data;
using WebLab.API.Services.BeerTypeService;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeerTypesController : ControllerBase
    {
		private readonly IBeerTypeService _beerTypeService;

		public BeerTypesController(IBeerTypeService beerTypeService)
        {
			_beerTypeService = beerTypeService;
		}

        // GET: api/BeerTypes
        [HttpGet]
        public async Task<ActionResult<ResponseData<BeerType[]>>> GetBeerTypes()
        {
            var beerTypes = await _beerTypeService.GetBeerTypeListAsync();
            return Ok(beerTypes);
        }
    }
}
