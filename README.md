# E-Commerce API

A robust, enterprise-grade RESTful Web API for an E-Commerce platform. Built with **.NET 10**, **Entity Framework Core**, and **SQL Server**.

## Key Features

* **Catalog Management**: Full CRUD for Products and Categories with hierarchical structures.
* **Advanced Search**: Pagination, Sorting, and Filtering implemented via `ProductParams`.
* **Shopping Cart**: Persistent database-backed cart management.
* **Order Processing**: Complete lifecycle management (Pending -> Payment -> Shipped -> Completed).
* **Payments**: Integrated **Stripe** payment intents and Webhooks.
* **Identity & Security**: 
    * JWT Authentication (Access + Refresh Tokens).
    * Role-Based Access Control (Admin vs User).
    * Email Verification & Password Reset flows.
* **User Engagement**: Product Reviews (verified purchase restricted) and Wishlists.
* **Background Jobs**: Automated stock release for expired/unpaid orders using `BackgroundService`.
* **Media**: Cloudinary integration for image optimization and storage.

## Technology Stack

* **Core**: ASP.NET Core Web API (.NET 10)
* **Data**: Entity Framework Core, SQL Server
* **Validation**: FluentValidation
* **Mapping**: AutoMapper
* **Logging**: Serilog (Console + File sinks)
* **Email**: MailKit / MimeKit
* **Payment**: Stripe.net SDK

## Getting Started

### Prerequisites

1.  [**.NET 10 SDK**](https://dotnet.microsoft.com/download)
2.  **SQL Server** (LocalDB, Docker, or SSMS)
3.  **Cloudinary Account** (for images)
4.  **Stripe Account** (for payments)
5.  **SMTP Server** (e.g., Gmail/Ethereal for dev, SendGrid for prod)

### Configuration

Update `appsettings.json` or use User Secrets:

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
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "...",
    "SmtpPass": "..."
  }
}
```
## Installation
### 1. Clone & Restore
```
git clone https://github.com/dabananda/ECommerce.git
dotnet restore
```
### 2. Database Migration
```
cd ECommerce.Api
dotnet ef database update
```
### 3. Run Application
```
dotnet run
```
The API will run on https://localhost:7029.

## Documentation
Detailed documentation is available in the `/docs` folder:

## Testing
- **Postman:** Import the collection provided in `docs/postman_collection.json`.

- **Stripe Webhooks:** Use the Stripe CLI to forward events locally:
```
stripe listen --forward-to https://localhost:7029/api/payments/webhook
```
- **Console Payment:** Use the Stripe CLI to to make payment locally:
```
stripe payment_intents confirm {payment_intent} --payment-method=pm_card_visa
```
