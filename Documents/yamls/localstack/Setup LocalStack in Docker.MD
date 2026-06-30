Here’s a clean, modern **docker‑compose.yml** that includes all the AWS services you asked for:

- **SQS**  
- **SNS**  
- **Lambda**  
- **DynamoDB**  
- **S3**  
- **API Gateway**

All running locally through **LocalStack**, with persistence enabled and ports exposed for development tools.

---

## ⭐ Updated `docker-compose.yml` (Full AWS Local Dev Stack)

```yaml
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
      - LOCALSTACK_AUTH_TOKEN=ls-furU6385-RADa-RuDu-WAmo-cEhE43075bdc
    volumes:
      - "./localstack:/var/lib/localstack"
      - "/var/run/docker.sock:/var/run/docker.sock"
```

---

## 🧠 What this compose file gives you

### ✔ **SQS**
Local queue creation:
```
aws sqs create-queue --queue-name my-queue --endpoint-url http://localhost:4566
```

### ✔ **SNS**
Create a topic:
```
aws sns create-topic --name my-topic --endpoint-url http://localhost:4566
```

### ✔ **Lambda**
LocalStack supports:

- Python  
- Node.js  
- Java  
- .NET (via custom runtime)

Invoke:
```
aws lambda invoke --function-name my-func out.json --endpoint-url http://localhost:4566
```

### ✔ **DynamoDB**
Same as your existing setup, but now inside LocalStack:
```
aws dynamodb list-tables --endpoint-url http://localhost:4566
```

### ✔ **S3**
Create a bucket:
```
aws s3 mb s3://my-bucket --endpoint-url http://localhost:4566
```

### ✔ **API Gateway**
Create a REST API:
```
aws apigateway create-rest-api --name my-api --endpoint-url http://localhost:4566
```

---

## 🎯 Why this setup is ideal for your team

### 1. **One container, all AWS services**
No need to run DynamoDB Local separately anymore.

### 2. **Perfect for local C# development**
Your .NET code points to:

```
http://localhost:4566
```

### 3. **Supports integration testing**
You can simulate:

- SQS → Lambda  
- SNS → SQS  
- API Gateway → Lambda → DynamoDB  
- S3 triggers  
- Full microservice flows  

### 4. **Persistent storage**
The `./localstack` folder keeps your data between restarts.

---

## 🧪 If you want, I can generate next:

### 🔹 C# SQS service class (send, receive, delete)  
### 🔹 C# SNS publisher  
### 🔹 C# Lambda handler template  
### 🔹 C# DynamoDB + SQS + Lambda integration example  
### 🔹 Full local microservice architecture using LocalStack  

Just tell me what you want to build next.