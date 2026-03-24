using Microsoft.AspNetCore.Mvc;
using payway.Models;
using payway.Services;

namespace payway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaywayController : ControllerBase
    {
        private readonly PaywayServices _paywayService;

        public PaywayController(PaywayServices paywayService)
        {
            _paywayService = paywayService;
        }

        [HttpPost("generate")]
        public IActionResult Generate([FromBody] PaymentRequest request)
        {
            try
            {
                var result = _paywayService.GenerateKHQR(
                    request.Amount,
                    request.BillNumber,
                    request.MobileNumber
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "KHQR_GENERATION_FAILED",
                    message = ex.Message
                });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Payway API is working");
        }

        [HttpGet("check-by-md5/{md5}")]
        public async Task<IActionResult> CheckPaymentByMd5(string md5)
        {
            try
            {
                var result = await _paywayService.CheckPaymentByMd5Async(md5);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "PAYMENT_CHECK_FAILED",
                    message = ex.Message
                });
            }
        }

    }
}