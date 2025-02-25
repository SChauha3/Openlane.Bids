## Getting started
## Set up local redis

# Pre-requisite 
Docker Desktop

Run below commands

docker pull redis
docker run -d --name redis-stack-server -p 6379:6379 redis:latest

