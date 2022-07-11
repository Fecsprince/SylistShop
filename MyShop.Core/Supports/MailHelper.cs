
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace MyShop.Core.Supports
{
    public class ConfirmEmailSend
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }

    public class MailHelper
    {
        public ConfirmEmailSend SendMail(IdentityMessage message, string html)
        {
            try
            {

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                msg.To.Add(new MailAddress(message.Destination));
                msg.Subject = message.Subject;

                //string text = message.Body;
                //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                //send the message 
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"].ToString());

                //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                NetworkCredential Credentials = new NetworkCredential(
                                                        ConfigurationManager.AppSettings["Email"].ToString(),
                                                        ConfigurationManager.AppSettings["Password"].ToString());

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = Credentials;
                smtp.Port = 8889;    //alternative port number is 8889
                smtp.EnableSsl = false;
                smtp.Timeout = 180000;
                smtp.Send(msg);
                return new ConfirmEmailSend()
                {
                    Message = "Email sent Successfully",
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new ConfirmEmailSend()
                {
                    Message = "WE WERE UNABLE TO REACH YOUR EMAIL PROVIDER!" + " " +
                    ex.Message.ToString(),
                    Result = true
                };
            }

        }
    }
}