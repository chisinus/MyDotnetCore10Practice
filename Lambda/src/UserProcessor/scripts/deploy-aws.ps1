Write-Host "Deploying Lambda to AWS..."

aws lambda update-function-code `
  --function-name UserProcessor `
  --zip-file fileb://lambda.zip `
  --no-cli-pager

Write-Host "AWS deployment complete."
