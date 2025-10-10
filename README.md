# 📚 Flashcards Study App

A console-based application built in C\# and Dapper for managing flashcard stacks, conducting study sessions, and tracking user performance history.

## ✨ Features

This application fully implements **CRUD** (Create, Read, Update, Delete) for two core entities and includes a complex study session module:

1.  **Stack Management:** Create, view, update, and delete topic stacks.
2.  **Flashcard Management:** Create, view, update, and delete flashcards linked to specific stacks.
3.  **Study Sessions:**
    * Users can select any stack to study.
    * Cards are **randomly shuffled** for each session.
    * Scores are calculated based on correct answers.
4.  **History Tracking:**
    * Saves the date, stack, and score for every session to the database.
    * Allows users to **view complete study history**, displaying the correct Stack Name via intelligent data mapping (ID-to-Name).

---

## 🏛️ Architecture & Technologies

The project follows a **Layered Architecture** with a clear **Separation of Concerns (SoC)**, utilizing a dedicated **Service Layer** to house business logic and keep the Controllers clean.

| Layer | Responsibility | Key Technology |
| :--- | :--- | :--- |
| **Presentation/UI** | Handles user input and displays output. | C\# Console & **Spectre.Console** |
| **Controller** | Orchestrates application flow and calls services. | C\# Classes |
| **Service (Business Logic)** | Houses all complex logic (e.g., shuffling, scoring, ID-to-Name mapping). | C\# Classes |
| **Repository (Data Access)** | Communicates directly with the database. | C\# & **Dapper** (Micro-ORM) |
| **Database** | Persistent data storage. | **SQL Server** |

---

## 🚀 Getting Started

Follow these steps to set up and run the application locally.

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (Compatible Version)
* Access to a **SQL Server** instance (LocalDB, Express, or full instance).

### 1. Database Setup

1.  Ensure your connection string is configured to point to a valid $\text{SQL}$ Server instance.
2.  Run your $\text{SQL}$ schema creation scripts to establish the `dbo.stacks`, `dbo.flashcards`, and `dbo.sessions` tables.

### 2. Running the Application

1.  Navigate to the project's root directory in your terminal.
2.  Run the application:

    ```bash
    dotnet run
    ```

### 💡 Load Demo Data

To immediately test all features without manual data entry:

* Uncomment the //Database.CreateDummyData.
* This feature executes a script to populate the database with two stacks ("HTML Basics," "SQL Fundamentals"), four flashcards, and three study session history records.
