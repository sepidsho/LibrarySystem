# Library Management System

## Project Description
This is a robust Library Management System built with **C# (Entity Framework Core)** and **SQL Server**. The system handles Members, Books, Authors, and Loans. It is designed to ensure Data Integrity using SQL Transactions and optimized for performance using Indexes.

## ER Diagram
The database is normalized to **3NF**.
- **Authors** are separated to avoid redundancy.
- **Loans** link Members and Books with transaction dates.

<img width="890" height="439" alt="Screenshot 2025-12-24 193935" src="https://github.com/user-attachments/assets/58842d5b-dcc8-4cdf-9e64-5cdef0420db9" />


## Technical Analysis 

### 1. Optimization & Indexes
To improve query performance, I analyzed the execution plans.
- **Clustered Index:** Automatically created on Primary Keys (e.g., BookID).
- **Non-Clustered Index:** I created an index on `Books(ISBN)` and `Members(Email)` because these columns are frequently used for searching.
- **Result:** As shown in the Execution Plan below, searching by ISBN uses an **Index Seek** instead of a full Table Scan, significantly reducing the cost of the query.
<img width="960" height="540" alt="Screenshot 2025-12-24 mylibrary" src="https://github.com/user-attachments/assets/c356b6c7-c03e-4536-bfa7-59bb7dc7fc41" />






### 2. Data Integrity & Transactions
Handling concurrent loans is critical.
- **Stored Procedure (`sp_RegisterLoan`):** I used a Stored Procedure wrapped in a `BEGIN TRANSACTION` block. This ensures that checking the stock and inserting the loan record happen atomically. If an error occurs (e.g., stock is 0), the entire transaction is rolled back (`ROLLBACK`), preventing data inconsistency.
- **Concurrency Control:** The system operates under the default `READ COMMITTED` isolation level, which prevents "Dirty Reads". By locking the row during the update, we ensure that two users cannot borrow the last copy of a book simultaneously.

### 3. Triggers & Automation
- **Trigger (`TR_UpdateStockOnReturn`):** A database trigger is implemented to automatically increment the `AvailableCopies` in the `Books` table whenever a `ReturnDate` is set in the `Loans` table. This guarantees that the stock count is always accurate, regardless of how the update is performed (via App or direct SQL).

## How to Run
1. Execute the `LibraryDB.sql` script in SQL Server to create the database and data.
2. Update the connection string in `LibraryDbContext.cs` if necessary.
3. Run the Console Application.
