namespace book_rental_api.Models
{

    public class StatsDTO
    {
        public BookBaseDTO? MostOverdueBook { get; set; }

        public BookBaseDTO? MostPopularBook { get; set; }


        public BookBaseDTO? LeastPopularBook { get; set; }

    }

    public class BookBaseDTO
    {
        public int BookId { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public string AuthorName { get; set; } = string.Empty;

        public int Count { get; set; }

    }
}
