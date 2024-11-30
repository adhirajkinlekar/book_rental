using System;
using System.Collections.Generic;

namespace book_rental_api.Data.book_rental_db;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
