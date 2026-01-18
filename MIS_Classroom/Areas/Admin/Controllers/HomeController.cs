using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe_Integration.Areas.Admin.Models;
using Stripe_Integration.Models;
using System.Net;
using System.Net.Mail;

namespace MIS_Classroom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly tattsContext _context;

        public HomeController(tattsContext context)
        {
            _context = context;
        }

        #region product
        // LIST
        public ActionResult ProductList()
        {
            var products = _context.ManageProducts.ToList();
            return View(products);
        }

        // ADD (GET)
        public ActionResult AddProduct()
        {
            return View();
        }

        // ADD (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(ManageProduct product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    product.ImagePath = "/Uploads/" + fileName;
                }

                product.CreatedOn = DateTime.Now;

                _context.ManageProducts.Add(product);
                _context.SaveChanges();

                return RedirectToAction("ProductList");
            }
            return View(product);
        }


        // EDIT (GET)
        public ActionResult EditProduct(int id)
        {
            var product = _context.ManageProducts.Find(id);
            //if (product == null)
            //    return HttpNotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(ManageProduct product, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(product);

            // Get existing product from DB
            var existingProduct = _context.ManageProducts
                                          .AsNoTracking()
                                          .FirstOrDefault(p => p.ProductID == product.ProductID);

            if (existingProduct == null)
                return NotFound();

            // IF new image uploaded → replace
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                product.ImagePath = "/Uploads/" + fileName;
            }
            else
            {
                // ❗ NO new image → keep old image
                product.ImagePath = existingProduct.ImagePath;
            }

            _context.Update(product);
            _context.SaveChanges();

            return RedirectToAction("ProductList");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.ManageProducts.Find(id);
            if (product != null)
            {
                _context.ManageProducts.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("ProductList");
        }

        #endregion


        #region teacher
        public IActionResult ListTeacher()
        {
            var teachers = _context.TechengineeMisTeachers.Include(t => t.Subject).ToList();
            return View(teachers);
        }

        public IActionResult AddTeacher()
        {
            var subjects = _context.TechengineeMisSubjects.ToList();
            return View(subjects);
        }

        [HttpPost]
        public IActionResult AddTeacher(TechengineeMisTeacher teacher, string password)
        {
            if (_context.TechengineeMisCredentials.Any(c => c.Email == teacher.Email))
            {
                ViewBag.Message = "Email address is already in use!";
                var subjects = _context.TechengineeMisSubjects.ToList();
                return View("AddTeacher", subjects);
            }
            _context.TechengineeMisTeachers.Add(teacher);
            _context.SaveChanges();

            var teacherId = teacher.TeacherId;

            var userType = _context.TechengineeMisUserTypes.FirstOrDefault(u => u.UserType.ToLower() == "teacher")?.TypeId;

            string hashedPassword = HashPassword(password);

            var credential = new TechengineeMisCredential
            {
                Email = teacher.Email,
                Password = hashedPassword,
                UserType = userType
            };

            _context.TechengineeMisCredentials.Add(credential);
            _context.SaveChanges();

            TempData["TeacherAdd"] = "Teacher Added Successfully.";

            return RedirectToAction(nameof(ListTeacher));
        }

        public IActionResult EditTeacher(int? id)
        {
            var teacher = _context.TechengineeMisTeachers
                .Include(t => t.Subject)
                .FirstOrDefault(t => t.TeacherId == id);

            var subjects = _context.TechengineeMisSubjects.ToList();

            ViewBag.Subjects = subjects;
            return View(teacher);
        }

        [HttpPost]
        public IActionResult EditTeacher(int id, TechengineeMisTeacher teacher, string password)
        {
            teacher.TeacherId = id;

            _context.Update(teacher);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(password))
            {
                var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == teacher.Email);
                if (credential != null)
                {
                    credential.Password = password;
                    _context.Update(credential);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(ListTeacher));
        }

        public IActionResult DeleteTeacher(int? id)
        {
            var teacher = _context.TechengineeMisTeachers.FirstOrDefault(t => t.TeacherId == id);
            return View(teacher);
        }

        [HttpPost, ActionName("DeleteTeacher")]
        public IActionResult DeleteTeacherConfirm(int id)
        {
            var teacher = _context.TechengineeMisTeachers.FirstOrDefault(t => t.TeacherId == id);
            var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == teacher.Email);

            _context.TechengineeMisCredentials.Remove(credential);
            _context.TechengineeMisTeachers.Remove(teacher);

            _context.SaveChanges();

            return RedirectToAction(nameof(ListTeacher));
        }

        #endregion

        #region student
        // For Student

        public IActionResult ListStudent()
        {
            var students = _context.TechengineeMisStudents.ToList();
            return View(students);
        }

        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(TechengineeMisStudent student, string password)
        {
            if (_context.TechengineeMisCredentials.Any(c => c.Email == student.Email))
            {
                ViewBag.Message = "Email address is already in use!";
                var subjects = _context.TechengineeMisSubjects.ToList();
                return View("AddStudent");
            }
            _context.TechengineeMisStudents.Add(student);
            _context.SaveChanges();

            var studentId = student.StudentId;

            var userType = _context.TechengineeMisUserTypes.FirstOrDefault(u => u.UserType.ToLower() == "student")?.TypeId;

            string hashedPassword = HashPassword(password);

            var credential = new TechengineeMisCredential
            {
                Email = student.Email,
                Password = hashedPassword,
                UserType = userType
            };

            _context.TechengineeMisCredentials.Add(credential);
            _context.SaveChanges();

            TempData["StudentAdd"] = "Student Added Successfully.";

            return RedirectToAction(nameof(ListStudent));
        }

        public IActionResult EditStudent(int? id)
        {
            var student = _context.TechengineeMisStudents.FirstOrDefault(s => s.StudentId == id);
            return View(student);
        }

        [HttpPost]
        public IActionResult EditStudent(int id, TechengineeMisStudent student, string password)
        {
            student.StudentId = id;


            _context.Update(student);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(password))
            {
                var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == student.Email);
                if (credential != null)
                {
                    credential.Password = password;
                    _context.Update(credential);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(ListStudent));
        }

        public IActionResult DeleteStudent(int? id)
        {
            var student = _context.TechengineeMisStudents.FirstOrDefault(s => s.StudentId == id);
            return View(student);
        }

        [HttpPost, ActionName("DeleteStudent")]
        public IActionResult DeleteStudentConfirm(int id)
        {
            var student = _context.TechengineeMisStudents.FirstOrDefault(s => s.StudentId == id);
            var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == student.Email);

            _context.TechengineeMisCredentials.Remove(credential);
            _context.TechengineeMisStudents.Remove(student);

            _context.SaveChanges();

            return RedirectToAction(nameof(ListStudent));
        }

        #endregion

        #region setting

        public IActionResult ResetStudentPassword(string email)
        {
            var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == email);

            const string newPassword = "123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);


            credential.Password = hashedPassword;
            _context.Update(credential);
            _context.SaveChanges();

            return RedirectToAction(nameof(ListStudent));
        }

        public IActionResult ResetTeacherPassword(string email)
        {
            var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == email);



            const string newPassword = "123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);


            credential.Password = hashedPassword;
            _context.Update(credential);
            _context.SaveChanges();


            return RedirectToAction(nameof(ListTeacher));
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }





        public IActionResult Index()
        {

            return View();
        }


        [HttpGet]
        public IActionResult SendPasswordResetEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var student = _context.TechengineeMisStudents.FirstOrDefault(s => s.Email == email);
            if (student == null)
            {
                return NotFound("Student with the provided email not found.");
            }

            string token = Guid.NewGuid().ToString();
            TempData["ResetToken"] = token;
            int id = student.StudentId;

            SendResetEmail(id, email, token);

            TempData["ResetEmailSent"] = "Password reset email has been sent.";

            /*return RedirectToAction("EditStudent", new { id = student.StudentId });*/
            return RedirectToAction(nameof(ListStudent));
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, int id)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token.");
            }

            var model = new ResetPasswordViewModel { Token = token, Id = id };
            return View("ResetPassword", model);
        }

        [HttpPost]
        public IActionResult ResetPassword(int id, string newPassword, string confirmPassword)
        {
            var student = _context.TechengineeMisStudents.FirstOrDefault(s => s.StudentId == id);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "New password and confirm password do not match.");
                return View("EditStudent", student);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword); ;
            var credential = _context.TechengineeMisCredentials.FirstOrDefault(c => c.Email == student.Email);
            if (credential == null)
            {
                return NotFound("Credential not found.");
            }

            credential.Password = hashedPassword;
            _context.Update(credential);
            _context.SaveChanges();

            TempData["PasswordResetSuccess"] = "Password has been reset successfully.";
            return RedirectToAction("Login", "Home", new { area = "" });


        }



        private void SendResetEmail(int id, string email, string token)
        {
            string smtpServer = "smtp.gmail.com";
            int port = 587;
            string senderEmail = "atul.baral8421@gmail.com";
            string senderPassword = "nemm arey koqy bmvm\r\n";


            using (SmtpClient client = new SmtpClient(smtpServer, port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.EnableSsl = true;


                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(senderEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = "Password Reset";
                mailMessage.Body = $"Click the following link to reset your password: https://localhost:7173/Admin/Home/ResetPassword?id={id}&token={token}";

                // Send email
                client.Send(mailMessage);
            }
        }
        #endregion
    }

}


