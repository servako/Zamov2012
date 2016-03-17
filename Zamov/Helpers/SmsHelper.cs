using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Zamov.srSms;

namespace Zamov.Helpers
{
    public static class SmsHelper
    {
        public static bool SendMessage(string from, contact[] to, string body)
        {
            bool result = true;
            var user = new user { userName = "Zamov", password = "Z@m0v_45er302" };
            
            var notification = new notification {alfaName = from, contacts = to, user = user, template = body};

            var responseMessageSMS = new responseMessage();

            var serviceDelegateClient = new NotificatorServiceDelegateClient();
            try
            {
                responseMessageSMS = serviceDelegateClient.sendMessages(notification);
                result = (responseMessageSMS.notificationResponse == 0);
            }
            catch (Exception)
            {
                result = false;
            }

            if (!result)
            {
                result = SendMessage(from, to, body, "SMS", false);
            }
            return result;
        }

        public static bool SendMessage(string from, contact[] to, string body, string subject, bool isBodyHtml)
        {
            var client = new SmtpClient();
            bool result = true;
            try
            {
                var message = new MailMessage();
                message.Body = body;
                message.Subject = subject;
                for (int i = 0; i < to.Count(); i++)
                {
                    message.To.Add(String.Format("{0}@Zamov.mobisoftline.com.ua", to[i].phone));
                }
                message.From = new MailAddress("no-reply@zamov.ua");
                message.IsBodyHtml = isBodyHtml;
                message.BodyEncoding = Encoding.GetEncoding("windows-1251");
                message.Headers.Add("Content-Transfer-Encoding", "8bit");
                client.Send(message);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool SendTemplate(string from, srSms.contact[] to, string template, string language, params object[] replacements)
        {
            string languageFolder = (string.IsNullOrEmpty(language)) ? string.Empty : language + "/";
            string filePath = HttpContext.Current.Server.MapPath("~/Content/SmsTemplates/" + languageFolder + template);
            var file = new FileStream(filePath, FileMode.Open);
            var reader = new StreamReader(file);
            string body = reader.ReadToEnd();
            string formattedBody = (replacements != null && replacements.Length > 0) ? string.Format(body, replacements) : body;
            reader.Close();
            return SendMessage(from, to, formattedBody);
        }
    }
}
