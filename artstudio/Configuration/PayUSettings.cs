using System;
using System.Collections.Generic;

namespace artstudio.Configuration
{
    public class PayUSettings
    {
        public string MerchantId { get; set; } = null!;
        public string AccountId { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ResponseUrl { get; set; } = null!;
        public string ConfirmationUrl { get; set; } = null!;
        public bool TestMode { get; set; } = false;
        public string SandboxUrl { get; set; } = null!;
        public string ProductionUrl { get; set; } = null!;

    }
}
