using SendGrid;
using SendGrid.Helpers.Mail;

namespace BestShop.MyHelpers
{
    public class EmailSender
    {
        /*
         Asenkron programlama, çağıran iş parçacığının yürütülmesini engellemeden kodun eşzamanlı olarak yürütülmesine
        izin veren bir programlama tekniğidir. Eşzamansız programlama, programların bu işlemleri çağıran iş parçacığını 
        engellemeden gerçekleştirmesine olanak tanır. Eşzamansız bir işlem başlatıldığında, program işlemin tamamlanmasını 
        beklerken diğer kodu yürütmeye devam eder. C#'ta "async" ve "await" anahtar sözcükleri, eşzamanlı koda benzeyen 
        eşzamansız kod yazmak için uygun bir yol sağlar ve okumayı ve bakımı kolaylaştırır. Eşzamanlı programlamada,
        ilk Yöntem1'i yürütür, bu yöntemin tamamlanmasını bekler ve ardından Yöntem2'yi yürütür. Ayrıca, daha fazla örnek 
        göreceğiz ve herhangi bir üçüncü Yöntem, Yöntem3'ün yöntem1'e bağımlılığı olduğu için, wait anahtar sözcüğü yardımıyla 
        Yöntem1'in tamamlanmasını bekleyecektir.
         */
        public static async Task SendEmail(string toEmail, string username, string subject, string message)
        {
            string apiKey = "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";
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress ("yunusfurkanozer@gmail.com", "BestShop.com");
            var to = new EmailAddress (toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";

            var msg = MailHelper.CreateSingleEmail(
                from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
