This repo has two hosting applications - Api (Openlane.Bids.Api) and Worker Service (Openlane.Bids.WorkerService) and one library which is Inftastructure code.
Api has two functionalities, one is storing bids on rabbimq and 2nd is fetching bids from database and store them on cache for next retrieval.
Worker service runs in background and fetcches the bids from queue and stores them on DB.

## Pre-requisite 
Docker Desktop  
Rest Client (for exmaple - Postman)

# Infrastructure Setup
## Set up containerized sql server, redis and rabbimq
execute docker compose file to run redis and rabbimq in container using below command
```
docker compose up -d
```

## Connect from other container
RabbitMQ: amqp://user:password@rabbitmq:5672  
Redis: redis://redis:6379

## Connect from local machine
RabbitMQ: localhost:5672  
Redis: redis://localhost:6379

##  Connect using browser
RabbitMQ: http://localhost:15672

## Api endpoints
GET: http://localhost:8080/api/bids?auctionId=1&carId=1&cursor=10&pageSize=12  
POST: http://localhost:8080/api/bids
```json
{
    "Amount": 400,
    "AuctionId":1,
    "BidderName": "Sachin",
    "CarId": 1,
    "Timestamp": "2025-02-26T12:34:56Z"
}
```

## Setup dockerized database
Open command prompt or shell client and run below commands:
```
docker exec -it sqlserver-db "bash"
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongP@ssw0rd123 -N -C
CREATE DATABASE OpenlaneDb;
GO
USE OpenlaneDb;
GO
Create TABLE Bids(Id INT PRIMARY KEY IDENTITY(1,1), AuctionId INT NOT NULL, CarId INT NOT NULL, Amount DECIMAL NOT NULL,[Timestamp] DATETIME NOT NULL); 
GO
CREATE INDEX IX_Bids_CarId_AuctionId ON Bids (CarId, AuctionId);
GO
CREATE PROCEDURE [dbo].[GetBid] @AuctionId INT, @CarId INT, @Cursor INT, @PageSize INT = 10 AS BEGIN SELECT TOP (@PageSize) * FROM Bids WHERE AuctionId = @AuctionId AND CarId = @CarId AND Id <= @Cursor ORDER BY Id Desc END;
GO
```
