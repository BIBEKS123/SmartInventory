# SmartInventory – Smart Product Inventory & Chatbot

A web-based Product Inventory Management System developed using **ASP.NET Core MVC (.NET 8), C#, Entity Framework Core, and SQL Server**. The application helps manage products, monitor stock levels, and answer inventory-related queries through an integrated chatbot.

## 🚀 Features

- **Admin Dashboard** – Overview of inventory information.
- **User Authentication** – Login and Registration.
- **Product Management** – Add, View, Update, and Delete products.
- **Inventory Tracking** – Monitor product prices and stock quantities.
- **Inventory Chatbot** – Ask questions about total products, low-stock items, and individual product stock.
- **Low Stock Detection** – Identify products with stock quantity of 5 or less.
- **REST API** – API endpoints for product and chatbot operations.
- **Database Integration** – SQL Server with Entity Framework Core.

## 📸 Application Screenshots

### Admin Dashboard
![SmartInventory Admin Dashboard](screenshots/admin-dashboard.png)

### Inventory Statistics
![SmartInventory Inventory Statistics](screenshots/inventory-statistics.png)

### Product Management
![SmartInventory Product Management](screenshots/product-management.png)

### Low Stock Alert
![SmartInventory Low Stock Alert](screenshots/low-stock-alert.png)

### AI Inventory Assistant
![SmartInventory AI Inventory Assistant](screenshots/ai-inventory-assistant.png)

## 🛠️ Technologies Used

| Technology | Purpose |
|---|---|
| C# | Backend Programming |
| ASP.NET Core MVC (.NET 8) | Web Application |
| Entity Framework Core | ORM / Database Operations |
| SQL Server | Database |
| HTML, CSS, JavaScript | Frontend |
| Bootstrap | UI Styling |
| REST API | API Integration |
| Git & GitHub | Version Control |

## 📂 Project Structure

```text
SmartInventory/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── ChatbotController.cs
│   ├── HomeController.cs
│   └── ProductsController.cs
│
├── Data/
│   ├── AppDbContext.cs
│   └── IdentitySeeder.cs
│
├── Migrations/
│
├── Models/
│   ├── Product.cs
│   └── Sale.cs
│
├── Views/
│
├── wwwroot/
│
├── appsettings.json
├── Program.cs
└── SmartInventory.csproj
```

## ⚙️ Installation & Setup

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio (SSMS)

### Step 1: Clone Repository

```bash
git clone https://github.com/BIBEKS123/SmartInventory.git
```

### Step 2: Open Project

Open `SmartInventory.sln` in Visual Studio.

### Step 3: Restore Dependencies

```bash
dotnet restore
```

### Step 4: Configure Database

Configure your SQL Server connection string using ASP.NET Core User Secrets.

```bash
dotnet user-secrets init
```

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_SQL_SERVER_CONNECTION_STRING"
```

Replace the placeholder with your own SQL Server connection string.

### Step 5: Apply Database Migrations

Open Visual Studio Package Manager Console and run:

```powershell
Update-Database
```

### Step 6: Run Application

```bash
dotnet run
```

Open the local URL displayed in the terminal.

## 💬 Chatbot Examples

Try asking:

- How many products are available?
- Show low stock products.
- Check iPhone stock.

## 🎯 Project Objective

The main objective of SmartInventory is to simplify product inventory management, improve stock monitoring, and provide quick inventory information through a chatbot interface.

## 👨‍💻 Developer

**Bibek Senapati**

B.Tech | Software Development

**GitHub:** https://github.com/BIBEKS123

---

⭐ If you find this project interesting, feel free to explore the source code and share your feedback.
