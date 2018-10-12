using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using System.Web.Mvc;
using System.Net.Mail;
using AWDC.Models;


namespace AWDC.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        public const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Contact/";

        public ActionResult RenderForm()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_Contact.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ContactModels model)
        {
            if (ModelState.IsValid)
            {
                SendEmail(model);
                TempData["success"] = true;
                return RedirectToCurrentUmbracoPage();
            } else
            {
                return CurrentUmbracoPage();
            }
        }

        private void SendEmail(ContactModels model)
        {
            MailMessage message = new MailMessage();
            message.To.Add("helmuth.norbert@gmail.com");
            message.Subject = model.Subject;
            message.From = new MailAddress(model.Email, model.Name);
            message.Body = model.Message + "\n my email is: " + model.Email;

            SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network; client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential("helmuth.norbert@gmail.com", "Ga");
                client.EnableSsl = true;

                client.Send(message);

            IContent comment = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "comment");
            comment.SetValue("email", model.Email);
            comment.SetValue("cname", model.Name);
            comment.SetValue("subject", model.Subject);
            comment.SetValue("message", model.Message);
            Services.ContentService.Save(comment);
        }
    }
}