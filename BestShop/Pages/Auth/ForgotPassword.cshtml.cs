using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Auth
{
    [RequireNoAuth]
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "The email is required"), EmailAddress]
        public string Email { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data  validation failed";
                return;
            }

            //1- create token   2-save token in database  3-send token by email to user
            try
            {
                string connectionString="Data Source=localhost\\SQLEXPRESS01;Initial Catalog=bestshop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM users WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) //bu kullan�c� varsa demek. (email varsa)
                            {
                                string firstname = reader.GetString(1);
                                string lastname = reader.GetString(2);

                                string token = Guid.NewGuid().ToString(); //unique token

                                //tokeni db ye kaydetmek
                                SaveToken(Email, token);

                                //send the token by email to the user
                                string resetUrl = Url.PageLink("/Auth/ResetPassword") + "?token=" + token;
                                string username = firstname + " " + lastname;
                                string subject = "Password Reset";
                                string message = "Dear " + username + ",\n\n" +
                                    "You can reset your password using the following link:\n\n" +
                                    resetUrl + "\n\n" +
                                    "Best Regards";

                                EmailSender.SendEmail(Email, username, subject, message).Wait();
                            }
                            else
                            {
                                errorMessage = "we dont have no user with this email address";
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            successMessage = "plase check your email and click on the reset password link";

        }

        private void SaveToken(string email, string token)
        {
            try
            {
                string connectionString="Data Source=localhost\\SQLEXPRESS01;Initial Catalog=bestshop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //delete any oldu token for this email address from database
                    string sql = "DELETE FROM password_resets WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }

                    //add token to database
                    sql = "INSERT INTO password_resets (email, token) VALUES (@email, @token)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email",email);
                        command.Parameters.AddWithValue("@token",token);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
