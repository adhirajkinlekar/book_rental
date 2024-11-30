using book_rental_api.Models;
using book_rental_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace book_rental_api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RentalsController : APIControllerBase
    {

        private readonly IRentalsService _rentalsService;

        public RentalsController(IRentalsService rentalsService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {

            _rentalsService = rentalsService;
        }

        [HttpPost("borrow/{bookId}")]
        public async Task<IActionResult> RentBook([FromRoute] int bookId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _rentalsService.RentBook(bookId, UserId!.Value));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetRentalHistory()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _rentalsService.GetRentalHistoryByUser(UserId!.Value));
        }

        [HttpPost("returns/{rentalId}")]
        public async Task<IActionResult> ReturnBook([FromRoute] int rentalId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _rentalsService.ReturnBook(rentalId, UserId!.Value));
        }

    }
}


