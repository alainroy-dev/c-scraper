# WebScraper(Boxing) (C# + Playwright)

A web scraper built with **C#** and **Microsoft Playwright** that automates login, navigation, and data extraction.  
The scraper also supports **proxy configuration**, **JSON file storage**, and can be published as a **standalone executable**.

---

## 🚀 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- Windows 10/11 (x64)  
- Git (optional, for cloning)

---

## 📦 Installation Guide

### 1. Clone the repository
```sh
git clone https://github.com/alainroy-dev/c-scraper.git
cd c-scraper
```

### 2. Install dependencies

```sh
dotnet add package Microsoft.Playwright
dotnet add package DotNetEnv
```

### 3. Build the project

```sh
dotnet build
```

### 4. Install Playwright CLI (once per system)

```sh
dotnet tool install --global Microsoft.Playwright.CLI
```

### 5. Install Playwright browsers

```sh
playwright install
```

---

## ▶️ Run the Scraper

To run directly:

```sh
dotnet run
```

---

## 📦 Publish as Standalone Executable

If you want to create a **single EXE** for Windows (64-bit):

```sh
dotnet publish -c Release -r win-x64 --self-contained true
```

This will generate files in:

```
bin/Release/net9.0/win-x64/publish/
```

You can share `BoxrecScraper.exe` with clients — they **do not need .NET installed**.

---

## ⚙️ Environment Variables

This project supports `.env` files (via **DotNetEnv**) for storing credentials and proxy settings.

Example `.env` file:

```
IS_PRODUCTION=true
SCRAPER_INTERVAL_MINS=20
PROXY_SERVER=gate.proxyprovider.com:1234
PROXY_USERNAME=myusername
PROXY_PASSWORD=mypassword

```

---

## 🔁 Run Continuously

The scraper is designed to run in a **continuous loop**.

* If an exception occurs, it **automatically restarts**.
* Press any key in the console to **stop it gracefully**.

---

## 🔑 Features

* Automated login with Playwright
* Works with proxies (residential/mobile/sticky sessions)
* JSON-based local data storage (no database required)
* Handles multi-tab scraping
* Retry & exception handling
* Can run continuously in background

---

## 🛠 Troubleshooting

* **Playwright browsers not found** → Run `playwright install` again.
* **Permission error when running `playwright.ps1`** → Run PowerShell as admin and execute:

  ```sh
  Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
  ```
* **Proxy login/session issues** → Ensure your proxy supports sticky sessions and check provider docs.

---

## 📄 License

MIT License.
Use responsibly and ensure compliance with the target website’s **Terms of Service**.
