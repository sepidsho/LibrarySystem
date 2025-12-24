using System;
using System.Collections.Generic;

namespace LibrarySystem.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Isbn { get; set; }

    public int? AuthorId { get; set; }

    public int? AvailableCopies { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Author? Author { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
