using System.Diagnostics;
using System.Data;
using BOOTSTRAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BOOTSTRAP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromBody] User loginModel)
        {
            User found = new User(null, loginModel.Email, loginModel.Password);
            string loginQuery = found.GenerateLoginQuery("Users");

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(loginQuery, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string fullName = reader.GetString(reader.GetOrdinal("FullName"));

                            HttpContext.Session.SetInt32("UserId", id);
                            HttpContext.Session.SetString("FullName", fullName);

                            return Json(new { success = true });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            return Json(new { success = false, message = "Invalid email or password." });
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            User user = new User(model.FullName, model.Email, model.Password);
            string insertQuery = user.GenerateInsertQuery("Users");
            string selectQuery = user.GenerateSelectQuery("Users", "Id");
            string updateQuery = user.GenerateUpdateQuery("Users", "Id");
            string deleteQuery = user.GenerateDeleteQuery("Users", "Id");
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            return Json(new
            {
                insert = insertQuery,
                select = selectQuery,
                update = updateQuery,
                delete = deleteQuery
            });
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            List<User> users = new List<User>();
            string selectAllQuery = BOOTSTRAP.Models.User.GenerateSelectAllQuery("Users");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(selectAllQuery, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User(
                            reader.GetString(reader.GetOrdinal("FullName")),
                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.GetString(reader.GetOrdinal("Password")))
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id"))
                        });
                    }
                }
            }

            ViewData["LoggedInUser"] = HttpContext.Session.GetString("FullName");
            return View(users);
        }

        [HttpPost]
        public IActionResult EditUser([FromBody] EditUserViewModel model)
        {
            User user = new User(model.FullName, model.Email, model.Password) { Id = model.Id };
            string updateQuery = user.GenerateUpdateQuery("Users", "Id");

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            return Json(new { success = true, query = updateQuery });
        }

        [HttpPost]
        public IActionResult DeleteUser([FromBody] IdOnlyModel model)
        {
            User user = new User(null, null, null) { Id = model.Id };
            string deleteQuery = user.GenerateDeleteQuery("Users", "Id");

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            return Json(new { success = true, query = deleteQuery });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    public class IdOnlyModel
    {
        public int Id { get; set; }
    }
}