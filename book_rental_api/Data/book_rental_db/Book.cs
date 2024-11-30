using System;
using System.Collections.Generic;

namespace book_rental_api.Data.book_rental_db;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Isbn { get; set; }

    public int GenreId { get; set; }

    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
}
