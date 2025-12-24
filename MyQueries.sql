USE LibraryDB;
GO

-- 1. Indexes for Optimization
CREATE INDEX IX_Books_ISBN ON Books(ISBN);
CREATE INDEX IX_Members_Email ON Members(Email);
GO

-- 2. View for Active Loans
CREATE VIEW v_ActiveLoans AS
SELECT 
    L.LoanID,
    M.FirstName + ' ' + M.LastName AS MemberName,
    B.Title AS BookTitle,
    L.LoanDate,
    L.DueDate
FROM Loans L
JOIN Members M ON L.MemberID = M.MemberID
JOIN Books B ON L.BookID = B.BookID
WHERE L.ReturnDate IS NULL;
GO

-- 3. Stored Procedure for Transactions
CREATE PROCEDURE sp_RegisterLoan
    @BookID INT,
    @MemberID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Stock INT;
        SELECT @Stock = AvailableCopies FROM Books WHERE BookID = @BookID;

        IF @Stock > 0
        BEGIN
            INSERT INTO Loans (BookID, MemberID, DueDate)
            VALUES (@BookID, @MemberID, DATEADD(day, 14, GETDATE()));

            UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookID = @BookID;

            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN
            ROLLBACK TRANSACTION;
            THROW 50001, 'Error: Out of stock.', 1;
        END
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 4. Trigger for Automation
CREATE TRIGGER TR_UpdateStockOnReturn
ON Loans
AFTER UPDATE
AS
BEGIN
    IF UPDATE(ReturnDate)
    BEGIN
        UPDATE Books
        SET AvailableCopies = AvailableCopies + 1
        FROM Books b
        INNER JOIN inserted i ON b.BookID = i.BookID
        WHERE i.ReturnDate IS NOT NULL AND i.ReturnDate <= GETDATE();
    END
END;
GO