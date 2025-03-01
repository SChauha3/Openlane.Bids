## Pre-requisite 
Docker Desktop

##Infrastructure Set Up

## Set up containerized sql server, redis and rabbimq
execute docker compose file to run redis and rabbimq in container using below command
docker compose up -d

## Connect from other container
RabbitMQ: amqp://user:password@rabbitmq:5672
Redis: redis://redis:6379

## Connect from local machine
RabbitMQ: localhost:5672
Redis: redis://localhost:6379

##  Connect using browser
http://localhost:15672

docker exec -it sqlserver "bash"
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongP@ssw0rd123 -N -C

CREATE DATABASE OpenlaneDb;
GO

USE OpenlaneDb;

Create TABLE Bids(Id INT PRIMARY KEY IDENTITY(1,1), AuctionId INT NOT NULL, CarId INT NOT NULL, Amount DECIMAL NOT NULL,[Timestamp] DATETIME NOT NULL);

