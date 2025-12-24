using System;
using System.Collections.Generic;

namespace LibrarySystem.Models;

public partial class Loan
{
    public int LoanId { get; set; }

    public int? BookId { get; set; }

    public int? MemberId { get; set; }

    public DateTime? LoanDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Member? Member { get; set; }
}
