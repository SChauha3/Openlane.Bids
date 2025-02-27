CREATE TABLE Auctions (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing primary key
    Name NVARCHAR(255) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL
);

GO

CREATE TABLE Cars (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Make NVARCHAR(255) NOT NULL,
    Model NVARCHAR(255) NOT NULL,
    Year INT NOT NULL
);
GO

CREATE TABLE Bids (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AuctionId INT NOT NULL,
    CarId INT NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Timestamp DATETIME2 NOT NULL,
    CONSTRAINT FK_Bids_Auctions FOREIGN KEY (AuctionId) REFERENCES Auctions(Id),
    CONSTRAINT FK_Bids_Cars FOREIGN KEY (CarId) REFERENCES Cars(Id)
);
GO

CREATE INDEX IX_Bids_CarId_AuctionId
ON Bids (CarId, AuctionId);
GO

CREATE PROCEDURE SaveBid(
    @AuctionId INT,
    @CarId INT,
    @Amount DECIMAL(18, 2),
    @Timestamp DATETIME)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Bids (AuctionId, CarId, Amount, Timestamp)
    VALUES (@AuctionId, @CarId, @Amount, @Timestamp);
END

Go

create proc GetBid(
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