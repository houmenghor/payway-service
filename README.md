# Payway Service

A lightweight ASP.NET Core Web API for generating **Bakong KHQR** payment QR codes and checking transaction status via the [National Bank of Cambodia (NBC) Bakong API](https://api-bakong.nbc.gov.kh).

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core Web API |
| QR Generation | [Kh.Gov.Nbc.BakongKHQR](https://www.nuget.org/packages/Kh.Gov.Nbc.BakongKHQR) |
| API Docs | Swagger / OpenAPI |

## Project Structure

```
payway-service/
├── Controllers/
│   └── Payway.cs          # API endpoints
├── Services/
│   └── PaywayServices.cs  # KHQR generation & payment inquiry logic
├── Models/
│   ├── BakongSettings.cs  # Configuration model
│   ├── PaymentRequest.cs
│   ├── PaymentResponse.cs
│   └── PaymentStatusResponse.cs
├── appsettings.json        # App configuration (DO NOT commit secrets)
└── Program.cs
```

## Configuration

Add your Bakong credentials in `appsettings.json` (or use environment variables / `appsettings.Local.json` which is git-ignored):

```json
{
  "BakongSettings": {
    "BakongAccount": "your_account@bkrt",
    "MerchantName": "YOUR NAME",
    "MerchantCity": "PHNOM PENH",
    "BankName": "YOUR BANK",
    "Token": "<your_bakong_jwt_token>",
    "InquiryUrl": "https://api-bakong.nbc.gov.kh/v1/check_transaction_by_md5"
  }
}
```

> **Security notice:** Never commit real tokens or credentials to version control. Use `appsettings.Local.json` or environment variables for secrets.

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run locally

```bash
dotnet restore
dotnet run
```

Swagger UI will be available at `https://localhost:<port>/swagger` in Development mode.

## API Endpoints

### `POST /api/payway/generate`

Generates a KHQR QR code for a payment.

**Request body:**
```json
{
  "amount": 5.00,
  "billNumber": "INV-001",   // optional, auto-generated if omitted
  "mobileNumber": "855xxxxxxxxx"  // optional
}
```

**Response:**
```json
{
  "qrToken": "<khqr_string>",
  "md5": "<md5_hash>",
  "billNumber": "INV-001"
}
```

---

### `GET /api/payway/check-by-md5/{md5}`

Checks whether a transaction has been paid using its MD5 hash.

**Response (paid):**
```json
{
  "isPaid": true,
  "status": "PAID",
  "paidAmount": 5.00,
  "paidAt": "2026-03-24T10:30:00Z"
}
```

**Response (pending):**
```json
{
  "isPaid": false,
  "status": "PENDING"
}
```

---

### `GET /api/payway/test`

Health check endpoint. Returns `"Payway API is working"`.

## QR Expiration

Generated QR codes expire **5 minutes** after creation.

## License

MIT
