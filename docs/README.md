# HireGameDev — Documentation

All project documentation lives in Notion and is the single source of truth
(see ADR-010). This file only tells you where to look.

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
