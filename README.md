## Pre-requisite 
Docker Desktop

##Infrastructure Set Up

## Set up local redis and rabbimq
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

## SQL Server
It can be downloaded from https://www.microsoft.com/en-us/sql-server/sql-server-downloads