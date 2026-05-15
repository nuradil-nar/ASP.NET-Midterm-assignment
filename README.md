# Task Tracker API – Midterm Assignment

A distributed task management microservice built with **ASP.NET Core 8**, implementing OOP, LINQ, events, REST API, Dependency Injection, and Docker containerization.

---

## Project Structure

```
TaskTrackerAPI/
├── src/
│   ├── Models/
│   │   ├── BaseTask.cs          # Abstract base class + delegate/event
│   │   ├── BugReportTask.cs     # Derived class with SeverityLevel
│   │   ├── FeatureRequestTask.cs# Derived class with EstimatedHours
│   │   └── DTOs.cs              # Request/Response records
│   ├── Repositories/
│   │   ├── ITaskRepository.cs   # Interface
│   │   └── InMemoryTaskRepository.cs # In-memory implementation
│   ├── Services/
│   │   ├── TaskFilterService.cs # Static LINQ filter service (Block 1)
│   │   └── TaskMapper.cs        # Domain → DTO mapper
│   ├── Controllers/
│   │   └── TasksController.cs   # Web API controller (Block 2)
│   └── Program.cs               # Entry point + DI setup
├── Dockerfile                   # Multi-stage build
├── docker-compose.yml
├── ARCHITECTURE.md              # Block 3 – integration pattern analysis
└── TaskTrackerAPI.csproj
```

---

## Running Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
cd TaskTrackerAPI
dotnet run --project src
```

Open Swagger UI at: **http://localhost:5000/swagger**

---

## Running with Docker

```bash
docker-compose up --build
```

API available at: **http://localhost:8080**

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get task by ID |
| GET | `/api/tasks/filtered` | LINQ-filtered results (Block 1) |
| POST | `/api/tasks/bug` | Create a bug report |
| POST | `/api/tasks/feature` | Create a feature request |
| PUT | `/api/tasks/{id}/complete` | Complete a task (fires event) |

### Sample Request – Create Bug

```json
POST /api/tasks/bug
{
  "title": "Null pointer on dashboard load",
  "severityLevel": 2
}
```

SeverityLevel values: `0=Low, 1=Medium, 2=High, 3=Critical`

### Sample Request – Create Feature

```json
POST /api/tasks/feature
{
  "title": "Export to PDF",
  "estimatedHours": 12
}
```

---

## Key C# Features Used

- **Abstract classes** with `init`-only properties (encapsulation)
- **Delegates & Events** (`TaskCompletedEventHandler`, `OnTaskCompleted`)
- **LINQ** (`OfType<T>`, `Where`, `OrderByDescending`, `Sum`)
- **Records** for immutable DTOs
- **Pattern Matching** (`switch` expression in `TaskMapper`)
- **Dependency Injection** (`ITaskRepository` in controller)
- **Multi-stage Docker build** (SDK → runtime image)

---

## Block 3 – Architecture

See [`ARCHITECTURE.md`](ARCHITECTURE.md) for the full analysis of why **RabbitMQ + MassTransit** (async) is preferred over synchronous HTTP/REST for the NotificationService integration.
# ASP.NET-Midterm-assignment
