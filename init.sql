CREATE DATABASE OpenlaneDb;
GO

USE OpenlaneDb;

Create TABLE Bids(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AuctionId INT NOT NULL,
    CarId INT NOT NULL,
    Amount DECIMAL NOT NULL,
    [Timestamp] DATETIME NOT NULL
);

CREATE INDEX IX_Bids_CarId_AuctionId
ON Bids (CarId, AuctionId);
GO

CREATE PROCEDURE GetBid(
@AuctionId INT,
@Pagesize INT,
@Cursor INT
)
AS BEGIN
	SELECT TOP (@PageSize) * FROM Bids 
	WHERE AuctionId = @AuctionId 
    AND Id < @Cursor
    ORDER BY Id Desc
END