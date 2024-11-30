using System;
using System.Collections.Generic;

namespace book_rental_api.Data.book_rental_db;

public partial class Author
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
