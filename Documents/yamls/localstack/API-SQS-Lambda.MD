Here’s a **clean, realistic, end‑to‑end C# integration example** showing how **SQS → Lambda → DynamoDB** works when you run everything locally on **LocalStack**.

This is the exact pattern your team will use in production, just with LocalStack endpoints swapped in.

I’ll give you:

1. **Architecture flow**
2. **SQS message producer (C# API service)**
3. **Lambda handler (C#)**
4. **DynamoDB repository (C#)**
5. **LocalStack setup commands**
6. **End‑to‑end flow demonstration**

Everything is self‑contained and ready to drop into your project.

---

## 🧭 Architecture Flow

### **1. Your C# API sends a message to SQS**
Example:  
```json
{ "photoId": "123", "fileName": "cat.png" }
```

### **2. Lambda is triggered by SQS**
Lambda receives the message body.

### **3. Lambda writes the item into DynamoDB**
Using your repository class.

---

## 🧱 1. SQS Message Producer (C# API Service)

This is your ASP.NET service or background worker sending messages to SQS.

```csharp
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

public class PhotoQueueService
{
    private readonly AmazonSQSClient _sqs;
    private readonly string _queueUrl;

    public PhotoQueueService(bool useLocal = true)
    {
        var config = new AmazonSQSConfig();

        if (useLocal)
            config.ServiceURL = "http://localhost:4566";

        _sqs = new AmazonSQSClient(config);
        _queueUrl = "http://localhost:4566/000000000000/photo-queue";
    }

    public async Task EnqueuePhotoAsync(string id, string fileName)
    {
        var payload = new
        {
            PhotoId = id,
            FileName = fileName
        };

        var message = JsonSerializer.Serialize(payload);

        await _sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = message
        });
    }
}
```

---

## 🧩 2. DynamoDB Repository (C#)

Same pattern you already used.

```csharp
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

[DynamoDBTable("PhotoItems")]
public class PhotoItem
{
    [DynamoDBHashKey]
    public string Id { get; set; }

    [DynamoDBProperty]
    public string FileName { get; set; }

    [DynamoDBProperty]
    public DateTime CreatedAt { get; set; }
}

public class PhotoRepository
{
    private readonly DynamoDBContext _context;

    public PhotoRepository(bool useLocal = true)
    {
        var config = new AmazonDynamoDBConfig();

        if (useLocal)
            config.ServiceURL = "http://localhost:4566";

        var client = new AmazonDynamoDBClient(config);
        _context = new DynamoDBContext(client);
    }

    public async Task SaveAsync(PhotoItem item)
    {
        await _context.SaveAsync(item);
    }
}
```

---

## ⚡ 3. Lambda Handler (C#)

This Lambda:

- Receives SQS messages  
- Deserializes JSON  
- Writes to DynamoDB  

```csharp
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

public class Function
{
    private readonly PhotoRepository _repo = new PhotoRepository(useLocal: true);

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            var body = record.Body;

            var data = JsonSerializer.Deserialize<PhotoMessage>(body);

            var item = new PhotoItem
            {
                Id = data.PhotoId,
                FileName = data.FileName,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.SaveAsync(item);
        }
    }
}

public class PhotoMessage
{
    public string PhotoId { get; set; }
    public string FileName { get; set; }
}
```

---

## 🧪 4. LocalStack Setup Commands

### **Create SQS queue**
```
aws sqs create-queue --queue-name photo-queue --endpoint-url http://localhost:4566
```

### **Create DynamoDB table**
```
aws dynamodb create-table \
  --table-name PhotoItems \
  --attribute-definitions AttributeName=Id,AttributeType=S \
  --key-schema AttributeName=Id,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --endpoint-url http://localhost:4566
```

### **Deploy Lambda to LocalStack**
Assuming you built your Lambda into `lambda.zip`:

```
aws lambda create-function \
  --function-name PhotoProcessor \
  --runtime dotnet8 \
  --handler Function::Function.FunctionHandler \
  --zip-file fileb://lambda.zip \
  --role arn:aws:iam::000000000000:role/lambda-role \
  --endpoint-url http://localhost:4566
```

### **Connect SQS → Lambda**
```
aws lambda create-event-source-mapping \
  --function-name PhotoProcessor \
  --event-source-arn arn:aws:sqs:us-east-1:000000000000:photo-queue \
  --endpoint-url http://localhost:4566
```

---

## 🎯 5. End‑to‑End Flow Demo

### **Step 1 — Your API sends a message**
```csharp
await photoQueueService.EnqueuePhotoAsync("123", "cat.png");
```

### **Step 2 — LocalStack SQS receives it**

### **Step 3 — Lambda is triggered automatically**

### **Step 4 — Lambda writes to DynamoDB**

### **Step 5 — Verify DynamoDB**
```
aws dynamodb scan --table-name PhotoItems --endpoint-url http://localhost:4566
```

You’ll see:

```json
{
  "Items": [
    {
      "Id": { "S": "123" },
      "FileName": { "S": "cat.png" },
      "CreatedAt": { "S": "2026-06-30T14:00:00Z" }
    }
  ]
}
```

---

## 🚀 Want the next step?

I can generate:

- A **full ASP.NET Web API project** wired to SQS  
- A **Lambda project template** with build + deploy scripts  
- A **complete LocalStack integration test suite**  
- A **multi‑service architecture diagram**  

Just tell me what you want to build next.