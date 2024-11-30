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
    public class BooksController : APIControllerBase
    {

        private readonly IBookService _booksService;

        public BooksController(IBookService booksService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {

            _booksService = booksService;
        }

        [HttpGet()]
        [AllowAnonymous]
        public async Task<IActionResult> SearchBooks(BooksQueryDTO booksQueryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            return Ok(await _booksService.SearchBooks(booksQueryDTO.Title, booksQueryDTO.Genre));
        }

        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooksStats()
        {

            return Ok(await _booksService.GetBooksStats());
        }
    }
}


