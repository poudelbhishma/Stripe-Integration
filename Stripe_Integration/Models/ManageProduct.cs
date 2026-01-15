namespace Stripe_Integration.Models
{
    public partial class ManageProduct
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? Descriptions { get; set; }
        public int? Quantity { get; set; }
        public string? Price { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
