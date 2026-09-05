# 💰 MoneyFlow – Online Banking Management System

MoneyFlow is a secure and modern **Online Banking Management System** built with **ASP.NET Core MVC, Entity Framework Core, and SQL Server**. The system provides separate experiences for customers and bank employees, with role-based access, account management, transfers, cash operations, transaction tracking, customer management, and data-driven dashboards.

## ✨ Features

### 🔐 Authentication & Authorization
- Customer and Employee authentication
- Role-based access control using ASP.NET Core Identity
- Secure password-based login
- Account registration for customers
- Google external authentication
- Login lockout support

### 👤 Customer Portal
- Customer dashboard with account overview
- View active accounts and balances
- View account details
- Open/manage accounts
- Update personal information
- Customer profile and settings
- View transaction history
- Search/filter transactions
- View notifications

### 🏦 Account Management
- Support for Checking/Current and Savings accounts
- Account status management
- Account balance tracking
- Employee account/customer operations
- Masked account numbers in customer-facing views

### 💸 Money Transfers
- Transfer funds between accounts
- Account/balance validation
- Transaction status tracking
- Automatic transaction records
- Incoming and outgoing transfer identification

### 💵 Deposit & Withdrawal Operations
- Employee-controlled cash deposits
- Employee-controlled cash withdrawals
- Automatic account balance updates
- Transaction status handling
- Employee dashboard cash-operation tracking
- Recent cash operations limited to Deposit and Withdrawal transactions

### 📜 Transaction Management
- Transaction history for customers and employees
- Transaction types including:
  - Deposit
  - Withdrawal
  - Transfer
- Transaction status tracking
- Transaction numbers and timestamps
- Transaction filtering and search
- Soft-delete aware data access

### 👥 Customer Management
- Employee customer overview
- View and search customers
- Manage customer information
- Customer account overview
- Employee customer operations

### 📊 Data-Driven Dashboards
#### Customer Dashboard
- Total balance
- Active account count
- Recent transaction count
- Recently opened active accounts
- Recent transactions

#### Employee Dashboard
- Total customers
- Total accounts
- Today's deposit count
- Customers served today
- Today's cash operations
- Recent desk cash operations
- Recent activity feed

The Employee Dashboard retrieves its information from the database rather than using hard-coded/mock dashboard data.

### 🔔 Notifications
- Customer notification center
- Persistent notification records
- Notification status tracking
- Sent/failed notification state management

### 🛡️ Security & Data Integrity
- ASP.NET Core Identity
- Role-based authorization
- Input/model validation
- Transaction status management
- Audit fields for entities
- Soft-delete support
- Repository and service-based architecture
- Controllers communicate with the Business layer rather than directly accessing repositories

---

## 🏗️ Architecture

MoneyFlow follows a layered architecture:

```text
MoneyFlow
│
├── MoneyFlow.Data
│   ├── Entities
│   ├── Enums
│   ├── Database
│   ├── Repositories
│   └── Migrations
│
├── MoneyFlow.Business
│   ├── Services
│   ├── Service Interfaces
│   ├── ViewModels
│   ├── Helpers
│   └── Common
│
└── MoneyFlow.Presentation
    ├── Controllers
    ├── Views
    ├── Shared Layouts
    └── Configuration
```

### Data Layer
Responsible for:
- Entity Framework Core
- SQL Server database access
- Entities and relationships
- Repository implementations
- Database migrations

### Business Layer
Responsible for:
- Business logic
- Services
- ViewModels
- Validation and application rules
- Dashboard data preparation

### Presentation Layer
Responsible for:
- ASP.NET Core MVC controllers
- Razor views
- Customer and Employee interfaces
- Authentication UI
- Shared layouts and navigation

---

## 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| **ASP.NET Core MVC** | Web application framework |
| **.NET 10** | Application runtime and SDK |
| **Entity Framework Core 10** | ORM and database access |
| **SQL Server** | Relational database |
| **ASP.NET Core Identity** | Authentication and authorization |
| **Google Authentication** | External login |
| **Razor Views** | Server-side UI |
| **Bootstrap** | Responsive UI |
| **HTML5** | Page structure |
| **CSS3** | Styling |
| **JavaScript** | Client-side interactions |
| **LINQ** | Data querying |

---

## 🗄️ Database

The project uses **SQL Server** with Entity Framework Core migrations.

Main domain entities include:

- `ApplicationUser`
- `Customer`
- `Employee`
- `Account`
- `Transaction`
- `EmailNotification`

The project also contains an Entity Relationship Diagram:

**`ERD.jpeg`**

---

## 🚀 Getting Started

### Prerequisites

Make sure the following are installed:

- .NET 10 SDK
- SQL Server
- Git
- Visual Studio 2022/Visual Studio Code or another compatible .NET IDE

### 1. Clone the Repository

```bash
git clone https://github.com/RanaTarekAhmed/MoneyFlow-BankingSystem.git
cd MoneyFlow-BankingSystem
```

### 2. Configure SQL Server

Update the `DefaultConnection` connection string in:

```text
MoneyFlow/MoneyFlow.Presentation/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MoneyFlow;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply Database Migrations

From the project directory, run the EF Core migrations against the database.

```bash
dotnet ef database update
```

If the EF CLI tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

Run the Presentation project:

```bash
dotnet run --project MoneyFlow/MoneyFlow.Presentation/MoneyFlow.Presentation.csproj
```

Then open the local URL displayed by ASP.NET Core in the terminal.

---

## 🔑 User Roles

MoneyFlow supports role-based access for:

### Customer
Customers can:
- Access their dashboard
- Manage/view accounts
- Transfer money
- View transaction history
- Update profile information
- View notifications

### Employee
Employees can:
- Access the Employee Dashboard
- Manage customers
- Perform deposits
- Perform withdrawals
- View customer information
- Monitor recent banking activity
- Review cash operations

---

## 📁 Project Structure

```text
MoneyFlow/
│
├── MoneyFlow.Business/
│   ├── Common/
│   ├── Helpers/
│   ├── Services/
│   └── ViewModels/
│
├── MoneyFlow.Data/
│   ├── Database/
│   ├── Entities/
│   ├── Enums/
│   ├── Migrations/
│   └── Repositories/
│
└── MoneyFlow.Presentation/
    ├── Controllers/
    ├── Views/
    ├── Properties/
    ├── appsettings.json
    └── Program.cs
```

---

## 📈 Dashboard Data Rules

The Employee Dashboard uses database-backed information for its statistics and activity sections.

- **Today's Deposits Count** counts completed Deposit transactions only.
- **Recent Desk Cash Operations** displays the latest 5 Deposit and Withdrawal operations.
- **Transfers are excluded** from the desk cash operations list.
- **Recent Activity Feed** remains independent and displays the latest 5 activities across transaction types.
- Dashboard data is retrieved through the Business/Service layer.

---

## 🔮 Future Enhancements

Potential future improvements include:

- 🔑 OTP verification for money transfers
- 💳 Bill payment services
- 🏦 Loan management
- 💳 Credit/Debit card management
- 📈 Advanced spending analytics
- 📄 PDF account statements
- 📱 Responsive/mobile-focused improvements
- 🔔 More advanced real-time notifications
- 📊 Advanced banking reports and analytics

---

## 🎯 Project Goal

MoneyFlow aims to provide a centralized and user-friendly banking platform that allows customers to securely manage their accounts and transactions while giving bank employees the tools they need to manage customers and perform day-to-day banking operations efficiently.

---

