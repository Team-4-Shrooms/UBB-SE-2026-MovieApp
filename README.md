# UBB-SE-2026-MovieApp

## Setup

1. Open `src/MovieApp/MovieApp.slnx`
2. Build the solution: **Build → Build Solution**

## Running the App

The app has three projects that must start in a specific order.

**Using Multiple Startup Projects (recommended):**

1. Right-click the solution in Solution Explorer → **Set Startup Projects...**
2. Select **Multiple startup projects**
3. Set the following projects to **Start**, in this order:
   - `MovieApp.WebApi`
   - `MovieApp.Web`
   - `MovieApp`
4. Click **OK**, then press **F5**

**Or start them manually one by one** (each in a separate terminal):

```bash
# 1. Start the API first (port 4544)
dotnet run --project src/MovieApp/MovieApp.WebApi

# 2. Start the web app second (port 5231)
dotnet run --project src/MovieApp/MovieApp.Web

# 3. Start the desktop app last
dotnet run --project src/MovieApp/MovieApp
```

> **Important:** `MovieApp.WebApi` must be fully started before the other two, as both the web app and the desktop app connect to it on startup.

## Structure
- src/ - source code
- docs/ - documentation

## Code Style

All coding conventions enforced in this project are documented in [Coding Style Rules](./docs/CODING_STYLE.md).

## Code Review Checklist

Before submitting a pull request, please go through the [Code Review Checklist](./docs/CODE_REVIEW_CHECKLIST.md) to make sure your changes are consistent with the project's standards.

## Legacy

This project was created by merging two independent repositories:

- **Team 927/1** — https://github.com/Team-4-Shrooms/UBB-SE-2026-927-1
- **Team 925/1** — https://github.com/UBB-SE-2026-925-1/UBB-SE-2026-925-1
