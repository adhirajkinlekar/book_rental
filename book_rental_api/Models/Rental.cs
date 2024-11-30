namespace book_rental_api.Models
{
    public class RentalHistoryDTO
    {
        public int RentalId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookIsbn { get; set; }
        public DateTime RentedOn { get; set; }
        public DateTime? ReturnedOn { get; set; }
        public bool? IsOverdue { get; set; }
    }

}
