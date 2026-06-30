Write-Host "Building Lambda..."

dotnet clean ../
dotnet publish ../Lambda.csproj -c Release -r linux-x64 --self-contained false -o ../publish

Compress-Archive -Path ../publish/* -DestinationPath ../lambda.zip -Force

Write-Host "Build complete: lambda.zip"
