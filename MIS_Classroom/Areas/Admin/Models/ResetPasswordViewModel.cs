namespace Stripe_Integration.Areas.Admin.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }
        public int Id { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
