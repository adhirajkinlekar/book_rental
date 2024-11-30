using Azure.Core;
using book_rental_api.Data.book_rental_db;
using book_rental_api.Exceptions;
using book_rental_api.Models;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace book_rental_api.Services
{
    public interface IRentalsService
    {
        Task<Response<Unit>> RentBook(int bookId, Guid userId);

        Task<Response<Unit>> ReturnBook(int rentalId, Guid userId);

        Task<Response<List<RentalHistoryDTO>>> GetRentalHistoryByUser(Guid userId);

        Task ProcessOverdueRentals();
    }

    public class RentalsService : IRentalsService
    {
        private readonly BookRentalDbContext _dbContext;

        private readonly EmailService _emailService;

        public RentalsService(BookRentalDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task<Response<Unit>> RentBook(int bookId, Guid userId)
        {

            Book? book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) throw new APIException("Book not found", 404);

            // Find the record for the last rented book and check if it has been returned (Considering all books to have a single copy based on the the provided sample data.)
            RentalRecord? existingRental = await _dbContext.RentalRecords.Where(r => r.BookId == bookId && r.ReturnedOn.HasValue)
                                                               .OrderByDescending(r => r.RentedOn)
                                                               .FirstOrDefaultAsync();

            if (existingRental != null) throw new APIException("A user has already rented this book.", 400);

            var rentalHistory = new RentalRecord
            {
                BookId = bookId,
                UserId = userId,
                RentedOn = DateTime.Now,
                IsOverdue = false
            };

            _dbContext.RentalRecords.Add(rentalHistory);

            await _dbContext.SaveChangesAsync();

            return new Response<Unit>
            {
                Data = new Unit(),
                Message = "Successfully rented the book",
                Success = true
            };
        }

        public async Task<Response<Unit>> ReturnBook(int rentalId, Guid userId)
        {
            // Find the rental history record by ID and validate it belongs to the current user
            RentalRecord? rentalRecord = await _dbContext.RentalRecords.FirstOrDefaultAsync(x => x.Id == rentalId && x.UserId == userId);

            if (rentalRecord == null) throw new APIException("Rental record not found or unauthorized access.", 404);

            if (rentalRecord.ReturnedOn.HasValue) throw new APIException("This book has already been returned.", 400);

            // Mark the book as returned
            rentalRecord.ReturnedOn = DateTime.Now;

            // Optionally, we can mark the rental as overdue (as a secondary check in case the job failed to mark it), and even apply a penalty.
            // if (rentalRecord.ReturnedOn > rentalRecord.RentedOn.AddDays(14)) rentalRecord.IsOverdue = true;  

            await _dbContext.SaveChangesAsync();

            return new Response<Unit>
            {
                Data = new Unit(),
                Message = "Successfully returned the book",
                Success = true
            };
        }

        public async Task<Response<List<RentalHistoryDTO>>> GetRentalHistoryByUser(Guid userId)
        {
            List<RentalHistoryDTO> rentalHistory = await _dbContext.RentalRecords
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RentedOn)
                .Include(r => r.Book)
                .Select(r => new RentalHistoryDTO
                {
                    RentalId = r.Id,
                    BookTitle = r.Book.Title,
                    BookIsbn = r.Book.Isbn,
                    RentedOn = r.RentedOn,
                    ReturnedOn = r.ReturnedOn,
                    IsOverdue = r.IsOverdue,
                })
                .ToListAsync();

            return new Response<List<RentalHistoryDTO>>
            {
                Data = rentalHistory,
                Message = "Successfully retrived rental history",
                Success = true
            }; ;
        }

        // The job will run this method periodically to mark rental as overdue if required
        public async Task ProcessOverdueRentals()
        {
            var currentDate = DateTime.UtcNow;

            // Find rentals that are overdue (14 days since rented and not returned)
            var overdueRentals = await _dbContext.RentalRecords.Include(r => r.Book).Include(r => r.User).Include(x => x.Book.Author)
                .Where(r => r.RentedOn < currentDate.AddDays(-14) && r.ReturnedOn == null && !r.IsOverdue)
                .ToListAsync();

            foreach (var rental in overdueRentals)
            {
                rental.IsOverdue = true;

                _dbContext.RentalRecords.Update(rental);
            }

            await _dbContext.SaveChangesAsync();

            foreach (var rental in overdueRentals)
            {
                string subject = "Your Rental Book Is Overdue for Return";

                string htmlContent = GenerateDynamicHtmlEmail(subject, rental.User.FullName, rental.Book.Title + " by " + rental.Book.Author.FullName);

                string plainTextContent = "This is a plain text version of the email.";

                await _emailService.SendEmailAsync(rental.User.Email, subject, plainTextContent, htmlContent); // In real world project i would not send the email in a loop, I would rather combine them and try to send in one call.
            }
        }

        public string GenerateDynamicHtmlEmail(string subject, string recipientName, string bookName)
        {
            // Define the HTML structure with inline styles
            string htmlContent = @"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>${subject}</title>
            </head>
            <body style=""font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4;"">
                <div style=""width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);"">
                    <div style=""background-color: #0073e6; padding: 20px; text-align: center; color: #ffffff; border-radius: 8px 8px 0 0;"">
                        <h1 style=""margin: 0; font-size: 24px;"">${subject}</h1>
                    </div>
                    <div style=""padding: 20px; color: #333333; font-size: 16px;"">
                        <p>Hello ${recipientName},</p>
                        <p>We hope you’ve been enjoying your book! This is a friendly reminder that the book you rented, <strong>${bookName}</strong>, is now overdue for return.</p>
                        <p>Thank you for being a valued member of our community!</p>
                        <p>Best regards,<br>Book rental company</p>
                    </div> 
                </div>
            </body>
            </html>";

            // Replace placeholders with dynamic values
            htmlContent = htmlContent.Replace("${subject}", subject)
                                     .Replace("${recipientName}", recipientName)
                                     .Replace("${bookName}", bookName);

            return htmlContent;
        }
    }
}
