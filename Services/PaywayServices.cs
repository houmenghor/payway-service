using kh.gov.nbc.bakong_khqr;
using kh.gov.nbc.bakong_khqr.model;
using Microsoft.Extensions.Options;
using payway.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace payway.Services
{
    public class PaywayServices
    {
        private readonly BakongSettings _settings;
        private readonly HttpClient _httpClient;

        private const int QR_EXPIRATION_MINUTES = 5;

        public PaywayServices(IOptions<BakongSettings> options, HttpClient httpClient)
        {
            _settings = options.Value;
            _httpClient = httpClient;

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Token);
        }

        public PaymentResponse GenerateKHQR(decimal amount, string? billNumber = null, string? mobileNumber = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            var bill = string.IsNullOrWhiteSpace(billNumber)
                ? Guid.NewGuid().ToString()
                : billNumber;

            var mobile = string.IsNullOrWhiteSpace(mobileNumber)
                ? "85512233457"
                : mobileNumber;

            var expirationTime = DateTimeOffset.UtcNow.AddMinutes(QR_EXPIRATION_MINUTES);
            long expirationTimestamp = expirationTime.ToUnixTimeMilliseconds();

            var response = BakongKHQR.GenerateIndividual(
                new IndividualInfo
                {
                    BakongAccountID = _settings.BakongAccount,
                    Currency = KHQRCurrency.USD,
                    AccountInformation = mobile,
                    AcquiringBank = _settings.BankName,
                    Amount = Convert.ToDouble(amount),
                    MerchantName = _settings.MerchantName,
                    MerchantCity = _settings.MerchantCity,
                    BillNumber = bill,
                    MobileNumber = mobile,
                    ExpirationTimestamp = expirationTimestamp
                }
            );

            if (response.Status.Code != 0)
            {
                throw new Exception("Failed to generate KHQR: " + response.Status.Message);
            }

            return new PaymentResponse
            {
                QRToken = response.Data.QR,
                MD5 = response.Data.MD5,
                BillNumber = bill
            };
        }
        public async Task<PaymentStatusResponse> CheckPaymentByMd5Async(string md5)
        {
            var requestBody = new
            {
                md5 = md5
            };

            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(_settings.InquiryUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP Error: {response.StatusCode} - {responseJson}");
            }

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            // Bakong API: responseCode 0 is success
            if (!root.TryGetProperty("responseCode", out var codeElement) || codeElement.GetInt32() != 0)
            {
                var message = root.TryGetProperty("responseMessage", out var msgElement) ? msgElement.GetString() : "Unknown error";
                return new PaymentStatusResponse
                {
                    IsPaid = false,
                    Status = message ?? "PENDING"
                };
            }

            var data = root.GetProperty("data");

            return new PaymentStatusResponse
            {
                IsPaid = true,
                Status = "PAID",
                PaidAmount = data.TryGetProperty("amount", out var amt)
                    ? (amt.ValueKind == JsonValueKind.Number ? amt.GetDecimal() : decimal.Parse(amt.GetString()!))
                    : null,
                PaidAt = data.TryGetProperty("createdDate", out var createdDate)
                    ? DateTime.Parse(createdDate.GetString()!)
                    : null,
                TransactionId = data.TryGetProperty("hash", out var hash)
                    ? hash.GetString()
                    : null
            };
        }
    }

}