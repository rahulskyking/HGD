# HireGameDev — Documentation

All project documentation lives in Notion and is the single source of truth
(see ADR-010). This file tells you where to look and how to run the app locally.

**Root page:** HireGameDev  
https://www.notion.so/3f98b4492f4d8066b1c4cf4da6a91977

| Section | What you will find | Link |
|---|---|---|
| Overview | What the product is, roles, current status, how we work | https://www.notion.so/3f98b4492f4d814aa6f3d8e60e285038 |
| Architecture & Conventions | Layering, project layout, naming, how to add a feature, reuse rules | https://www.notion.so/3f98b4492f4d81ce8ffce54fdf6ef022 |
| Database Schema | One page per table (columns, indexes, relationships, planned changes) — matches the latest committed migration | https://www.notion.so/a72718773be0434e86e95e3974ee5353 |
| Features & Roadmap | One page per step/feature with status, scope and dependencies | https://www.notion.so/bb90dc3e96a8452b988b9f9cedb0d746 |
| Decisions | Architecture Decision Records (ADR-001 …) | https://www.notion.so/b25a9f111ad44976845b4db59d2f14de |

## Rules

- Nothing is documented as implemented unless it is in this repository.
- Every completed step updates: its Features & Roadmap page, affected Database Schema pages, and any new ADRs.
- `HGD.html` in the repository root is a product/visual reference only (ADR-011); it is not a specification to follow blindly.
- EF Core migrations are generated locally by the developer, never hand-written (ADR-008):

  ```bash
  dotnet ef migrations add <Name> --project src/HireGameDev.Infrastructure --startup-project src/HireGameDev.Web
  ```

## Local setup

Sign-in is Google / GitHub only (ADR-013), so you need OAuth credentials even locally.
Secrets go into `dotnet user-secrets` (the Web project already has a `UserSecretsId`); never into `appsettings*.json`.

### 1. PostgreSQL

Create an empty database (e.g. `hiregamedev`) and store the connection string:

```bash
cd src/HireGameDev.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hiregamedev;Username=postgres;Password=YOUR_PASSWORD"
```

### 2. Google OAuth app (required)

1. https://console.cloud.google.com → create/select a project.
2. **APIs & Services → OAuth consent screen**: User type *External*, fill in app name + emails. While the app is in *Testing*, add your own Google account under **Test users**.
3. **APIs & Services → Credentials → Create credentials → OAuth client ID**: Application type *Web application*.
   - Authorized JavaScript origins: `https://localhost:7059`
   - Authorized redirect URIs: `https://localhost:7059/signin-google`
4. Copy the Client ID and Client secret:

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "xxxxxxxx.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "GOCSPX-..."
```

### 3. GitHub OAuth app (optional — the button only appears when configured)

1. https://github.com/settings/developers → **OAuth Apps → New OAuth App**.
   - Homepage URL: `https://localhost:7059`
   - Authorization callback URL: `https://localhost:7059/signin-github`
2. Register, then **Generate a new client secret**:

```bash
dotnet user-secrets set "Authentication:GitHub:ClientId" "Iv1...."
dotnet user-secrets set "Authentication:GitHub:ClientSecret" "..."
```

### 4. Run

```bash
dotnet dev-certs https --trust            # once per machine
dotnet ef database update --project src/HireGameDev.Infrastructure --startup-project src/HireGameDev.Web
dotnet run --project src/HireGameDev.Web --launch-profile https
```

Open https://localhost:7059 → *I'm a game developer* / *I'm a studio* → Continue with Google.
Use the `https` profile: the OAuth callback URLs above are registered for port 7059.
