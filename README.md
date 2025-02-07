Here’s a **`README.md`** for your **MiniFlexCRM** project, covering setup, architecture, technologies, and API usage.

---

### **📌 MiniFlexCRM**

🚀 **MiniFlexCRM** is a **multi-tenant, API-driven CRM** designed for **scalability, flexibility, and security**. It provides **RESTful APIs** for managing tenants, customers, companies, users, and relationships, with built-in authentication and role-based access control (RBAC).

---

## **📖 Table of Contents**

- [📌 MiniFlexCRM](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#-miniflexcrm)
- [⚡ Features](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#-features)
- [🛠️ Tech Stack](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#-tech-stack)
- [🚀 Getting Started](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#-getting-started)
    - [1️⃣ Prerequisites](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#1%EF%B8%8F%E2%83%A3-prerequisites)
    - [2️⃣ Setup Instructions](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#2%EF%B8%8F%E2%83%A3-setup-instructions)
- [🔐 Authentication & Authorization](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#-authentication--authorization)
- [📡 API Endpoints](https://chatgpt.com/c/67a4d306-4eb8-8005-a6e4-b62561697dd8#-api-endpoints)

---

## **⚡ Features**

✅ **Multi-Tenant Support** - Each tenant has isolated data.  
✅ **Role-Based Access Control (RBAC)** - Restrict access with roles.  
✅ **RESTful API Design** - Built with **ASP.NET Core 8**.  
✅ **JWT Authentication** - Secure API access using JWT tokens.  
✅ **Dapper ORM** - Lightweight and high-performance database interactions.  
✅ **PostgreSQL JSON Storage** - Store flexible attributes for customers and companies.  
✅ **Dockerized Deployment** - Fully containerized with **Docker Compose**.  
✅ **Cloud Ready** - Designed for **AWS/Azure/GCP** integration.

---

## **🛠️ Tech Stack**

|**Category**|**Technology**|
|---|---|
|**Backend**|.NET 8 (ASP.NET Core Web API)|
|**Frontend**|React.js|
|**Database**|PostgreSQL (JSON support)|
|**Authentication**|JWT (JSON Web Tokens)|
|**ORM**|Dapper|
|**Infrastructure**|Docker, Docker Compose|
|**Cloud Services**|AWS S3, Azure Blob Storage (optional)|

---

## **🚀 Getting Started**

### **1️⃣ Prerequisites**

- **Docker & Docker Compose** → Install from [Docker](https://www.docker.com/get-started)
- **.NET 8 SDK** → Install from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Node.js & Yarn** → Install from [nodejs.org](https://nodejs.org/)

---

### **2️⃣ Setup Instructions**

#### **🔧 Clone the Repository**

```bash
git clone https://github.com/yourusername/MiniFlexCRM.git
cd MiniFlexCRM
```

#### **🔧 Start the Application with Docker**

```bash
docker-compose up --build
```

This will:

- Start **PostgreSQL** (database)
- Start **MiniFlexCRM API** (backend)
- Start **React Frontend** (UI)

#### **🔧 Run Without Docker (Manual Mode)**

1. **Start the Database**
    
    ```bash
    docker run --name miniflexcrm-db -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -e POSTGRES_DB=miniflexcrm -p 5432:5432 -d postgres
    ```
    
2. **Run Migrations (if applicable)**
    
    ```bash
    dotnet ef database update
    ```
    
3. **Run the API**
    
    ```bash
    cd MiniFlexCrmApi
    dotnet run
    ```
    
4. **Run the Frontend**
    
    ```bash
    cd frontend
    yarn install && yarn start
    ```
    

---

## **🔐 Authentication & Authorization**

MiniFlexCRM uses **JWT-based authentication** with **RBAC (Role-Based Access Control)**.

### **1️⃣ Obtain a Token (Login)**

```http
POST /api/auth/login
Content-Type: application/json

{
    "username": "admin",
    "password": "securepassword"
}
```

**Response:**

```json
{
    "token": "eyJhbGciOiJIUzI1...",
    "expiration": "2024-02-06T12:00:00Z"
}
```

### **2️⃣ Access Protected Endpoints**

Include the token in the `Authorization` header:

```http
GET /api/customers
Authorization: Bearer eyJhbGciOiJIUzI1...
```

### **3️⃣ Role-Based Access Control**

- **`admin`** → Full access to tenant data.
- **`user`** → Limited access (CRUD operations only on assigned entities).
- **`readonly`** → Can only view data.

---

## **📡 Updated API Endpoints**

|Method|Endpoint|Description|
|---|---|---|
|**Auth (No Tenant Context Required)**|||
|`POST`|`/api/auth/login`|Authenticate user and get a JWT token|
|`POST`|`/api/auth/signup`|Create a new user account|
|`POST`|`/api/auth/refresh`|Refresh an expired token|
|**Tenant Management**|||
|`GET`|`/api/tenant/{tenant_id}`|Get tenant details|
|`POST`|`/api/tenant`|Create a new tenant|
|**Customer Management**|||
|`GET`|`/api/tenant/{tenant_id}/customers`|Get all customers for a tenant|
|`GET`|`/api/tenant/{tenant_id}/customers/{id}`|Get a specific customer|
|`POST`|`/api/tenant/{tenant_id}/customers`|Create a customer|
|`PUT`|`/api/tenant/{tenant_id}/customers/{id}`|Update customer details|
|`DELETE`|`/api/tenant/{tenant_id}/customers/{id}`|Remove a customer|
|**Company Management**|||
|`GET`|`/api/tenant/{tenant_id}/companies`|Get all companies for a tenant|
|`GET`|`/api/tenant/{tenant_id}/companies/{id}`|Get a specific company|
|`POST`|`/api/tenant/{tenant_id}/companies`|Create a company|
|`PUT`|`/api/tenant/{tenant_id}/companies/{id}`|Update company details|
|`DELETE`|`/api/tenant/{tenant_id}/companies/{id}`|Remove a company|
|**User Management**|||
|`GET`|`/api/tenant/{tenant_id}/users`|Get all users for a tenant|
|`GET`|`/api/tenant/{tenant_id}/users/{id}`|Get a specific user|
|`POST`|`/api/tenant/{tenant_id}/users`|Create a user|
|`PUT`|`/api/tenant/{tenant_id}/users/{id}`|Update user details|
|`DELETE`|`/api/tenant/{tenant_id}/users/{id}`|Remove a user|
|**Relations Management**|||
|`GET`|`/api/tenant/{tenant_id}/relations`|Get all relations for a tenant|
|`GET`|`/api/tenant/{tenant_id}/relations/{id}`|Get a specific relation|
|`POST`|`/api/tenant/{tenant_id}/relations`|Create a relation|
|`PUT`|`/api/tenant/{tenant_id}/relations/{id}`|Update relation details|
|`DELETE`|`/api/tenant/{tenant_id}/relations/{id}`|Remove a relation|

---

## **🔐 Authentication & Multi-Tenant Authorization**

### **1️⃣ Obtain a JWT Token (Login)**

```http
POST /api/auth/login
Content-Type: application/json

{
    "username": "admin",
    "password": "securepassword"
}
```

**Response:**

```json
{
    "token": "eyJhbGciOiJIUzI1...",
    "expiration": "2024-02-06T12:00:00Z"
}
```

### **2️⃣ Access Protected Tenant-Specific Endpoints**

Pass the `tenant_id` in the path and include the token in the `Authorization` header:

```http
GET /api/tenant/1/customers
Authorization: Bearer eyJhbGciOiJIUzI1...
```

✅ **Ensures the user has access to the specified tenant.**  
✅ **Prevents unauthorized cross-tenant access.**