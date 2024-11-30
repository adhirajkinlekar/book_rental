using System;
using System.Collections.Generic;

namespace book_rental_api.Data.book_rental_db;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
}
