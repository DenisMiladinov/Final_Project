# VacationSpot Booking App

A .NET 8 ASP .NET MVC application for browsing, booking and managing vacation rentals—complete with roles (Admin/Receptionist/User), Stripe payments, and live chat support via SignalR.

---

## Table of Contents

1. [Features](#features)  
2. [Tech Stack](#tech-stack)  
3. [Prerequisites](#prerequisites)  
4. [Getting Started](#getting-started)  
   1. [Clone & Build](#clone--build)  
   2. [Configuration](#configuration)  
   3. [Database Setup & Seeding](#database-setup--seeding)  
   4. [Running the App](#running-the-app)  
5. [Seeded Users & Roles](#seeded-users--roles)  
6. [Live Chat Support](#live-chat-support)  
7. [Testing](#testing)  
8. [CI/CD Pipeline](#cicd-pipeline)  
9. [Deployment](#deployment)  
10. [Folder Structure](#folder-structure)  
11. [Contributing](#contributing)  
12. [License](#license)

---

## Features

- Browse vacation spots by category  
- Bookings with Stripe payment integration  
- Roles & permissions:  
  - **Admin**: manage spots, categories, bookings, users  
  - **Receptionist**: manage bookings, live-chat support  
  - **User**: browse, book, chat  
- Live chat between users & receptionists via SignalR  
- EF Core Code-First with data seeding  
- Unit tests (target ≥ 65% coverage)  
- CI/CD via GitHub Actions and auto-deploy to Render.com

---

## Tech Stack

- **Framework**: .NET 8, ASP .NET Core MVC  
- **ORM**: Entity Framework Core  
- **Auth**: ASP .NET Identity (roles: Admin, Receptionist, User)  
- **Real-time**: SignalR  
- **Payments**: Stripe Checkout  
- **CI/CD**: GitHub Actions  
- **Deployment**: Docker / Render.com  

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- SQL Server (local or Azure)  
- (Optional) Docker CLI for container builds  
- A Stripe account for `Stripe:SecretKey`  

---

## Getting Started
Extract the zip and run Server.sln

### Clone & Build

```bash
git clone https://github.com/your-org/vacationspot.git
cd vacationspot/Server
dotnet build

Configuration
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FinalProject;Trusted_Connection=True;MultipleActiveResultSets=true",
  },
  "Stripe": {
    "SecretKey": "sk_test_51RIoCOJQHuuAFDcLf1cc4r8CCHYn6U33Y9FiGPDcO7PbgQIxq3Pc5Ga53b2JzmkbteRK7D5his36lXRzdwrxWXLI00itXDKPKh"
  }
}

cd Server
dotnet ef database update
# DataSeeder will auto-migrate & seed on first run
dotnet run

dotnet run --project Server/Server.csproj
# open https://localhost:5001 in your browser

Seeded Users & Roles

Role	Email	Password
Admin	Admin	Admin123!
User	GuestUser User123!
Receptionist	Receptionist Reception123!

Live Chat Support
Users click Chat on their booking details to message receptionists.

Receptionists see all incoming chats via the Manage Bookings page or Support panel.

Powered by SignalR hub at /chatHub.

cd UnitTests
dotnet test --collect:"XPlat Code Coverage"

CI/CD Pipeline
All pushes & PRs run the .github/workflows/ci.yml workflow:

dotnet restore

dotnet build

dotnet test with coverage

Coverage gate (≥ 65%)

dotnet publish

Artifact upload

The main branch is protected to require a passing build.
