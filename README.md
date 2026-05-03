# Delivery Management System

A Windows desktop application for managing and monitoring deliveries, employees, and customers for a delivery company.  
Built with C# / WPF using a layered architecture (DAL → BL → PL).

---

##  Features

- Manage **deliveries** — create, track, and update delivery status
- Manage **employees** — assign couriers and track workload
- Manage **customers** — store and view customer information
- Two data storage modes: **XML** and **In-Memory List**
- Clean layered architecture following SOLID principles

---

## Project Architecture

```
delivery-management-system/
├── DalFacade/      # DAL interface (abstraction layer)
├── DalList/        # In-memory data storage implementation
├── DalXml/         # XML-based data storage implementation
├── BL/             # Business Logic layer
├── BlTest/         # Business Logic unit tests
├── DalTest/        # DAL unit tests
├── PL/             # Presentation Layer (WPF UI)
└── xml/            # XML data files
```

---

##  Requirements

- Windows 10 / 11
- [.NET 6.0+](https://dotnet.microsoft.com/en-us/download)
- Visual Studio 2022 (to build from source)

---

## Getting Started

### Option A — Download & Run (no installation needed)
1. Go to the [Releases](../../releases) page
2. Download the latest `DeliveryManagementSystem.zip`
3. Extract and run `DeliveryApp.exe`

### Option B — Build from Source
```bash
git clone https://github.com/ShiraStern/dotNet5786_8477_9698.git
cd dotNet5786_8477_9698
```
Then open `dotNet5786_8477_9698.sln` in Visual Studio and press **F5** to run.

---

## 👥 Authors

Developed as part of the .NET Windows Systems course — Lev Academy, Jerusalem.


