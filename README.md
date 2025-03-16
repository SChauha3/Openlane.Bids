This repo has two hosting applications - Api (Openlane.Bids.Api) and Worker Service (Openlane.Bids.WorkerService) and one library which is Inftastructure code.  
Api has two functionalities, one is storing bids on rabbimq and 2nd is fetching bids from database and store them on cache for next retrieval.  
Worker service runs in background and fetcches the bids from queue and stores them on DB.  
Application is reachable at http://localhost:8080/

## Pre-requisite 
Docker Desktop  
Visual Studio/VSCode

## Set up containerized sql server, redis and rabbimq
execute docker compose file to run redis and rabbimq in container using below command
```
docker compose up -d
```
Don't forget to start stopped containers manually.

## Connect from other container
RabbitMQ: amqp://user:password@rabbitmq:5672  
Redis: redis://redis:6379

## Connect from local machine
RabbitMQ: localhost:5672  
Redis: redis://localhost:6379

##  Connect using browser
RabbitMQ: http://localhost:15672

## Api Using Guidelines
```json
{
  "openapi": "3.0.1",
  "info": {
    "title": "Minimal API with OpenAPI",
    "description": "Bids Api",
    "contact": {
      "name": "sachin Chauhan",
      "email": "sachinchauhan.cs1988@gmail.com"
    },
    "version": "v1"
  },
  "paths": {
    "/api/bids": {
      "get": {
        "tags": [
          "Openlane.Bids.Api"
        ],
        "parameters": [
          {
            "name": "auctionId",
            "in": "query",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "carId",
            "in": "query",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "cursor",
            "in": "query",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "pageSize",
            "in": "query",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/IResultTask"
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Openlane.Bids.Api"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateBid"
              }
            }
          },
          "required": true
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/IResultTask"
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "CreateBid": {
        "type": "object",
        "properties": {
          "auctionId": {
            "type": "integer",
            "format": "int32"
          },
          "carId": {
            "type": "integer",
            "format": "int32"
          },
          "bidderName": {
            "type": "string",
            "nullable": true
          },
          "amount": {
            "type": "number",
            "format": "double"
          }
        },
        "additionalProperties": false
      },
      "IResult": {
        "type": "object",
        "additionalProperties": false
      },
      "IResultTask": {
        "type": "object",
        "properties": {
          "result": {
            "$ref": "#/components/schemas/IResult"
          }
        },
        "additionalProperties": false
      }
    }
  }
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
Create TABLE Bids(Id INT PRIMARY KEY IDENTITY(1,1), TransactionId UNIQUEIDENTIFIER, AuctionId INT NOT NULL, BidderName NVARCHAR(100), CarId INT NOT NULL, Amount DECIMAL NOT NULL,[Timestamp] DATETIME NOT NULL);
GO
CREATE INDEX IX_Bids_CarId_AuctionId ON Bids (CarId, AuctionId);
GO
CREATE PROCEDURE [dbo].[GetBid] @AuctionId INT, @CarId INT, @Cursor INT, @PageSize INT = 10 AS BEGIN SELECT TOP (@PageSize) * FROM Bids WHERE AuctionId = @AuctionId AND CarId = @CarId AND Id <= @Cursor ORDER BY Id Desc END;
GO
```
## Load Test Results
![Bids API Screenshot](tests/load/results/load-test-get.jpg)
![Bids API Screenshot](tests/load/results/load-test-post.jpg)