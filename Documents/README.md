# MyDotnetCore10Practice

Down-grade shared project to .net 8.0, so we can use it with LocalStack .NET Lambda runtime

Install AWS DynamoDB in a Docker

docker-compose.yml

========================================================================================
version: "3.8"

services:
  dynamodb:
    image: amazon/dynamodb-local
    ports:
      - "8000:8000"
    # 1. Override the default command to use the database path
    command: "-jar DynamoDBLocal.jar -sharedDb -dbPath /home/dynamodblocal/data"
    # 2. Mount the local folder to the container's data directory
    volumes:
      - C:/practice/dynamodb-data:/home/dynamodblocal/data
========================================================================================

docker compose up -d


Install LocalStack in a Docker to support 
    SQS
    SNS
    Lambda
    DynamoDB
    S3
    API Gateway

========================================================================================
version: "3.8"

services:
  localstack:
    image: localstack/localstack:latest
    container_name: localstack
    ports:
      - "4566:4566"        # Main LocalStack edge port
      - "4571:4571"        # Lambda API (optional)
    environment:
      - SERVICES=sqs,sns,lambda,dynamodb,s3,apigateway
      - DEBUG=1
      - LAMBDA_EXECUTOR=docker
      - DOCKER_HOST=unix:///var/run/docker.sock
      - AWS_DEFAULT_REGION=us-east-1
    volumes:
      - "./localstack:/var/lib/localstack"
      - "/var/run/docker.sock:/var/run/docker.sock"
========================================================================================

docker compose up -d

Set up SQS

aws sqs create-queue --queue-name xj-queue --endpoint-url http://localhost:4566

To create a clean Lambda zip file:

dotnet publish src/LambdaUserProcessor/Lambda.csproj -c Release -r linux-x64 --self-contained false -o publish
Compress-Archive -Path publish/* -DestinationPath lambda.zip -Force

