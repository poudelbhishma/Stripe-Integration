
using Stripe_Integration.Models;

namespace Stripe_Integration.Areas.Teacher.Models
{
    public class AddQuestionViewModel
    {
        public List<TechengineeMisSubject> Subjects { get; set; }
        public List<TechengineeMisQuestion> Questions { get; set; }
        public int SubjectId { get; set; }
    }
}
