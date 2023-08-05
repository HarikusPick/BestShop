using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages
{
    public class ContactModel : PageModel
    {
        public void OnGet()
        {

        }

        [BindProperty, Required(ErrorMessage = "The First Name is  Required")]
        [Display(Name = "First Name*")]
        public string FirstName { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Last Name is  Required")]
        [Display(Name = "Last Name*")]
        public string LastName { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Email is  Required"), EmailAddress]
        [Display(Name = "Email*")]
        public string Email { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; } = "";

        [BindProperty, Required]
        [Display(Name = "Subject*")]
        public string Subject { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Message is  Required")]
        [MinLength(5, ErrorMessage = "The message should be at least 5 chracters")]
        [MaxLength(1024, ErrorMessage = "The message should be at less than 1024 chracters")]
        [Display(Name = "Message*")]
        public string Message { get; set; } = "";


        public List<SelectListItem> SubjectList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Order Status", Text = "Order Status"},
            new SelectListItem { Value = "Refund Request", Text = "Refund Request"},
            new SelectListItem { Value = "Job Application", Text = "Job Application"},
            new SelectListItem { Value = "Other", Text = "Other"},
        };



        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";



        public void OnPost()
        {
            //gerekli alanlar boþ mu diye kontrol ediyoruz
            if (!ModelState.IsValid)
            {
                // error
                ErrorMessage = "Please fill all required fields";
                return;
            }

            if (Phone == null) Phone = "";

            //  add this message to the database

            try {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=bestshop;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO messages " +
                        "(firstname, lastname, email, phone, subject, message) VALUES " +
                        "(@firstname, @lastname, @email, @phone, @subject, @message);";
                    
                    using (SqlCommand command = new SqlCommand (sql, connection))
                    {
                        
                        command.Parameters.AddWithValue("@firstname", FirstName);
                        command.Parameters.AddWithValue("@lastname", LastName);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@subject", Subject);
                        command.Parameters.AddWithValue("@message", Message);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch ( Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            // send confirmation email to the client
            string username = FirstName + " " + LastName;
            string emailSubject = "About your message";
            string emailMessage = "Dear " + username + ",\n" + 
                "We received your message. Thank you for contacting us. \n" +
                "Our team will contact you very soon.\n" +
                "Best Regards\n\n" +
                "Your message:\n" + Message;

            EmailSender.SendEmail(Email, username, emailSubject, emailMessage).Wait();


            SuccessMessage = "Your message has been received corretly";

            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";

            ModelState.Clear();
        }
    }
}
