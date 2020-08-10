using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Jobby.Utilities
{
    public static class SecurityUtilities
    {
        public static HttpCookie CreateAuthenticationCookie(string name, Guid id, string userType)
        {
            int timeout = 60;
            string userData = id.ToString() + "," + userType;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddMinutes(timeout), true, userData);
            string encrypted = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                Expires = DateTime.Now.AddMinutes(timeout),
                HttpOnly = true
            };
            return cookie;
        }

        public static Guid GetAuthenticatedUserID()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var list = ticket.UserData.Split(',');
            Guid id = Guid.Parse(list[0]);
            return id;
        }

        public static string GetAuthenticatedUserType()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var list = ticket.UserData.Split(',');
            return list[1];
        }

        public static string Hash(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                );
        }
    }
}