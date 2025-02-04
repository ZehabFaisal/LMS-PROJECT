//using LMS_PROJECT.Models;
//using System;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.Web.Mvc;

//namespace LMS_PROJECT.Controllers
//{
//    public class AccountController : Controller
//    {
//        // GET: SignUp Page
//        public ActionResult SignUp()
//        {
//            // Log all connection strings in the web.config for debugging purposes
//            foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
//            {
//                Debug.WriteLine($"Name: {conn.Name}, Connection String: {conn.ConnectionString}");
//            }

//            return View();
//        }

//        // POST: Handle SignUp Form Submission
//        [HttpPost]
//        public ActionResult SignUp(User user)
//        {
//            if (ModelState.IsValid)
//            {
//                foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
//                {
//                    Debug.WriteLine($"Name: {conn.Name}, Connection String: {conn.ConnectionString}");
//                }

//                string connectionString = ConfigurationManager.ConnectionStrings["UserDb"].ConnectionString;
//                if (string.IsNullOrEmpty(connectionString))
//                {
//                    ViewBag.Message = "Error: Connection string is empty.";
//                    return View();
//                }

//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    try
//                    {
//                        con.Open();
//                        string query = "INSERT INTO Users (FirstName, LastName, Email, Password) VALUES (@FirstName, @LastName, @Email, @Password)";

//                        using (SqlCommand cmd = new SqlCommand(query, con))
//                        {
//                            string hashedPassword = HashPassword(user.Password);

//                            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
//                            cmd.Parameters.AddWithValue("@LastName", user.LastName);
//                            cmd.Parameters.AddWithValue("@Email", user.Email);
//                            cmd.Parameters.AddWithValue("@Password", hashedPassword);

//                            int rowsAffected = cmd.ExecuteNonQuery();

//                            if (rowsAffected > 0)
//                            {
//                                ViewBag.Message = "User registered successfully!";
//                            }
//                            else
//                            {
//                                ViewBag.Message = "Error: Unable to register user.";
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        ViewBag.Message = "Error: " + ex.Message;
//                    }
//                }
//            }
//            return View();
//        }
//        // Function to hash the password using SHA256
//        private string HashPassword(string password)
//        {
//            using (var sha256 = System.Security.Cryptography.SHA256.Create())
//            {
//                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
//                var hash = sha256.ComputeHash(bytes);
//                return Convert.ToBase64String(hash);
//            }
//        }
//    }
//}

using Dapper;
using LMS_PROJECT.Models;
using System;
using System.Web.Mvc;


namespace LMS_PROJECT.Controllers
{
    public class AccountController : Controller
    {
        DapperHelper db;

        public AccountController()
        {
            db = new DapperHelper();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var model = db._connection.QueryFirstOrDefault<User>("Select * from UserTable where Email = @Username and password = @Password", new { Username = username, Password = password });
            if (model != null)
            {
                Session["CurrentUser"] = model;
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewBag.Error = "Invalid username or password!";
                return View();
            }

        }

        public ActionResult Logout()
        {
            if (Session["CurrentUser"] != null)
            {
                Session.Clear();
                Session.Abandon();
            }
            return RedirectToAction("Login");
        }

        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User u, string iagree)
        {
            try
            {
                int rowsAffected = db._connection.Execute("INSERT INTO UserTable(FirstName, LastName, Email, Password) VALUES(@FirstName, @LastName, @Email, @Password)", u);
                if (rowsAffected > 0)
                {
                    ViewBag.Message = "Signup successful!";
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ViewBag.Message = "Signup failed!";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }
            return View();

        }
    }
}
