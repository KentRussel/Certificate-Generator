using System;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace Monday_Tradeskola.Handlers
{
    public class MondayWebhookHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            string body = String.Empty;
            context.Request.InputStream.Position = 0;

            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    body = inputStream.ReadToEnd();
                }

                context.Response.Write(body);

                SendMail("<br><br>body : " + body);

                context.Response.StatusCode = 200;
            }
            catch (HttpException ex)
            {
                context.Response.StatusCode = ex.GetHttpCode();

                SendMail("\nError: " + ex.Message + "\n\n\n<br><br>" + body);
            }
            finally
            {
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        private void SendMail(string strBody)
        {
            string FromEmail = "dbtird.intern4@gmail.com";
            string ToEmail = "kentpayumo16@gmail.com";

            MailAddress from = new MailAddress(FromEmail);
            MailAddress to = new MailAddress(ToEmail);
            MailMessage objMail = new MailMessage(from, to);
            objMail.IsBodyHtml = true;

            objMail.Subject = "Monday Certification";
            objMail.Body = strBody;

            SmtpClient client = new SmtpClient();

            client.Host = "your_smtp_server";

            client.Send(objMail);
            objMail.Dispose();
        }
    }
}
