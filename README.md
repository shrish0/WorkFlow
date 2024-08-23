# Work Flow Application

## Overview

The Work Flow application is a .NET ASP MVC web application designed to manage various aspects of an organization's workflow. This includes managing categories and subcategories, user roles and profiles, and requisition processes. The application is equipped with identity management, clearance levels, and approval routing functionalities, along with bulk data import/export capabilities.

## Features

### 1. Category Management
- **List Categories**:
  - Displays a list of all categories.
  - Includes details such as category code, title, description, created by, created at, modified at, and active status.
  - Allows filtering by active/inactive status.

- **Create Category**:
  - Admin users can create new categories.
  - Requires category code, title, and description.
  - Automatically sets the `CreatedBy` and `CreatedAt` fields.

- **Edit Category**:
  - Admin users can edit existing categories.
  - Automatically updates the `ModifiedAt` field when changes are saved.
  - Active status can be toggled.

- **Delete Category**:
  - Admin users can delete categories.
  - Provides an option to soft delete or permanently delete based on business rules.

- **Bulk Import/Export**:
  - Categories can be imported from or exported to Excel files using ClosedXML.
  - Includes data validation during import.

### 2. SubCategory Management
- **List SubCategories**:
  - Displays a list of all subcategories associated with active categories.
  - Includes details like category code, subcategory code, description, created by, created at, modified at, and active status.

- **Create SubCategory**:
  - Admin users can create new subcategories under active categories.
  - Requires subcategory code, title, and description.
  - Automatically sets the `CreatedBy` and `CreatedAt` fields.

- **Edit SubCategory**:
  - Admin users can edit existing subcategories.
  - Automatically updates the `ModifiedAt` field when changes are saved.
  - Active status can be toggled.

- **Delete SubCategory**:
  - Admin users can delete subcategories.
  - Provides an option to soft delete or permanently delete based on business rules.

- **Bulk Import/Export**:
  - Subcategories can be imported from or exported to Excel files using ClosedXML.
  - Includes data validation during import.

### 3. User Management
- **List Users**:
  - Displays a list of all users, including their roles, clearance levels, and active status.
  - Provides search functionality for quick access to specific users.
  
- **Create User**:
  - Admin users can create new users with roles and clearance levels.
  - Automatically assigns a unique `ApplicationUserId` (e.g., `u000000001`).
  - Configurable password and email confirmation.

- **Edit User**:
  - Admin users can edit user details, including roles and clearance levels.
  - Automatically updates the `ModifiedAt` field when changes are saved.

- **Delete User**:
  - Admin users can delete users.
  - Provides options for soft deletion or permanent deletion.

- **Role Management**:
  - Admin users can manage roles assigned to users.
  - Roles are tied to clearance levels, controlling access to different parts of the application.

- **Email Confirmation**:
  - Users receive email confirmations for account creation and important updates.

### 4. Requisition Management
- **List Requisitions**:
  - Displays a list of all requisitions with details such as requisition ID, title, category, subcategory, created by, status, and approval stages.
  - Requisitions can be filtered based on status, approval stages, and categories.

- **Create Requisition**:
  - Users can create new requisitions.
  - Automatically generates a unique `RequisitionId` in the format `REQYYYYNNNNNN`.
  - Validates required fields and associated categories/subcategories.

- **Edit Requisition**:
  - Users can edit requisitions they have created.
  - Updates the `ModifiedAt` field automatically.
  - Provides functionality to update requisition status and add comments.

- **Approve/Reassign Requisition**:
  - Users with the appropriate clearance level can approve, reject, or reassign requisitions.
  - Approval flow is managed based on clearance levels and user roles.

- **Requisition Supplement Handling**:
  - Allows adding multiple supplements to a requisition after it has been created.
  - Supplements can be uploaded one at a time with the option to add more.
  - Files are handled separately from the requisition creation process.

- **Requisition Approval Filtering**:
  - Users can filter requisitions based on whether they are the 'Sent To' or 'Sent By' user.
  - Provides an interface for users in the 'Sent To' field to edit details in the approval comment and action.

## Technology Stack
- **Backend**: ASP.NET Core MVC
- **Frontend**: Razor Pages, Bootstrap, jQuery
- **Database**: SQL Server 2022
- **Identity Management**: ASP.NET Core Identity
- **File Handling**: ClosedXML for Excel import/export, ASP.NET Core File Upload for supplements
- **Logging**: Serilog (with Message Diagnostic Context - MDC)
- **Containerization**: Dockerfile for building Docker images

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Node.js](https://nodejs.org/en/download/) (for frontend dependencies)

### Installation
1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/workflow.git
   cd workflow
   ```

2. **Setup the database**:
   - Update the connection string in `appsettings.json`.
   - Run the database migrations:
     ```bash
     dotnet ef database update
     ```

3. **Build the application**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the application**:
   Open your browser and navigate to `http://localhost:5000`.

## Contributing

Contributions are welcome! Please read the [contributing guidelines](CONTRIBUTING.md) for more information.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.md) file for details.
```

This `README.md` provides an overview of your Work Flow application, detailing all key features, how to get started, and other relevant information for users and developers.