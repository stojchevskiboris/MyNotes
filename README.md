# MyNotes Application

MyNotes is a full-stack web application designed for creating, managing, and organizing digital sticky notes. It features a dynamic, draggable workspace, user authentication, and customizable themes.

## Features

- **Draggable Notes:** Freely move and resize notes on a virtual canvas.
- **Layering (Z-Index):** Recently interacted notes or new notes appear on top of others.
- **Custom Colors:** Assign unique colors to each note for better organization.
- **Search & Filter:** Quickly find notes by title, content, or tags.
- **Pinning & Archiving:** Keep important notes on top or archive completed ones.
- **User Authentication:** Secure login and registration, including Google Sign-In support.
- **User Settings:** Customize your profile (Name, Email) and choose between Light and Dark themes.
- **Modern UI:** Responsive design using Bootstrap and Angular CDK.

## Architecture

- **Backend:** .NET 8 Web API
  - **Data Access:** Entity Framework Core with SQL Server.
  - **Patterns:** Repository and Service pattern for clean separation of concerns.
  - **Security:** JWT-based authentication.
- **Frontend:** Angular 18
  - **Components:** Modular structure with dedicated components for Notes, Login, and Registration.
  - **Services:** Centralized data and authentication services.
  - **Interceptors:** Automatic JWT attachment to outgoing requests.

## Prerequisites

- **.NET SDK 8.0+**
- **Node.js 18+**
- **Angular CLI** (`npm install -g @angular/cli`)
- **SQL Server** (LocalDB or a full instance)

## Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd MyNotes
```

### 2. Backend Setup
1. Navigate to the server directory:
   ```bash
   cd MyNotes.Server
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Update the database connection string in `appsettings.json` if necessary.
4. Apply migrations:
   ```bash
   dotnet ef database update
   ```
5. Run the API:
   ```bash
   dotnet run
   ```

### 3. Frontend Setup
1. Navigate to the client directory:
   ```bash
   cd ../mynotes.client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   npm start
   ```

The application should now be running. The frontend typically serves on `https://localhost:4200` (or as configured in proxy), and the backend on `https://localhost:7078`.

## Development

- **Migrations:** When changing entities, use `dotnet ef migrations add <MigrationName>`.
- **Styling:** Global styles are in `styles.css`, and component-specific styles are in their respective `.css` files.
- **Environment:** API URL is configured in `mynotes.client/src/environments/environment.ts`.
