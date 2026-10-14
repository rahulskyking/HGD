# Architecture Decision Log

Short records of decisions that shape the codebase. Newest at the bottom.
Format: Decision · Reason · Alternatives considered · Chosen approach.

---

## ADR-001 — Layering and dependency direction

- **Decision:** Four projects — `Domain`, `Application`, `Infrastructure`, `Web` — with
  `Web → Application → Domain` and `Infrastructure → Application → Domain`. `Web` references
  `Infrastructure` only to call `AddInfrastructure()` from the composition root.
- **Reason:** Keep business rules independent of EF Core, ASP.NET and third-party services.
- **Alternatives:** Single project; vertical-slice-only without a Domain project.
- **Chosen:** Clean-architecture layering as above (pre-existing in the initial commit).

## ADR-002 — Application use cases are plain handlers (no MediatR)

- **Decision:** Each use case is a `record` command/query plus a handler class registered in DI
  (`AddApplication()`), organised by feature under `Application/Features/<Feature>/Commands|Queries/<UseCase>/`.
  Single-call operations with no orchestration (e.g. login, logout, confirm email) are invoked
  directly on the Application interface from Web rather than through pass-through handlers.
- **Reason:** Simplest thing that keeps orchestration out of Web; no pipeline needed yet.
- **Alternatives:** MediatR + FluentValidation pipeline behaviours.
- **Chosen:** Plain handlers. Revisit if cross-cutting pipeline behaviour (validation, logging,
  transactions) becomes repetitive.

## ADR-003 — ASP.NET Identity is fully wrapped behind `IIdentityService`

- **Decision:** Web never uses `UserManager`, `SignInManager` or `ApplicationUser`. All account
  and sign-in operations go through `Application.Identity.IIdentityService`, implemented in
  `Infrastructure/Identity/IdentityService` and returning Application records
  (`IdentityOperationResult`, `CreateUserResult`, `UserAccount`, `SignInOutcome`, `ExternalLoginDetails`).
  Identity's user type and the User↔Profile FK are configured only in Infrastructure.
- **Reason:** Prevents Infrastructure types leaking into Web; keeps Web pages thin and testable.
- **Accepted detail:** Starting an OAuth challenge is an HTTP concern and stays in Web; it sets
  the `LoginProvider` item that Identity expects (mirrors `SignInManager.ConfigureExternalAuthenticationProperties`).
- **Alternatives:** Identity-UI style — Web uses `SignInManager<ApplicationUser>` directly.
- **Chosen:** Full wrap.

## ADR-004 — Email via Resend HTTPS API with a plain `HttpClient`

- **Decision:** `Application.Email.IEmailSender` with two Infrastructure implementations:
  `ResendEmailSender` (typed `HttpClient`, `POST https://api.resend.com/emails`) and
  `LogEmailSender` (writes the message to the log). Selected by `Email:Provider` (`Resend` | `Log`);
  `Resend` fails fast at startup if `ApiKey`/`FromAddress` are missing. No SMTP, ever (spec §20).
- **Reason:** One API call does not justify an SDK dependency; the log sender makes local
  development possible without credentials.
- **Alternatives:** Official Resend NuGet SDK; defer email entirely.
- **Chosen:** HttpClient + Log sender.

## ADR-005 — Email confirmation is required before password sign-in

- **Decision:** `SignIn.RequireConfirmedEmail = true`. Accounts created through an external
  provider start with `EmailConfirmed = true`.
- **Reason:** Spec §7.1 ("email verification"); prevents sign-in with unverified addresses.
- **Alternatives:** Allow sign-in and nag.
- **Chosen:** Required.

## ADR-006 — External logins auto-link only to confirmed accounts

- **Decision:** When an external sign-in has no linked account but its email matches an existing
  account, the login is linked and the user signed in **only if that account's email is confirmed**.
  Unconfirmed matches are rejected with guidance to confirm first. Unknown emails go to a
  role-selection step that creates a new account.
- **Reason:** Convenience for Google users with an existing password account, without allowing
  an unconfirmed (possibly attacker-created) account to be taken over.
- **Alternatives:** Never auto-link (require password sign-in then explicit linking).
- **Chosen:** Guarded auto-link. Easy to flip to "never" in `SignInWithExternalLoginHandler`.

## ADR-007 — `UserRole` enum in Domain is the source of role names

- **Decision:** `Domain.Enums.UserRole { Candidate, Studio, Admin }`; the member names are the
  Identity role names. `IdentitySeeder` seeds one role per member. Self-registration rule
  (`CanSelfRegister`) lives in Domain.
- **Reason:** Role is a business concept; avoids string role names scattered across layers.
- **Chosen:** Enum + extension.

## ADR-008 — EF migrations are generated locally, not hand-written

- **Decision:** Agent changes entities/configurations; the developer runs
  `dotnet ef migrations add <Name> --project src/HireGameDev.Infrastructure --startup-project src/HireGameDev.Web`
  and commits the result.
- **Reason:** The agent sandbox has no .NET SDK; hand-written migrations risk desynchronising the model snapshot.
