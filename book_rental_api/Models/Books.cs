using Microsoft.AspNetCore.Mvc;

namespace book_rental_api.Models
{
    public class BooksListDTO
    {
        public string Title { get; set; } = string.Empty!;

        public string Author { get; set; } = string.Empty!;

        public string Genre { get; set; } = string.Empty!;

        public string ISBN { get; set; } = string.Empty!;

    }

    public class BooksQueryDTO
    {
        [FromQuery]
        public string Title { get; set; } = string.Empty!;

        [FromQuery]
        public string Genre { get; set; } = string.Empty!;
    }
}
