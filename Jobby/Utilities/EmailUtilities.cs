using Jobby.Models;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Jobby.Utilities
{
    public static class EmailUtilities
    {
        public static bool IsEmailExists(string email)
        {
            using (JobbyEntities db = new JobbyEntities())
            {
                var exist = db.Users.Where(user => user.Email == email).FirstOrDefault();
                return exist != null;
            }
        }

        public static void SendActivationLinkEmail(string Email, string activationCode, string EmailPurpose = "VerifyAccount")
        {
            //defining email info
            var fromEmail = new MailAddress("websitejobby@gmail.com", "Jobby");
            var fromEmailPW = "jobby@1234";
            var toEmail = new MailAddress(Email);

            string subject = "";
            string body = "";
            if (EmailPurpose == "VerifyAccount")
            {
                subject = "Your Jobby Account Successfully Created";
                body = "<br/>We are excited to inform you that your Jobby Account has been successfully created.<br/>" +
                           "Your activation code is : <strong>" + activationCode + "</strong><br/>" +
                           "Please don't share this code with others as this code keeps your account protected";

            }
            else if (EmailPurpose == "ResetPassword")
            {
                subject = "Reset Your Jobby Account Password";
                body = "<br/>Use the code below to reset your jobby account password.<br/>" +
                           "Your code is : <strong>" + activationCode + "</strong><br/>" +
                           "Please don't share this code with others as this code keeps your account protected";

            }
            else if (EmailPurpose == "ResendCode")
            {
                subject = "Your Jobby Account Activation Code";
                body = "<br/>Use the code below to active your jobby account.<br/>" +
                           "Your code is : <strong>" + activationCode + "</strong><br/>" +
                           "Please don't share this code with others as this code keeps your account protected";
            }

            //sending email
            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPW)
            })
            using (var msg = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(msg);
        }
    }
}