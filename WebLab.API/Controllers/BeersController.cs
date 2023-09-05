using Microsoft.AspNetCore.Mvc;
using WebLab.API.Services.BeerService;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BeersController : ControllerBase
	{
		private readonly IBeerService _beerService;

		public BeersController(IBeerService beerService)
		{
			_beerService = beerService;
		}

		// GET: api/Beers
		[HttpGet("page{pageNo}")]
		[HttpGet]
		public async Task<ActionResult<ResponseData<List<Beer>>>> GetBeers(string? beerType, int pageNo = 1, int pageSize = 3)
		{
			var beerList = await _beerService.GetBeerListAsync(beerType, pageNo, pageSize);
			return Ok(beerList);
		}

		// GET: api/Beers/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ResponseData<Beer>>> GetBeer(int id)
		{
			var beerResponse = await _beerService.GetBeerByIdAsync(id);
			return Ok(beerResponse);
		}

		// PUT: api/Beers/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutBeer(int id, Beer beer)
		{
			await _beerService.UpdateBeerAsync(id, beer);
			return NoContent();
		}

		// POST: api/Beers
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<ResponseData<Beer>>> PostBeer(Beer beer)
		{
			var createdBeer = await _beerService.CreateBeerAsync(beer);
			return CreatedAtAction("GetBeer", new { id = createdBeer.Data.Id }, createdBeer);
		}

		// DELETE: api/Beers/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBeer(int id)
		{
			await _beerService.DeleteBeerAsync(id);
			return NoContent();
		}

		// POST: api/Dishes/5
		[HttpPost("{id}")]
		public async Task<ActionResult<ResponseData<string>>> PostImage(int id, IFormFile formFile)
		{
			var response = await _beerService.SaveImageAsync(id, formFile);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			return NotFound(response);
		}
	}
}
