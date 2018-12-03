using NLog;
using System;
using System.Net.Mail;

namespace ToDoApplication.Code
{
    public class EmailManager
    {
        public static readonly string companyEmail = "todoappoficial@mail.ru";//"todoappoficial@gmail.com";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool SendNewPassword(string email, string newPassword)
        {
            try
            {
                MailMessage mail = new MailMessage(companyEmail, email);
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.mail.ru";//"smtp.gmail.com";
                client.EnableSsl = true;

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(companyEmail, "<zQ^`7KR");

                mail.Subject = "Восстановление пароля.";
                mail.Body = $"Ваш новый временный пароль: {newPassword}";
                client.Send(mail);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"SendNewPassword error: {ex}");
                return false;
            }

            return true;
        }
    }
}