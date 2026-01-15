using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe_Integration.Models;

namespace Stripe_Integration.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly tattsContext _context;

        public HomeController(tattsContext context)
        {
            _context = context;
        }

        private int GetStudentIdFromSession()
        {
            var email = HttpContext.Session.GetString("Email");
           
            var student = _context.TechengineeMisStudents.FirstOrDefault(s => s.Email == email);
            return student.StudentId; 
        }



        public IActionResult Index()
        {
            var subjects = _context.TechengineeMisSubjects.ToList();
            return View(subjects);
        }





        public IActionResult FetchQuestions(int subjectCode)
        {
            var questions = _context.TechengineeMisQuestions.Include(t => t.Subject)
                                .Where(q => q.SubjectCode == subjectCode)
                                .OrderBy(q => q.Position)
                                .ToList();
             ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(questions);
        }





        public IActionResult AnswerView(int questionId)
        {
            int studentId = GetStudentIdFromSession();

            var answer = _context.TechengineeMisAnswers
                .FirstOrDefault(a => a.StudentId == studentId && a.QuestionId == questionId);

            var question = _context.TechengineeMisQuestions.Include(t => t.Subject)
                .FirstOrDefault(q => q.QuestionId == questionId);

            if (answer != null)
            {
                ViewBag.Answer = answer;
                return View("AlreadyAnsweredView", question);
            }
            else
            {
                return View("ProvideAnswerView", question);
            }
        }





        [HttpPost]
        public IActionResult SubmitAnswer(TechengineeMisAnswer answer, int subjectCode)
        {
            int studentId = GetStudentIdFromSession();

            answer.StudentId = studentId;

            _context.TechengineeMisAnswers.Add(answer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Answer submitted successfully.";


            return RedirectToAction("FetchQuestions", new { subjectCode = subjectCode });
        }



        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("Email");
            var credential = _context.TechengineeMisCredentials.FirstOrDefault(t => t.Email == email);

            if (credential == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, credential.Password))
            {
                ViewBag.ErrorMessage = "The current password is incorrect.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "New password and Confirm password do not match.";
                return View();
            }

            string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            credential.Password = hashedNewPassword;
            _context.Update(credential);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully.";

            return RedirectToAction("ChangePassword");
        }



    }




}

