CREATE TABLE Auctions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL
);

GO

CREATE TABLE Cars (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Make NVARCHAR(255) NOT NULL,
    Model NVARCHAR(255) NOT NULL,
    Year INT NOT NULL
);

GO

CREATE TABLE Bids (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    AuctionId UNIQUEIDENTIFIER NOT NULL,
    CarId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Timestamp DATETIME2 NOT NULL,
    CONSTRAINT FK_Bids_Auctions FOREIGN KEY (AuctionId) REFERENCES Auctions(Id),
    CONSTRAINT FK_Bids_Cars FOREIGN KEY (CarId) REFERENCES Cars(Id)
);

GO

CREATE INDEX IX_Bids_CarId_AuctionId
ON Bids (CarId, AuctionId)
INCLUDE ([Timestamp]) ;
GO

CREATE PROCEDURE SaveBid
    @Id UNIQUEIDENTIFIER,
    @AuctionId UNIQUEIDENTIFIER,
    @CarId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18, 2),
    @Timestamp DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Bids (Id, AuctionId, CarId, Amount, Timestamp)
    VALUES (@Id, @AuctionId, @CarId, @Amount, @Timestamp);
END

Go

create proc GetBid(
@AuctionId UNIQUEIDENTIFIER,
@Pagesize INT,
@Cursor UNIQUEIDENTIFIER
)
AS BEGIN
	SELECT TOP (@PageSize) * FROM Bids 
	WHERE AuctionId = @AuctionId 
    AND (@Cursor IS NULL OR Timestamp > (SELECT Timestamp FROM Bids WHERE Id = @Cursor))
    ORDER BY [Timestamp]
END