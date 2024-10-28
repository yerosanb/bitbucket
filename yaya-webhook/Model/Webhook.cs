using System.ComponentModel.DataAnnotations;

namespace yaya_webhook.Model
{
    public class Webhook
    {
        [Key]
        public string Id { get; set; } = "";
        public int Amount { get; set; }
        public string Currency { get; set; } = "";
        public long CreatedAtTime { get; set; }
        public long Timestamp { get; set; }
        public string Cause { get; set; } = "";
        public string FullName { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string InvoiceUrl { get; set; } = "";
    }
}
