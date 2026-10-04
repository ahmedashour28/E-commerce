# E-Commerce API

ASP.NET Core Web API for an e-commerce backend: catalog, cart, orders, Stripe payments, JWT auth, reviews, and wishlists.

## Features

- **Catalog** — Products and hierarchical categories; admin CRUD with Cloudinary image upload
- **Search** — Product pagination, search, and sorting via query params
- **Cart** — Database-backed cart (authenticated users or guest `buyerId`)
- **Orders** — Create from cart with stock reservation; statuses: `Pending` → `PaymentReceived` / `PaymentFailed` → `Shipped` → `Complete`
- **Payments** — Stripe PaymentIntents + webhooks; cash-on-delivery option
- **Auth & security** — JWT access + refresh tokens, roles (`Admin` / `User`), email verification, password reset
- **Reviews** — Product reviews limited to verified purchasers
- **Wishlist** — Per-user product wishlist
- **Background jobs** — Releases stock for unpaid Stripe orders after 30 minutes
- **Ops** — Serilog logging, global exception middleware, OpenAPI in Development

## Stack

| Area | Technology |
|------|------------|
| Runtime | .NET 10 / ASP.NET Core Web API |
| Data | Entity Framework Core 10, SQL Server |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Logging | Serilog |
| Email | MailKit / MimeKit |
| Images | Cloudinary |
| Payments | Stripe.net |

## Project layout

```
E-Commerce/
├── ECommerce.slnx
└── ECommerce.Api/
    ├── Controllers/
    ├── Data/              # DbContext, seed
    ├── Dtos/
    ├── Entities/
    ├── Repositories/
    ├── Services/          # App services + background jobs
    ├── Middleware/
    ├── Migrations/
    └── Validators/
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Docker, or full instance)
- Accounts for full features: Cloudinary, Stripe, SMTP (e.g. Gmail app password or Ethereal for dev)

## Configuration

Committed `appsettings.json` files do **not** contain secrets. Use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) (preferred for local dev) or environment variables.

```bash
cd ECommerce.Api
dotnet user-secrets set "ConnectionStrings:ECommerceDbConnection" "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_LONG_RANDOM_SECRET"
dotnet user-secrets set "Jwt:Issuer" "ECommerceApi"
dotnet user-secrets set "Jwt:Audience" "ECommerceClient"
dotnet user-secrets set "Jwt:ExpireMinutes" "60"
dotnet user-secrets set "AppUrl" "https://localhost:7029"
dotnet user-secrets set "AdminSettings:UserName" "admin"
dotnet user-secrets set "AdminSettings:Email" "admin@example.com"
dotnet user-secrets set "AdminSettings:Password" "YourAdminPassword1"
dotnet user-secrets set "AdminSettings:FullName" "Store Admin"
```

Equivalent JSON shape (User Secrets / private config):

```json
{
  "ConnectionStrings": {
    "ECommerceDbConnection": "Server=...;Database=ECommerceDb;..."
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_MUST_BE_LONG",
    "Issuer": "ECommerceApi",
    "Audience": "ECommerceClient",
    "ExpireMinutes": "60"
  },
  "AppUrl": "https://localhost:7029",
  "AdminSettings": {
    "UserName": "admin",
    "Email": "admin@example.com",
    "Password": "YourAdminPassword1",
    "FullName": "Store Admin"
  },
  "CloudinarySettings": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  },
  "StripeSettings": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "MailSettings": {
    "EmailFrom": "no-reply@ecommerce.com",
    "DisplayName": "E-Commerce",
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "...",
    "SmtpPass": "..."
  }
}
```

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:ECommerceDbConnection` | SQL Server |
| `Jwt:*` | Access token signing and validation |
| `AppUrl` | Base URL for email verification / password-reset links |
| `AdminSettings:*` | Seeds the initial Admin user on startup (`Email` + `Password` required) |
| `CloudinarySettings:*` | Product image upload |
| `StripeSettings:*` | Payments and webhook verification |
| `MailSettings:*` | Transactional email |

CORS allows `http://localhost:5173` (typical Vite frontend). Change the policy in `Program.cs` if your client uses another origin.

## Getting started

```bash
git clone https://github.com/ahmedashour28/E-commerce.git
cd ECommerce
dotnet restore
```

Configure secrets (see above), then apply migrations and run:

```bash
cd ECommerce.Api
dotnet ef database update
dotnet run --launch-profile https
```

| Profile | URLs |
|---------|------|
| `https` (default recommended) | `https://localhost:7029`, `http://localhost:5045` |
| `http` | `http://localhost:5045` |

On startup the API seeds `Admin` / `User` roles and the admin account from `AdminSettings`. Migrations are **not** applied automatically — run `dotnet ef database update` first.

In Development, OpenAPI is available via the mapped OpenAPI endpoint (no Swagger UI package is included).

## API overview

| Area | Prefix | Notes |
|------|--------|--------|
| Auth | `api/Auth` | Register, login, verify email, forgot/reset password, refresh token |
| Products | `api/Products` | Public read; admin create/update/delete (multipart + images) |
| Categories | `api/Categories` | Public read; admin write |
| Cart | `api/Cart` | Optional auth; pass `buyerId` for guests |
| Orders | `api/Orders` | Authenticated create/list; admin status updates |
| Payments | `api/Payments` | Create PaymentIntent; Stripe webhook |
| Reviews | `api/products/{productId}/reviews` | Authenticated create (verified purchase) |
| Wishlist | `api/Wishlist` | Authenticated get / toggle |
| Admin | `api/Admin` | List users, assign role, delete user |

Product list query params include `PageNumber`, `PageSize`, `SearchTerm`, and `OrderBy` (`priceAsc`, `priceDesc`, `nameDesc`, or default name ascending). Paginated responses include an `X-Pagination` header.

## Stripe (local)

Forward webhooks to the running API:

```bash
stripe listen --forward-to https://localhost:7029/api/payments/webhook
```

Confirm a test PaymentIntent:

```bash
stripe payment_intents confirm {payment_intent_id} --payment-method=pm_card_visa
```

Copy the CLI webhook signing secret into `StripeSettings:WebhookSecret`.

## License

MIT — see [LICENSE.txt](LICENSE.txt).
