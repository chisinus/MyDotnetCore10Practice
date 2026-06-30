Write-Host "Deploying Lambda to LocalStack..."

# Get the absolute path of lambda.zip in the parent folder relative to this script
$zipPath = "$PSScriptRoot/../lambda.zip".Replace('\', '/')

# 1. Delete the function if it exists
aws lambda delete-function `
  --function-name UserProcessor `
  --endpoint-url http://localhost:4566 `
  --no-cli-pager `
  2>$null

# 2. Check for and delete any orphaned event source mappings
$mapping = aws lambda list-event-source-mappings `
  --function-name UserProcessor `
  --endpoint-url http://localhost:4566 `
  --query "EventSourceMappings[0].UUID" `
  --output text `
  2>$null

if ($mapping -and $mapping -ne "None") {
    Write-Host "Deleting existing event source mapping ($mapping)..."
    aws lambda delete-event-source-mapping `
      --uuid $mapping `
      --endpoint-url http://localhost:4566 `
      --no-cli-pager `
      2>$null
}

# 3. Create the Lambda function
aws lambda create-function `
  --function-name UserProcessor `
  --runtime dotnet8 `
  --handler Lambda::Lambda.Function::FunctionHandler `
  --zip-file fileb://$zipPath `
  --role arn:aws:iam::000000000000:role/lambda-role `
  --endpoint-url http://localhost:4566

# 4. Map SQS queue to the Lambda function
aws lambda create-event-source-mapping `
  --function-name UserProcessor `
  --event-source-arn arn:aws:sqs:us-east-1:000000000000:xj-queue `
  --endpoint-url http://localhost:4566

Write-Host "LocalStack deployment complete."