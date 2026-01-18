using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe_Integration.Models;

public class CheckoutController : Controller
{
    private readonly tattsContext _context;
    private readonly IConfiguration _config;

    public CheckoutController(tattsContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // Checkout page
    public IActionResult Index(int id)
    {
        var product = _context.ManageProducts.Find(id);
        if (product == null) return NotFound();

        return View(product);
    }

    // Stripe Checkout
    [HttpPost]
    public IActionResult CreateCheckoutSession(int productId)
    {
        var product = _context.ManageProducts.Find(productId);
        if (product == null) return NotFound();
        decimal price = decimal.Parse(product.Price);




        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "npr",
                        UnitAmount = (long)(price * 100),

                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.ProductName,
                            Description = product.Descriptions
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = Url.Action("Success", "Checkout", null, Request.Scheme),
            CancelUrl = Url.Action("Cancel", "Checkout", null, Request.Scheme)
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Redirect(session.Url);
    }

    public IActionResult Success()
    {
        return View();
    }

    public IActionResult Cancel()
    {
        return View();
    }
}
