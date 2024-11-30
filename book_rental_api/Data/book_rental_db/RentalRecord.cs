using System;
using System.Collections.Generic;

namespace book_rental_api.Data.book_rental_db;

public partial class RentalRecord
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public Guid UserId { get; set; }

    public DateTime RentedOn { get; set; }

    public DateTime? ReturnedOn { get; set; }

    public bool IsOverdue { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
