# Work Flow Application

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Technologies](#technologies)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
  - [Category Management](#category-management)
  - [Subcategory Management](#subcategory-management)
  - [User Management](#user-management)
  - [Requisition Management](#requisition-management)
- [Database Setup](#database-setup)

## Introduction
The Work Flow Application is a robust .NET-based platform designed to manage categories, subcategories, users, and requisitions. It is built using ASP.NET Core MVC and integrates with SQL Server for data management.

## Features
- **User Management:** Create, edit, delete users, and manage roles.
- **Category Management:** Add, edit, delete, and bulk import/export categories.
- **Subcategory Management:** Add, edit, delete, and bulk import/export subcategories.
- **Requisition Management:** Create, track, and approve requisitions with support for supplements(add supplements file) and clearance levels.

## Technologies
- **Frontend:** ASP.NET Core MVC, Razor Pages (for Identity)
- **Backend:** ASP.NET Core (MVC Pattern)
- **Database:** SQL Server 2022
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity with Razor Pages
- **Excel Integration:** ClosedXML for bulk import/export of categories and subcategories

## Requirements
- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio](https://visualstudio.microsoft.com/) (optional but recommended)

## Configuration

### appsettings.json
Before running the application, ensure that you add `WorkFlowWeb/appsettings.json` and ensure file is configured correctly. Below is an example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;"
  },
  "EmailSettings": {
  "SenderEmail": "Your Email ID",
  "SenderName": "Your Name",
  "SenderPassword": "Your Password",
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 465
   },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

## Installation

1. **Clone the Repository:**
    ```bash
    git clone https://github.com/yourusername/work-flow.git
    cd work-flow
    ```

2. **Set Up the Database:**
   - Update the `appsettings.json` file with your SQL Server connection string.
   
   - Run the following commands to create and seed the database:
     ```bash
     dotnet ef database update
     ```

3. **Run the Application:**
    ```bash
    dotnet run
    ```

4. **Open in Browser:**
    - Navigate to `https://localhost:5001` (or `http://localhost:5000` for HTTP) in your web browser.
    - Navigate to `https://localhost:7092` (for HTTPS) in your web browser.

## Usage

1. ### User Management:
  - **Description:** Manage application users and roles.

  - **Features:**
    - Create new user accounts.
    - Edit existing user information.
    - Delete users.
    - Assign roles to users.
    - Manage user-specific attributes like `ClearanceLevel`.


2. ### Category Management:
    - **Description:** Manage categories with full CRUD operations and bulk import/export functionality using ClosedXML.

    - **Features:**
      - Create new categories.
      - Edit and delete existing categories.
      - Import categories from Excel files.
      - Export categories to Excel files.
      - Can Block/UnBlock Category

3. ### Subcategory Management:
    - **Description:** Similar to category management, with full CRUD operations and bulk import/export functionality.

    - **Features:**
      - Create, edit, delete subcategories.
      - Only those sub category are created whose category is activated
      - Import subcategories from Excel files.
      - Export subcategories to Excel files.
      - Can Block/UnBlock Sub Category

4. ### Requisition Management:
  - **Description:** Manage requisitions with features like clearance level-based routing and supplement handling.

  - **Features:**
    - Create and submit requisitions.
    - can attach file
    - Track requisition status.
    - Manage requisition supplements.
    - Approve or reject requisitions based on clearance   levels.
    - Route requisitions according to user clearance levels.

## Database Setup

1. **Migrations:**
   - To add new migrations, use the following command:
     ```bash
     dotnet ef migrations add MigrationName
     ```

2. **Update Database:**
   - Apply migrations to the database with:
     ```bash
     dotnet ef database update
     ```







