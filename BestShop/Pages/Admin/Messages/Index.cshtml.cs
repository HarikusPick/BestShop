using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
//Data Source=localhost\\SQLEXPRESS01;Initial Catalog=bestshop;Integrated Security=True
namespace BestShop.Pages.Admin.Messages
{
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<MessageInfo> listMessages = new List<MessageInfo>();
        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // each html page shows pageSize messages



        public void OnGet()
        {
            page = 1; //Ýlk olarak, page deðiþkeni 1 olarak ayarlanýr. Bu, varsayýlan olarak görüntülenecek HTML sayfasýnýn numarasýný temsil eder.
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage); //Request.Query["page"] ifadesi kullanýlarak URL'de "page" parametresi kontrol edilir. Eðer bu parametre mevcutsa, sayfa numarasý alýnýr.
                }
                catch (Exception ex)
                {
                    page = 1; //Ardýndan, sayfa numarasý int.Parse() metoduyla tamsayýya dönüþtürülür. Eðer dönüþtürme iþlemi hata verirse, sayfa numarasý 1 olarak ayarlanýr.
                }
            }

            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=bestshop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM messages";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM messages ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    /*
                     Sayfalama iþlemi için OFFSET ve FETCH NEXT ifadeleri kullanýlýr. @skip ve 
                    @pageSize parametreleri, belirli bir sayfada 
                    kaç kaydýn atlanacaðýný ve kaç kaydýn alýnacaðýný belirler.
                     */

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MessageInfo messageInfo = new MessageInfo();
                                messageInfo.Id = reader.GetInt32(0);
                                messageInfo.FirstName = reader.GetString(1);
                                messageInfo.LastName = reader.GetString(2);
                                messageInfo.Email = reader.GetString(3);
                                messageInfo.Phone = reader.GetString(4);
                                messageInfo.Subject = reader.GetString(5);
                                messageInfo.Message = reader.GetString(6);
                                messageInfo.CreatedAt = reader.GetDateTime(7).ToString("MM/dd/yyyy");

                                listMessages.Add(messageInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class MessageInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
