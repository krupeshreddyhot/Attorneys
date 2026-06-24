# Attorneys — Legal Case Management

Web application migrated from the legacy VB6 **Attorneys** desktop app. Built with the same architecture pattern as Abhyanvaya.

## Stack

| Layer | Technology |
|-------|------------|
| Frontend | React + TypeScript + Vite + MUI |
| Backend | ASP.NET Core 8 Web API |
| Database | PostgreSQL (EF Core) |
| Auth | JWT (Administrator / General roles) |
| Documents | Local disk or S3/R2 (configurable) |

## Project structure

```
Attorneys/
├── Attorneys.sln
├── Attorneys.Domain/
├── Attorneys.Application/
├── Attorneys.Infrastructure/
├── Attorneys.API/
├── attorneys-ui/
├── Dockerfile
└── deploy/DEPLOY.md
```

## Features

- Public landing page + staff login
- **Case Entry** — full form with editable hearing grid (Stage, Date, Next Date, IA, IA Stage)
- **Courts** — CRUD (admin)
- **Reports** — Today's Cases, Diary (by date), Court Wise, Pending
- **Documents** — upload/download PDF, DOC, images per case
- **Accounts** — payments (admin only)

## Prerequisites

- .NET 8 SDK
- Node.js 20+
- PostgreSQL (local or Neon)

## Database setup

1. Create PostgreSQL database `attorneys_db`.
2. Update connection string in `Attorneys.API/appsettings.Development.json`.
3. Migrations run automatically on API startup.

## Run locally

**API:**
```powershell
cd Attorneys.API
dotnet run --launch-profile https
```
Swagger: https://localhost:7063/swagger

**UI:**
```powershell
cd attorneys-ui
npm install
npm run dev
```
App: http://localhost:5173

## Default users (seeded)

| Login type | Firm Code | Username | Password | Role |
|------------|-----------|----------|----------|------|
| Firm staff | `DEMO` | admin | Admin@123 | Administrator |
| Firm staff | `DEMO` | staff | Staff@123 | General |
| Super Admin | — | superadmin | SuperAdmin@123 | SuperAdmin |

Super Admin can provision new law firms at `/app/organizations`. Firm staff log in with **Firm Code + Username + Password**.

Change these before production.

## Routes

| URL | Description |
|-----|-------------|
| `/` | Landing page |
| `/login` | Staff login |
| `/app/cases/new` | New case entry |
| `/app/cases/{caseNo}/edit` | Edit case |
| `/app/reports` | Reports (tabs) |
| `/app/documents` | Document upload |

## Deploy to cloud

See **[deploy/DEPLOY.md](deploy/DEPLOY.md)** for Neon + Render + Cloudflare Pages setup.

## Notes

- Schema based on legacy `LEGAL.MDB`; no Access data import.
- Document files: `Documents:Provider=local` uses `App_Data/documents`; set `Documents:Provider=s3` for AWS S3 or Cloudflare R2 in production (see `deploy/DEPLOY.md`).
