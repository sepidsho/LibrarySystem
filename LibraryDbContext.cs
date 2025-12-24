using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Models;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC14B685C4C9");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C2279ADBBE35");

            entity.HasIndex(e => e.Isbn, "IX_Books_ISBN");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA95A73BB1").IsUnique();

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.AvailableCopies).HasDefaultValue(1);
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Books__AuthorID__00200768");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD43715E9A377");

            entity.ToTable(tb => tb.HasTrigger("TR_UpdateStockOnReturn"));

            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.LoanDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Loans)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Loans__BookID__08B54D69");

            entity.HasOne(d => d.Member).WithMany(p => p.Loans)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Loans__MemberID__09A971A2");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Members__0CF04B38B6EDC09B");

            entity.HasIndex(e => e.Email, "IX_Members_Email");

            entity.HasIndex(e => e.Email, "UQ__Members__A9D10534CC667B9B").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
