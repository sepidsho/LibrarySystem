using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibrarySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new LibraryDbContext())
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=== Library Management System (VG Level) ===");
                    Console.WriteLine("1. Add New Member");
                    Console.WriteLine("2. List All Members");
                    Console.WriteLine("3. Add New Book");
                    Console.WriteLine("4. List All Books (Check Stock)");
                    Console.WriteLine("5. Borrow a Book (Register Loan)");
                    Console.WriteLine("6. Return a Book (Test Trigger)");
                    Console.WriteLine("7. View Active Loans");
                    Console.WriteLine("8. Exit");
                    Console.Write("\nPlease select an option: ");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": AddMember(context); break;
                        case "2": ListMembers(context); break;
                        case "3": AddBook(context); break;
                        case "4": ListBooks(context); break;
                        case "5": BorrowBook(context); break;
                        case "6": ReturnBook(context); break;
                        case "7": ListActiveLoans(context); break;
                        case "8": return;
                        default: Console.WriteLine("Invalid option."); break;
                    }

                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        // --- 1. اعضا ---
        static void AddMember(LibraryDbContext context)
        {
            Console.WriteLine("\n--- Add New Member ---");
            Console.Write("First Name: "); string fName = Console.ReadLine();
            Console.Write("Last Name: "); string lName = Console.ReadLine();
            Console.Write("Email: "); string email = Console.ReadLine();

            var newMember = new Member { FirstName = fName, LastName = lName, Email = email, JoinDate = DateTime.Now };
            try { context.Members.Add(newMember); context.SaveChanges(); Console.WriteLine("Success!"); }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static void ListMembers(LibraryDbContext context)
        {
            Console.WriteLine("\n--- Member List ---");
            foreach (var m in context.Members.ToList())
                Console.WriteLine($"ID: {m.MemberId} | {m.FirstName} {m.LastName}");
        }

        // --- 2. کتاب‌ها ---
        static void AddBook(LibraryDbContext context)
        {
            Console.WriteLine("\n--- Add New Book ---");
            Console.Write("Title: "); string title = Console.ReadLine();
            Console.Write("ISBN: "); string isbn = Console.ReadLine();
            Console.Write("Author First Name: "); string af = Console.ReadLine();
            Console.Write("Author Last Name: "); string al = Console.ReadLine();
            Console.Write("Copies: "); int copies = int.Parse(Console.ReadLine());

            var book = new Book { Title = title, Isbn = isbn, AvailableCopies = copies, Author = new Author { FirstName = af, LastName = al } };
            try { context.Books.Add(book); context.SaveChanges(); Console.WriteLine("Success!"); }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static void ListBooks(LibraryDbContext context)
        {
            // این خط مهم برای پاک کردن حافظه کش و دیدن تغییرات تریگر
            context.ChangeTracker.Clear();

            Console.WriteLine("\n--- Book List ---");
            var books = context.Books.Include(b => b.Author).ToList();
            foreach (var b in books)
                Console.WriteLine($"ID: {b.BookId} | {b.Title} | Copies: {b.AvailableCopies}");
        }

        // --- 3. امانت دادن (Stored Procedure) ---
        static void BorrowBook(LibraryDbContext context)
        {
            Console.WriteLine("\n--- Borrow a Book ---");
            Console.Write("Enter Member ID: "); int memberId = int.Parse(Console.ReadLine());
            Console.Write("Enter Book ID: "); int bookId = int.Parse(Console.ReadLine());

            try
            {
                context.Database.ExecuteSqlRaw("EXEC sp_RegisterLoan {0}, {1}", bookId, memberId);
                Console.WriteLine(">>> Loan registered successfully! (Stock reduced via Transaction)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction Failed: {ex.Message}");
            }
        }

        // --- 4. نمایش وام‌های فعال ---
        static void ListActiveLoans(LibraryDbContext context)
        {
            Console.WriteLine("\n--- Active Loans ---");
            var loans = context.Loans.Include(l => l.Book).Include(l => l.Member).Where(l => l.ReturnDate == null).ToList();

            if (loans.Count == 0) Console.WriteLine("No active loans.");

            foreach (var l in loans)
            {
                Console.WriteLine($"LoanID: {l.LoanId} | Book: {l.Book.Title} | Member: {l.Member.FirstName} {l.Member.LastName}");
            }
        }

        // --- 5. بازگرداندن کتاب (Trigger Test) ---
        static void ReturnBook(LibraryDbContext context)
        {
            Console.WriteLine("\n--- Return a Book ---");
            Console.Write("Enter Loan ID (Not Book ID!): ");
            int loanId = int.Parse(Console.ReadLine());

            var loan = context.Loans.Find(loanId);

            if (loan != null && loan.ReturnDate == null)
            {
                loan.ReturnDate = DateTime.Now;
                context.SaveChanges();
                // اینجا تریگر SQL اجرا میشه و موجودی رو زیاد میکنه
                Console.WriteLine(">>> Book returned! Trigger executed (Stock +1).");
            }
            else
            {
                Console.WriteLine("Loan not found or already returned.");
            }
        }
    }
}