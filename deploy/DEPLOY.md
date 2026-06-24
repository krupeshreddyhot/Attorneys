# Deploy Attorneys (Neon + Render + Cloudflare Pages)

Same layout as Abhyanvaya: **Neon** (PostgreSQL), **Render** (API Docker), **Cloudflare Pages** (Vite UI).

---

## 1. Database (Neon)

1. Create a project at [neon.tech](https://neon.tech).
2. Create database `attorneys_db` (or use default).
3. Copy the connection string (SSL required).

Run migrations once from your PC:

```powershell
cd D:\Rupesh\Projects\Attorneys
$env:ConnectionStrings__DefaultConnection="Host=...;Username=...;Password=...;Database=attorneys_db;SSL Mode=Require"
dotnet ef database update --project Attorneys.Infrastructure --startup-project Attorneys.API
```

---

## 2. API (Render — Docker)

1. Push this repo to GitHub.
2. Render → **New Web Service** → connect repo.
3. **Dockerfile path**: `Dockerfile`
4. **Instance**: Free tier is fine for demos.

### Environment variables (Render)

| Key | Value |
|-----|--------|
| `ConnectionStrings__DefaultConnection` | Neon connection string |
| `Jwt__Key` | Long random secret (32+ chars) |
| `Jwt__Issuer` | `Attorneys` |
| `Jwt__Audience` | `AttorneysUsers` |
| `Cors__ReactOrigin` | `https://YOUR-APP.pages.dev,http://localhost:5173` |
| `Documents__Provider` | `s3` (recommended) or `local` |
| `Documents__S3__Bucket` | S3 bucket name |
| `Documents__S3__Region` | e.g. `us-east-1` |
| `Documents__S3__AccessKeyId` | IAM access key |
| `Documents__S3__SecretAccessKey` | IAM secret key |
| `Documents__S3__KeyPrefix` | `cases` (optional folder prefix in bucket) |
| `EnableSwagger` | `true` for demo |

Render sets `PORT` automatically; the API listens on it.

After deploy note API URL, e.g. `https://attorneys-api.onrender.com`.

**Seed users** (created on first startup if DB is empty):

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Administrator |
| staff | Staff@123 | General |

Change these immediately in production.

---

## 3. Frontend (Cloudflare Pages)

1. Dashboard → **Build → Compute → Workers & Pages** → **Create application** → **Pages**.
2. Connect Git repo.
3. **Root directory**: `attorneys-ui`
4. **Build command**: `npm ci && npm run build`
5. **Build output**: `dist`
6. **Project name**: e.g. `attorneys-ui` (lowercase, hyphens only)

### Build environment variable

| Key | Value |
|-----|--------|
| `VITE_API_BASE_URL` | `https://YOUR-API.onrender.com/api` |

Must end with `/api`. Redeploy after changing.

---

## 4. Local testing against cloud API

Create `attorneys-ui/.env.local`:

```
VITE_API_BASE_URL=https://YOUR-API.onrender.com/api
```

Ensure `Cors__ReactOrigin` on Render includes `http://localhost:5173`.

---

## 5. Docker local check

From repo root:

```bash
docker build -t attorneys-api .
docker run --rm -p 8080:8080 ^
  -e ConnectionStrings__DefaultConnection="..." ^
  -e Jwt__Key="your-secret-key-min-32-chars-long" ^
  -e Cors__ReactOrigin="http://localhost:5173" ^
  attorneys-api
```

Open `http://localhost:8080/swagger`.

---

## 6. Document storage (AWS S3 / R2)

Case documents use **`Documents:Provider`** — same S3-compatible pattern as Abhyanvaya branding.

### Recommended: AWS S3 (no Render disk needed)

1. Create an S3 bucket (e.g. `attorneys-documents-prod`).
2. Block public access (files served only through the API).
3. Create an IAM user with policy allowing `s3:PutObject`, `s3:GetObject`, `s3:DeleteObject` on that bucket.
4. Set Render env vars:

| Key | Value |
|-----|--------|
| `Documents__Provider` | `s3` |
| `Documents__S3__Bucket` | your bucket name |
| `Documents__S3__Region` | e.g. `us-east-1` |
| `Documents__S3__AccessKeyId` | IAM access key |
| `Documents__S3__SecretAccessKey` | IAM secret |
| `Documents__S3__KeyPrefix` | `cases` (optional) |

Leave `Documents__S3__Endpoint` empty for AWS S3.

### Cloudflare R2 (S3-compatible)

| Key | Value |
|-----|--------|
| `Documents__Provider` | `s3` |
| `Documents__S3__Bucket` | R2 bucket name |
| `Documents__S3__Endpoint` | R2 S3 API endpoint URL |
| `Documents__S3__Region` | `auto` |
| `Documents__S3__AccessKeyId` | R2 access key |
| `Documents__S3__SecretAccessKey` | R2 secret |
| `Documents__S3__ForcePathStyle` | `true` |

### Local disk (dev or fallback)

Set `Documents__Provider=local`. Files go to `App_Data/documents` locally, or `Documents__PhysicalRoot` if set.

On Render without S3, uploads are **lost on redeploy** unless you mount a persistent disk — S3 is preferred.

---

## 7. Security checklist

- Rotate `Jwt__Key` and DB password before go-live
- Never commit real secrets to git
- Use Neon env vars on Render, not `appsettings.json`
- Change default admin/staff passwords after first login
