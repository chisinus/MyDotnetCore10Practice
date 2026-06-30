List tables:

aws dynamodb list-tables --endpoint-url http://localhost:4566  (LocalStack)
aws dynamodb list-tables --endpoint-url http://localhost:8000  (DynamoDB Local)

Enable execution policy for PowerShell:

Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
For current user: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

List all functions:

aws lambda list-functions --endpoint-url http://localhost:4566