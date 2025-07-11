# 📋 Task Management System

A **Task Management System** built with **C# .NET 6+**. This system allows users to create, assign, track, and manage tasks efficiently with proper validation, logging, and business rules.

## 🏗️ Architecture Overview

This project implements **Clean Architecture** with separation of concerns across multiple layers:

```
📁 TaskManagement.Api          → Presentation Layer (REST API)
📁 TaskManagement.Application  → Application Layer (Business Logic)
📁 TaskManagement.Domain       → Domain Layer (Core Business)
📁 TaskManagement.Infrastructure → Infrastructure Layer (Data Access)
📁 TaskManagement.UnitTests    → Unit Testing
📁 TaskManagement.IntegrationTests → Integration Testing
```

## 🎯 SOLID Principles Implementation

### **S** - Single Responsibility Principle (SRP)
- Each class has a single, well-defined responsibility
- `TaskService` handles only task-related business logic
- `UserService` handles only user-related business logic
- Controllers act only as orchestrators

### **O** - Open/Closed Principle (OCP)
- System is open for extension but closed for modification
- Repository pattern allows easy extension of data access methods
- Service interfaces allow easy addition of new business features
- BaseEntity provides extensible audit functionality

### **L** - Liskov Substitution Principle (LSP)
- All entities properly inherit from BaseEntity
- Repository implementations are fully substitutable with their interfaces
- Service implementations fully conform to their interface contracts

### **I** - Interface Segregation Principle (ISP)
- Specific repository interfaces (`ITaskRepository`, `IUserRepository`)
- Focused service interfaces without unnecessary methods
- Clients depend only on methods they actually use

### **D** - Dependency Inversion Principle (DIP)
- High-level modules (Services) don't depend on low-level modules (Repositories)
- Both depend on abstractions (Interfaces)
- Dependency injection throughout the application

## 🚀 Core Features

### ✅ Task Management
- **Create** new tasks with validation
- **Update** task details (title, description, due date, priority)
- **Update** task status (Pending → InProgress → Completed)
- **Assign/Unassign** tasks to users
- **Delete** tasks (with business rules)
- **Retrieve** tasks with filtering options

### ✅ User Management  
- **Create** new users with email validation
- **Retrieve** user information
- **Delete** users (with constraint validation)
- **Search** users by email

### ✅ Advanced Filtering
- Get tasks by **user assignment**
- Get tasks by **status**
- Get tasks by **priority**
- Get **overdue tasks**

### ✅ Business Rules & Validation
- Due dates cannot be in the past
- Email addresses must be unique
- Tasks in progress cannot be deleted
- Users with assigned tasks cannot be deleted
- Automatic overdue task detection

## 🛠️ Technology Stack

### Backend Framework
- **.NET 6+** - Modern, cross-platform framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for data access
- **SQLite** - Lightweight database for development/testing

### Logging & Monitoring
- **Serilog** - Structured logging framework
- **Console & File logging** with rotation
- **Request/Response logging** with enrichment

### Testing
- **xUnit** - Unit testing framework
- **Integration Tests** - End-to-end API testing
- **Repository Pattern** - Enables easy mocking

### Documentation
- **Swagger/OpenAPI** - Interactive API documentation
- **XML Comments** - Inline code documentation

## 📊 Database Schema & ERD

![Screenshot 2025-06-29 103602](https://github.com/user-attachments/assets/4b45007d-c69e-4575-8a79-0f4216229827)

### Database Tables

#### Users Table
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| Name | nvarchar(100) | NOT NULL | User full name |
| Email | nvarchar(200) | NOT NULL, Unique | User email address |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |
| CreatedBy | nvarchar(100) | NOT NULL | Creator identifier |
| UpdatedBy | nvarchar(100) | NULL | Last updater identifier |

#### Tasks Table
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| Title | nvarchar(200) | NOT NULL | Task title |
| Description | nvarchar(1000) | NULL | Task description |
| DueDate | datetime2 | NULL | Task due date |
| Priority | int | NOT NULL | Priority level (1-4) |
| Status | int | NOT NULL | Task status (1-4) |
| AssignedUserId | int | FK, NULL | Foreign key to Users |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |
| CreatedBy | nvarchar(100) | NOT NULL | Creator identifier |
| UpdatedBy | nvarchar(100) | NULL | Last updater identifier |

### Enumerations

#### TaskStatus
- `1` - **Pending** - Task is created but not started
- `2` - **InProgress** - Task is currently being worked on
- `3` - **Completed** - Task is finished
- `4` - **Cancelled** - Task is cancelled

#### TaskPriority  
- `1` - **Low** - Low priority task
- `2` - **Medium** - Medium priority task
- `3` - **High** - High priority task
- `4` - **Critical** - Critical priority task

### Relationships
- **Users** ↔ **Tasks**: One-to-Many relationship
- A user can have multiple tasks assigned
- A task can be assigned to only one user (or none)
- Foreign key constraint with `ON DELETE SET NULL`

### Indexes
- `Tasks.Status` - For filtering by status
- `Tasks.Priority` - For filtering by priority  
- `Tasks.DueDate` - For overdue task queries
- `Tasks.AssignedUserId` - For user task queries
- `Users.Email` - Unique constraint for email lookup

## 🌐 API Endpoints

### Tasks Controller (`/api/tasks`)

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| GET | `/api/tasks` | Get all tasks | - |
| GET | `/api/tasks/{id}` | Get task by ID | - |
| POST | `/api/tasks` | Create new task | `CreateTaskDto` |
| PUT | `/api/tasks/{id}` | Update task | `UpdateTaskDto` |
| PATCH | `/api/tasks/{id}/status` | Update task status | `UpdateTaskStatusDto` |
| PATCH | `/api/tasks/{id}/assign` | Assign task to user | `AssignTaskDto` |
| PATCH | `/api/tasks/{id}/unassign` | Unassign task | - |
| DELETE | `/api/tasks/{id}` | Delete task | - |
| GET | `/api/tasks/user/{userId}` | Get tasks by user | - |
| GET | `/api/tasks/status/{status}` | Get tasks by status | - |
| GET | `/api/tasks/overdue` | Get overdue tasks | - |

### Users Controller (`/api/users`)

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| GET | `/api/users` | Get all users | - |
| GET | `/api/users/{id}` | Get user by ID | - |
| POST | `/api/users` | Create new user | `CreateUserDto` |
| DELETE | `/api/users/{id}` | Delete user | - |
| GET | `/api/users/email/{email}` | Get user by email | - |

### Sample Request/Response

#### Create Task Request
```json
POST /api/tasks
{
  "title": "Implement user authentication",
  "description": "Add JWT-based authentication to the API",
  "dueDate": "2024-07-01T10:00:00Z",
  "priority": 3,
  "assignedUserId": 1
}
```

#### Create Task Response
```json
{
  "id": 1,
  "title": "Implement user authentication",
  "description": "Add JWT-based authentication to the API",
  "dueDate": "2024-07-01T10:00:00Z",
  "priority": 3,
  "status": 1,
  "assignedUserId": 1,
  "assignedUserName": "John Doe",
  "createdAt": "2024-06-29T10:30:00Z",
  "updatedAt": null,
  "createdBy": "api-user",
  "updatedBy": null,
  "isOverdue": false,
  "canBeDeleted": true
}
```

## 🚀 Getting Started

### Prerequisites
- **.NET 6 SDK** or later
- **Visual Studio 2022**
- **Git** for version control

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TaskManagement
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Set up the database**
   ```bash
   cd TaskManagement.Api
   dotnet ef database update
   ```
   *Note: Database will be automatically created with seed data*

4. **Run the application**
   
   **Backend API (Main Focus):**
   ```bash
   dotnet run --project TaskManagement.Api
   ```
   
   **Web UI**
   ```bash
   dotnet run --project TaskManagement.Web
   ```

5. **Access the application**
   - **API**: `https://localhost:7102` or `http://localhost:5102`
   - **Swagger UI**: `https://localhost:7102` (Development only)
   - **Web UI**: `https://localhost:7007` (Demo dashboard)

## 🧪 Testing

### Running Unit Tests
```bash
dotnet test TaskManagement.UnitTests
```

### Running Integration Tests
```bash
dotnet test TaskManagement.IntegrationTests
```
*Note: Integration tests are planned for future implementation*

### Running All Tests
```bash
dotnet test
```

### Test Coverage
The test suite covers:
- ✅ **Domain Logic** - Business rules and validation
- ✅ **Service Layer** - Application business logic
- ✅ **API Controllers** - HTTP endpoints and responses
- ✅ **Repository Pattern** - Data access operations
- 🔄 **Integration Tests** - End-to-end API workflows (Planned)

## 📁 Project Structure

```
TaskManagement/
├── 📁 TaskManagement.Api/              # 🎯 Presentation Layer
│   ├── Controllers/                    # API Controllers
│   │   ├── BaseApiController.cs        # Base controller with common functionality
│   │   ├── TasksController.cs          # Task management endpoints
│   │   └── UsersController.cs          # User management endpoints
│   ├── logs/                          # Application logs
│   ├── appsettings.json               # Configuration
│   ├── Program.cs                     # Application entry point
│   └── taskmanagement.db              # SQLite database file
│
├── 📁 TaskManagement.Application/      # 💼 Application Layer  
│   ├── Common/                        # Common utilities
│   │   └── ServiceResult.cs           # Result pattern implementation
│   ├── DTOs/                          # Data Transfer Objects
│   │   ├── AssignTaskDto.cs           # Task assignment data
│   │   ├── CreateTaskDto.cs           # Task creation data
│   │   ├── CreateUserDto.cs           # User creation data
│   │   ├── TaskResponseDto.cs         # Task response data
│   │   ├── UpdateTaskDto.cs           # Task update data
│   │   ├── UpdateTaskStatusDto.cs     # Status update data
│   │   └── UserResponseDto.cs         # User response data
│   ├── Interfaces/                    # Service contracts
│   │   ├── ITaskService.cs            # Task service interface
│   │   └── IUserService.cs            # User service interface
│   └── Services/                      # Business logic implementations
│       ├── TaskService.cs             # Task business logic
│       └── UserService.cs             # User business logic
│
├── 📁 TaskManagement.Domain/           # 🏛️ Domain Layer
│   ├── Entities/                      # Domain entities
│   │   ├── BaseEntity.cs              # Base entity with audit fields
│   │   ├── TaskItem.cs                # Task domain entity
│   │   └── User.cs                    # User domain entity
│   ├── Enums/                         # Domain enumerations
│   │   ├── TaskPriority.cs            # Priority levels
│   │   └── TaskStatus.cs              # Task states
│   └── Interfaces/                    # Domain contracts
│       ├── IRepository.cs             # Generic repository interface
│       ├── ITaskRepository.cs         # Task repository interface
│       ├── IUnitOfWork.cs            # Unit of work interface
│       └── IUserRepository.cs         # User repository interface
│
├── 📁 TaskManagement.Infrastructure/   # 🔧 Infrastructure Layer
│   ├── Configurations/                # EF Core configurations
│   │   ├── TaskItemConfiguration.cs   # Task entity configuration
│   │   └── UserConfiguration.cs       # User entity configuration
│   ├── Data/                          # Data access implementations
│   │   ├── TaskManagementDbContext.cs # Database context
│   │   └── UnitOfWork.cs              # Unit of work implementation
│   ├── Repositories/                  # Repository implementations
│   │   ├── Repository.cs              # Generic repository base
│   │   ├── TaskRepository.cs          # Task repository implementation
│   │   └── UserRepository.cs          # User repository implementation
│   └── DependencyInjection.cs         # Service registration
│
├── 📁 TaskManagement.Web/              # 🌐 Web UI Layer (Optional)
│   ├── Components/                     # Razor components
│   │   ├── Layout/                     # Layout components
│   │   └── Modals/                     # Modal components
│   │       ├── TaskAssignModal.razor   # Task assignment modal
│   │       ├── TaskCreateModal.razor   # Task creation modal
│   │       ├── TaskEditModal.razor     # Task editing modal
│   │       ├── TaskStatusModal.razor   # Status update modal
│   │       └── UserCreateModal.razor   # User creation modal
│   ├── Pages/                          # Razor pages
│   │   ├── Error.razor                 # Error page
│   │   ├── Home.razor                  # Dashboard page
│   │   ├── Overdue.razor              # Overdue tasks page
│   │   ├── Tasks.razor                # Tasks management page
│   │   └── Users.razor                # Users management page
│   ├── Services/                       # Frontend services
│   │   ├── TaskApiService.cs          # API client for tasks
│   │   └── UserApiService.cs          # API client for users
│   ├── wwwroot/                       # Static files
│   ├── _Imports.razor                 # Global imports
│   ├── App.razor                      # Root component
│   ├── Routes.razor                   # Routing configuration
│   ├── appsettings.json              # Web configuration
│   └── Program.cs                     # Web application entry point
│
├── 📁 TaskManagement.UnitTests/        # 🧪 Unit Tests
└── 📁 TaskManagement.IntegrationTests/ # 🔄 Integration Tests (Future)
```

## 📝 Key Design Patterns

### 1. **Repository Pattern**
- Abstracts data access logic
- Enables easy testing with mock repositories
- Provides consistent interface for data operations

### 2. **Unit of Work Pattern**
- Manages transactions across multiple repositories
- Ensures data consistency
- Centralizes database context management

### 3. **Factory Pattern**
- `TaskItem.Create()` factory method for task creation
- Ensures proper initialization and validation
- Encapsulates complex object creation logic

### 4. **Result Pattern**
- `ServiceResult<T>` for consistent error handling
- Eliminates exception-driven control flow
- Provides structured success/failure responses

### 5. **Dependency Injection**
- Constructor injection throughout the application
- Loose coupling between components
- Easy testing and component substitution

## 🔒 Security Considerations

### Input Validation
- ✅ **Data Annotations** - Automatic model validation
- ✅ **Domain Validation** - Business rule enforcement
- ✅ **SQL Injection Protection** - Entity Framework parameterization

---
