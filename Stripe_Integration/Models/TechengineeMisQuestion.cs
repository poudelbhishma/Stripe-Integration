using System;
using System.Collections.Generic;

namespace Stripe_Integration.Models
{
    public partial class TechengineeMisQuestion
    {
        public int QuestionId { get; set; }
        public int? SubjectCode { get; set; }
        public string? QuestionsTxt { get; set; }
        public int? Position { get; set; }

        // Navigation property
        public TechengineeMisSubject Subject { get; set; }
    }
}
