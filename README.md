# RaftLabs .NET Developer Assignment – External User Service

A robust .NET 8 class library for interacting with external user APIs, featuring caching, retry policies, clean architecture, and comprehensive error handling.

---

## 🏗️ Architecture Overview

This solution follows **Clean Architecture** principles, ensuring separation of concerns and ease of maintenance:

- **Models**: DTOs and API response models  
- **Services**: Business logic and orchestration  
- **Clients**: HTTP API communication layer  
- **Configuration**: Strongly-typed settings  
- **Extensions**: Dependency injection setup  

---

## 🚀 Features

### ✅ Core Requirements

- HTTP client implementation using `IHttpClientFactory`  
- Async/await pattern throughout  
- POCOs/DTOs for data modeling  
- JSON deserialization of API responses  
- Service layer with reusable methods  
- Configurable base URL and settings  
- Comprehensive error handling  
- Unit tests using **xUnit** and **Moq**

### 🎁 Bonus Features

- In-memory caching with configurable expiration  
- Retry policies using **Polly**  
- Options pattern for structured configuration  
- Clean Architecture-based project structure  
- Structured and semantic logging  
- Extension methods for DI setup  

---

## 📁 Project Structure

```
RaftLabsAssignment/
├── src/
│   ├── RaftLabs.ExternalUserService/     # Core library
│   └── RaftLabs.ConsoleApp/              # Demo console app
├── tests/
│   └── RaftLabs.ExternalUserService.Tests/  # Unit tests
├── RaftLabsAssignment.sln
└── README.md
```

---

## 🛠️ Prerequisites

- .NET 8 SDK  
- Visual Studio 2022 or VS Code  
- Git

---

## 📦 Dependencies

### Core Library

- `Microsoft.Extensions.Http` – HTTP client factory  
- `Microsoft.Extensions.Caching.Memory` – In-memory caching  
- `Polly.Extensions.Http` – Retry policies  
- `System.Text.Json` – JSON serialization  

### Console App

- `Microsoft.Extensions.Hosting` – Generic host for DI  

### Testing

- `xUnit` – Testing framework  
- `Moq` – Mocking library  
- `Microsoft.NET.Test.Sdk` – Test runner  

---

## 🏃 Getting Started

### 1. Clone the Repository

```bash
git clone <repo-url>
cd RaftLabsAssignment
```

### 2. Build the Solution

```bash
dotnet restore
dotnet build
```

### 3. Run Tests

```bash
dotnet test
```

### 4. Run the Console Demo

```bash
cd src/RaftLabs.ConsoleApp
dotnet run
```

---

## 📖 Usage Examples

### Service Registration

```csharp
services.AddExternalUserService(configuration);
```

### Controller Usage

```csharp
public class MyController : ControllerBase
{
    private readonly IExternalUserService _userService;

    public MyController(IExternalUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}
```

### Configuration (`appsettings.json`)

```json
{
  "ExternalApi": {
    "BaseUrl": "https://reqres.in/api/",
    "TimeoutSeconds": 30,
    "MaxRetryAttempts": 3,
    "CacheExpirationMinutes": 10
  }
}
```

---

## 🔧 Configuration Options

| Setting                   | Default                    | Description                          |
|---------------------------|----------------------------|--------------------------------------|
| `BaseUrl`                 | `https://reqres.in/api/`   | Base URL for the external API        |
| `TimeoutSeconds`          | `30`                       | HTTP request timeout in seconds      |
| `MaxRetryAttempts`        | `3`                        | Retry attempts for transient errors  |
| `CacheExpirationMinutes`  | `10`                       | Expiration time for cached entries   |

---

## 🧪 Testing

Comprehensive unit tests include:

- API Client: HTTP communication, deserialization, error handling  
- Service Layer: Business logic, caching, pagination  
- Integration Scenarios: End-to-end service behavior

### Run Tests

```bash
dotnet test
```

### Run with Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Class

```bash
dotnet test --filter "ClassName=UserApiClientTests"
```

---

## 🔄 Error Handling

Layered error-handling ensures resilience:

1. **Network Errors** – Polly retries with exponential backoff  
2. **HTTP Errors** – Custom handling for status codes  
3. **Deserialization Errors** – Graceful JSON error fallback  
4. **Timeouts** – Controlled with configuration  

### Common Exceptions

- `InvalidOperationException`: Communication failures  
- `TimeoutException`: Request exceeded configured timeout  
- `ArgumentException`: Invalid input parameters  

---

## 📊 Caching Strategy

- **In-Memory Caching**: Quick retrieval of frequent requests  
- **Cache Expiration**: Set via configuration  
- **Cache Keys**: Structured for consistency  
- **Cache-Aside Pattern**: Load data on cache miss  

---

## 🔁 Retry Policy with Polly

- **Retry Count**: 3 attempts by default  
- **Backoff**: Exponential (2^n seconds)  
- **Handled Errors**: Transient errors like timeouts, 5xx responses  
- **Logging**: Retry attempts are logged for observability  

---

## 🎯 Design Decisions

### Why `IHttpClientFactory`?

- Avoids socket exhaustion  
- Enables named/typed clients  
- Manages client lifecycle properly  

### Why Polly?

- Standard .NET library for fault tolerance  
- Supports retry, timeout, circuit breakers  
- Integrated with `HttpClientFactory`  

### Why Memory Cache?

- Fast and lightweight  
- Suitable for single-instance apps  
- Easily replaceable with distributed cache  

### Why Clean Architecture?

- Separation of concerns  
- High testability  
- Scalable and maintainable  

---

## 🚦 Performance Considerations

- **Async/Await**: Non-blocking I/O  
- **Connection Pooling**: Reuses `HttpClient` connections  
- **Memory Caching**: Reduces redundant API calls  
- **Pagination**: Handles large datasets efficiently  

---

## 🔒 Security Considerations

- Validates all input  
- Safe JSON deserialization  
- Avoids leaking sensitive data in logs  
- Timeout protection against long-running requests  

---

## 🚀 Potential Enhancements

- ✅ **Distributed Caching** (e.g., Redis)  
- ✅ **Circuit Breaker** with Polly  
- ✅ **Authentication**: API key / OAuth support  
- ✅ **Rate Limiting**: Client-side throttling  
- ✅ **Health Checks**  
- ✅ **Metrics & Monitoring**

---


