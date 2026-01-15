using System;
using System.Collections.Generic;

namespace Stripe_Integration.Models
{
    public partial class TechengineeMisCredential
    {
        public int CredentialId { get; set; }
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public int? UserType { get; set; }
    }
}
