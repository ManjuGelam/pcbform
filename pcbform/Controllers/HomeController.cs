using pcbform.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace pcbform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Forms()
        {
            return View();
        }
        public ActionResult Jdatatable()
        {

            return View();
        }
        public ActionResult loaddata()
        {
            using (JquerytblDataContext dc = new JquerytblDataContext())

            {
                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var data = dc.jquerytbls.OrderBy(a => a.sno).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Captcha()
        {
            return View();
        }
        public string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 6)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }
        public ActionResult CaptchaImage()
        {
            string captchaCode = GenerateRandomString();
            HttpContext.Session["CaptchaCode"] = captchaCode;
            // Create a bitmap image of the captcha code
            Bitmap bitmap = new Bitmap(150, 50, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Gray);
            // Draw the captcha code on the image
            Font font = new Font("Arial", 20, FontStyle.Bold);
            graphics.DrawString(captchaCode, font, new SolidBrush(Color.Black), 10, 10);
            Random random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var x1 = random.Next(0, 200);
                var y1 = random.Next(0, 70);
                var x2 = random.Next(0, 200);
                var y2 = random.Next(0, 70);
                graphics.DrawLine(Pens.SeaGreen, x1, y1, x2, y2);
            }
            // Convert the bitmap image to a byte array and return it as an image
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            byte[] captchaBytes = memoryStream.ToArray();
            return File(captchaBytes, "image/png");
        }
        public ActionResult Password()
        {
            
            return View();
        }
        public ActionResult Index(string userName, string password)
        {
            string constr = ConfigurationManager.ConnectionStrings["UserDbContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO Users VALUES (@Username, @Password)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Username", userName);
                    cmd.Parameters.AddWithValue("@Password", encryption(password));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View(GetUsers());
        }

        private static List<User> GetUsers()
        {
            List<User> users = new List<User>();
            string constr = ConfigurationManager.ConnectionStrings["UserDbContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {

                using (SqlCommand cmd = new SqlCommand("SELECT Username, Password FROM Users"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            users.Add(new User
                            {
                                Username = sdr["Username"].ToString(),
                                EncryptedPassword = sdr["Password"].ToString(),
                                DecryptedPassword = sdr["Password"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return users;
        }


        public string encryption(String password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            //encrypt the given password string into Encrypted data  
            encrypt = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            //Create a new string by using the encrypted data  
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());
            }
            return encryptdata.ToString();
        }
        public string Decrypt(string password, string hash1)
        {
            string source = "01cfcd4f6b8770febfb40cb906715822";// "Hello World!";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(source);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("password"));
            }
            return sb.ToString();
        }
    }

}
