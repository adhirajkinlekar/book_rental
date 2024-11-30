using Azure.Core;
using book_rental_api.Data.book_rental_db;
using book_rental_api.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace book_rental_api.Services
{
    public interface IBookService
    {
        Task<Response<List<BooksListDTO>>> SearchBooks(string? title, string? genre);

        Task<Response<StatsDTO>> GetBooksStats(); 
    }

    public class BookService : IBookService
    {
        private readonly BookRentalDbContext _dbContext;

        private readonly EmailService _emailService;

        public BookService(BookRentalDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }


        public async Task<Response<List<BooksListDTO>>> SearchBooks(string? title, string? genre)
        {
            var query = _dbContext.Books.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(title)) query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));


            if (!string.IsNullOrEmpty(genre)) query = query.Where(b => b.Genre.Name.ToLower().Contains(genre.ToLower()));

            // In a real-world project, I would implement pagination and avoid fetching all books at once like this.
            List<BooksListDTO> booksList = await query
                .Select(x => new BooksListDTO
                {
                    Title = x.Title,
                    Author = x.Author.FullName,
                    Genre = x.Genre.Name,
                    ISBN = x.Isbn ?? string.Empty
                })
                .ToListAsync();

            return new Response<List<BooksListDTO>>
            {
                Data = booksList,
                Message = "Successfully retrieved books",
                Success = true
            }; ;
        }

        public async Task<Response<StatsDTO>> GetBooksStats()
        {

            StatsDTO StatsDTO = new();

            // Most overdue book: Book with the highest number of overdue rentals
            StatsDTO.MostOverdueBook = await _dbContext.RentalRecords.AsNoTracking()
                .Where(r => r.IsOverdue)
                .GroupBy(r => r.BookId)
                .OrderByDescending(g => g.Count())
                .Select(g => new BookBaseDTO
                {
                    BookId = g.Key,
                    BookTitle = g.Select(r => r.Book.Title).FirstOrDefault() ?? string.Empty,
                    AuthorId = g.Select(r => r.Book.Author.Id).FirstOrDefault(),
                    AuthorName = g.Select(r => r.Book.Author.FullName).FirstOrDefault() ?? string.Empty,
                    Count = g.Count(),
                })
                .FirstOrDefaultAsync();

            // Most popular book: Book with the highest number of total rentals
            StatsDTO.MostPopularBook = await _dbContext.RentalRecords.AsNoTracking()
                .GroupBy(r => r.BookId)
                .OrderByDescending(g => g.Count())
                .Select(g => new BookBaseDTO
                {
                    BookId = g.Key,
                    BookTitle = g.Select(r => r.Book.Title).FirstOrDefault() ?? string.Empty,
                    AuthorName = g.Select(r => r.Book.Author.FullName).FirstOrDefault() ?? string.Empty,
                    Count = g.Count(),
                })
                .FirstOrDefaultAsync();

            // Least popular book: Book with the least number of rentals
            StatsDTO.LeastPopularBook = await _dbContext.RentalRecords.AsNoTracking()
                .GroupBy(r => r.BookId)
                .OrderBy(g => g.Count())
                .Select(g => new BookBaseDTO
                {
                    BookId = g.Key,
                    BookTitle = g.Select(r => r.Book.Title).FirstOrDefault() ?? string.Empty,
                    AuthorName = g.Select(r => r.Book.Author.FullName).FirstOrDefault() ?? string.Empty,
                    Count = g.Count(),
                })
                .FirstOrDefaultAsync();

            return new Response<StatsDTO>
            {
                Data = StatsDTO,
                Message = "Successfully fetched books stats",
                Success = true
            }; ;
        }
    }
}
