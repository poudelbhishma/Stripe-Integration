using Stripe_Integration.Models;

namespace Stripe_Integration.Areas.Student.Models
{
    public class ViewQuestionsViewModel
    {
        public List<TechengineeMisSubject> Subjects { get; set; }
        public List<TechengineeMisQuestion> Questions { get; set; }
    }
}
