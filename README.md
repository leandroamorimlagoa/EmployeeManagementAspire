# Employee Management System

This project is a **Full-Stack Employee Management System** composed of:

- **Backend:** ASP.NET Core 9 Web API
- **Frontend:** Angular 19
- **Database:** PostgreSQL
- **Logging:** Seq (structured logging)
- **Database Admin Panel:** pgAdmin

The API includes authentication and authorization using **JWT**, and the frontend allows users to manage employees.

---

## 🚀 Getting Started

You can run the entire application using **Docker Compose** or **.NET Aspire**.

### **1️⃣ Running with Docker Compose**
> Requires **Docker** installed on your machine.

1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo-url.git
   cd EmployeeManagementAspire
   ```

2. Build and start the containers:
   ```sh
   docker compose up --build -d
   ```

3. Access the services:
   - **API:** [`http://localhost:5104`](http://localhost:5104) (HTTP) or [`https://localhost:7194`](https://localhost:7194) (HTTPS)
   - **Frontend (Angular):** [`http://localhost:4200`](http://localhost:4200) (if running manually)
   - **pgAdmin:** [`http://localhost:5050`](http://localhost:5050)  
     - Login: `admin@admin.com`  
     - Password: `admin`
   - **Seq (Logging UI):** [`http://localhost:5341`](http://localhost:5341)

---

### **2️⃣ Running with .NET Aspire**
> Requires **.NET 9 SDK** installed on your machine.

1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo-url.git
   cd EmployeeManagementAspire
   ```

2. Run the application using Aspire:
   ```sh
   dotnet run --project Aspire.AppHost
   ```

3. Access the services (same as above).

---

## **🔑 First User Login**
After running the API, the first admin user will be automatically created:

- **Email:** `admin@admin.com`
- **Password:** `Admin@123!`

---

## **📌 Project Structure**
The solution is structured as follows:

```
EmployeeManagementAspire/
│── src/
│   ├── Api/               # .NET Web API
│   ├── Application/       # Business logic
│   ├── Domain/            # Domain models
│   ├── Infrastructure/    # Database and Repositories
│── angular-employee-management/ # Angular 19 frontend
│── Aspire.AppHost/        # .NET Aspire service orchestration
│── docker-compose.yml     # Docker Compose configuration
│── README.md              # Project documentation
```

---

## **🛠 API Controllers**
The Web API exposes two main controllers:

### **🔐 AuthenticateController (`/api/authenticate`)**
Handles authentication and returns a **JWT Token**.

- **POST** `/api/authenticate`
  - **Request Body:**
    ```json
    {
      "email": "admin@admin.com",
      "password": "Admin@123!"
    }
    ```
  - **Response:**
    ```json
    {
      "token": "eyJhbGciOiJIUz..."
    }
    ```

- **GET** `/api/authenticate/info`
  - Returns information about the currently authenticated user.

### **👥 EmployeeController (`/api/employee`)**
Manages employee records.

- **GET** `/api/employee`
  - Fetches a paginated list of employees.
  
- **GET** `/api/employee/{id}`
  - Fetches a single employee by ID.

- **POST** `/api/employee`
  - Creates a new employee.

- **PUT** `/api/employee`
  - Updates an existing employee.

- **DELETE** `/api/employee/{id}`
  - Deletes an employee.

---

## **📜 Environment Variables**
The `.NET API` reads configuration values from **`appsettings.json`** and environment variables.

Example for **PostgreSQL connection:**
```json
"Aspire": {
  "Npgsql": {
    "connectionString": "Host=localhost;Port=5432;Database=CompanyDB;Username=postgres;Password=123456"
  }
}
```

For **JWT Authentication:**
```json
"Jwt": {
  "Key": "CHAVE_SECRETA_SUPER_SECRETA_LONGA_E_SEGURA_32_CHARSAKI"
}
```

---

## **🐳 Docker Containers**
| Service   | Description |
|-----------|------------|
| `postgres-db` | PostgreSQL database |
| `pgadmin` | PostgreSQL admin panel |
| `seq` | Logging with Seq |
| `api` | ASP.NET Core 9 Web API |

To stop the containers:
```sh
docker compose down
```

---

This `README.md` file provides clear **setup instructions**, **API documentation**, and **service descriptions**, making it easy for anyone to get started. 🚀 Let me know if you need any modifications!