# 🚤 HouseBoat Booking Management System

An AI-powered HouseBoat Booking Management System built using **ASP.NET Core (.NET 10)**, following **Clean Architecture** and **Vertical Slice Architecture** principles.

The project combines modern backend engineering practices with AI integration to provide an intelligent booking assistant that can understand natural-language requests, check availability, maintain conversation context, and initiate bookings after user confirmation.

---

## ✨ Key Features

### 🔐 Authentication & Authorization
- Customer registration and login.
- JWT-based authentication.
- Role-based authorization (Customer and Admin).
- Secure password handling.

### 🚤 HouseBoat Management
- View available houseboats.
- Manage houseboat information.
- Houseboat capacity and pricing management.
- AC, bedroom and location details.
- Active/inactive houseboat management.

### 📅 Booking Management
- Create and manage bookings.
- Check houseboat availability.
- Prevent overlapping bookings.
- Validate guest capacity and booking dates.
- View personal booking history.
- Booking status management.

### 👨‍💼 Admin Management
- View all bookings.
- Retrieve individual booking details.
- Confirm bookings.
- Cancel bookings.
- Complete bookings.
- Pagination, filtering and sorting.
- Admin dashboard functionality.

### ⭐ Reviews & Notifications
- Customer review functionality.
- Notification management.

---

## 🤖 AI Integration

One of the key features of this project is its **AI-powered Booking Assistant**.

Instead of building a chatbot that only generates text, the assistant is connected to actual backend functionality.

### AI Capabilities
- Understand natural-language booking requests.
- Collect booking details conversationally.
- Check houseboat availability through backend functionality.
- Maintain conversation context across multiple messages.
- Ask users to confirm booking details.
- Create bookings after confirmation.
- Prevent duplicate bookings after successful completion.

### AI Providers

| Provider | Purpose |
|---|---|
| Ollama | Run Large Language Models locally |
| OpenAI | Alternative cloud-based AI provider |

### AI Conversation Flow

```text
User Message
     |
     v
AI Booking Assistant
     |
     v
Understand User Intent
     |
     v
Collect Booking Details
     |
     v
Check Availability
     |
     v
Present Booking Summary
     |
     v
Request User Confirmation
     |
     v
Create Booking
     |
     v
Booking Confirmation
```

### Conversation Persistence

AI conversation state is persisted using a `BookingConversation` entity.

The conversation state tracks information such as:

- Conversation ID
- Location
- HouseBoat name
- Check-in and check-out dates
- Guest count
- Booking confirmation status
- Booking completion status
- Created booking ID

This allows the assistant to maintain context across multiple messages instead of treating every request as a new conversation.

---

## 🏗️ Architecture

The backend follows:

- **Clean Architecture** – Separation of responsibilities and dependency boundaries.
- **Vertical Slice Architecture** – Organizing application code around individual business features.

### Solution Structure

```text
HouseBoat Management System
│
├── src
│   ├── HouseBoat.API
│   │   ├── Controllers
│   │   ├── Middleware
│   │   └── Program.cs
│   │
│   ├── HouseBoat.Application
│   │   ├── Features
│   │   │   ├── Auth
│   │   │   ├── HouseBoats
│   │   │   ├── Bookings
│   │   │   ├── Reviews
│   │   │   ├── Notifications
│   │   │   └── AI
│   │   ├── Common
│   │   └── Interfaces
│   │
│   ├── HouseBoat.Domain
│   │   ├── Entities
│   │   └── Enums
│   │
│   ├── HouseBoat.Infrastructure
│   │   ├── Authentication
│   │   ├── AI
│   │   └── Services
│   │
│   └── HouseBoat.Persistence
│       ├── Context
│       ├── Configurations
│       └── Migrations
│
└── tests
    ├── HouseBoat.Application.Tests
    └── HouseBoat.API.Tests
```

### Architectural Approach

**Clean Architecture**

Separates the system into distinct layers:

- API – Handles HTTP requests and responses.
- Application – Contains use cases and business workflows.
- Domain – Contains core business entities and rules.
- Infrastructure – Handles external services and integrations.
- Persistence – Handles database access through Entity Framework Core.

**Vertical Slice Architecture**

Features are organized by business capability rather than by technical type.

For example:

```text
Features/
   Bookings/
      Create/
      GetById/
      MyBookings/
      Cancel/
      Admin/
         GetAll/
         GetById/
         Confirm/
         Cancel/
         Complete/
```

---

## 🛠️ Technology Stack

| Category | Technology |
|---|---|
| Backend | ASP.NET Core Web API |
| Framework | .NET 10 |
| Architecture | Clean Architecture |
| Application Design | Vertical Slice Architecture |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | JWT |
| Validation | FluentValidation |
| Logging | Serilog |
| API Documentation | Swagger / OpenAPI |
| Local AI | Ollama |
| Alternative AI Provider | OpenAI |
| Unit Testing | xUnit, Moq |
| Integration Testing | xUnit, ASP.NET Core TestServer |
| Database Testing | EF Core InMemory |

---

## ⚙️ Getting Started

### Prerequisites

Install the following:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or Visual Studio Code
- Ollama (if using the local AI provider)
- Git

### 1. Clone the Repository

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
```

```bash
cd <YOUR_PROJECT_DIRECTORY>
```

### 2. Configure the Database

Update the connection string in the API configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HouseBoatBookingDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Configure JWT

Configure the JWT settings using your local configuration or secure environment variables.

```json
{
  "Jwt": {
    "Key": "<YOUR_SECRET_KEY>",
    "Issuer": "HouseBoat.API",
    "Audience": "HouseBoat.Client",
    "ExpiryMinutes": 30
  }
}
```

Do not commit production secrets or API keys to GitHub.

### 4. Apply Database Migrations

From the solution root:

```bash
dotnet ef database update --project src/HouseBoat.Persistence --startup-project src/HouseBoat.API
```

### 5. Run the API

```bash
dotnet run --project src/HouseBoat.API
```

Open Swagger to explore and test the available API endpoints.

---

## 🧪 Testing

The project includes application unit tests and API integration tests.

### Run Application Unit Tests

```bash
dotnet test tests/HouseBoat.Application.Tests/HouseBoat.Application.Tests.csproj
```

### Run API Integration Tests

```bash
dotnet test tests/HouseBoat.API.Tests/HouseBoat.API.Tests.csproj
```

### Current Testing Progress

- Application unit tests: 5 passing in the latest reported run.
- API integration tests: 16 passing in the latest reported run.

API integration coverage includes authentication, role-based authorization, booking creation, customer booking retrieval, booking ownership and admin booking workflows.

Testing is ongoing as additional scenarios are added.

---

## 🚀 Development Roadmap

- [x] Clean Architecture setup
- [x] Vertical Slice Architecture
- [x] JWT authentication and role-based authorization
- [x] HouseBoat management
- [x] Booking management
- [x] Admin booking workflow
- [x] AI booking assistant
- [x] Ollama integration
- [x] OpenAI provider support
- [x] Conversation state persistence
- [x] Initial unit testing
- [x] Initial API integration testing
- [ ] Complete backend integration test coverage
- [ ] Complete AI failure and concurrency testing
- [ ] Docker and Docker Compose
- [ ] GitHub Actions CI/CD
- [ ] Angular frontend
- [ ] End-to-end testing

---

## 🎯 Project Objective

This project is developed as a hands-on learning initiative to explore modern .NET application architecture, backend engineering practices, database management, automated testing and practical AI integration.

The primary goal is to build an application where AI is not limited to generating responses but is connected to real business workflows through controlled backend operations.

---

## 👨‍💻 Author

**Vijal B**

.NET Full Stack Developer | ASP.NET Core | Angular

