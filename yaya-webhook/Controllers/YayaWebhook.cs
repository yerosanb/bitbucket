using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using yaya_webhook.Data;
using yaya_webhook.Model;

namespace yaya_webhook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string? _secretKey;

        public WebhookController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration["SECRET_KEY"];
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook([FromBody] Webhook payload)
        {
            // Extract the YAYA-SIGNATURE header
            var signature = Request.Headers["YAYA-SIGNATURE"].ToString();

            // Prepare the signed payload
            var signedPayload = $"{payload.Id}{payload.Amount}{payload.Currency}{payload.CreatedAtTime}{payload.Timestamp}{payload.Cause}{payload.FullName}{payload.AccountName}{payload.InvoiceUrl}";

            // Check for null secret key
            if (_secretKey == null)
            {
                return BadRequest("Secret key is not configured.");
            }

            // Calculate the expected signature
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey!)))
            {
                var expectedSignature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload))).Replace("-", "").ToLowerInvariant();

                // Verify the signature
                if (!signature.Equals(expectedSignature))
                {
                    return Forbid();
                }
            }

            // Check timestamp to prevent replay attacks
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (Math.Abs(currentTime - payload.Timestamp) > 300) // 5 minutes
            {
                return Forbid();
            }

            // Store the payload in the database
            await _context.Webhooks.AddAsync(payload);
            await _context.SaveChangesAsync();

            return Ok(new { status = "success" });
        }
    }
    }

