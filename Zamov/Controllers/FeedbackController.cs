using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Models;
using System.Web.Security;
using System.Data;

namespace Zamov.Controllers
{
    public class FeedbackController : Controller
    {
        //
        // GET: /Feedback/

        private List<FeedbackPresentation> GetFeedbacks(int dealerId)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                List<FeedbackPresentation> feedbacks = (from feedback in context.DealerFeedback.Include("Dealer")
                                                        where feedback.Dealer.Id == dealerId
                                                        orderby feedback.Date descending
                                                        select new FeedbackPresentation
                                                        {
                                                            Text = feedback.Text,
                                                            Email = feedback.Email,
                                                            FirstName = feedback.FirstName,
                                                            Id = feedback.Id,
                                                            Date = feedback.Date

                                                        }).ToList();
                return feedbacks;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult Index(int id)
        {
            List<FeedbackPresentation> feedbacks = GetFeedbacks(id);
            ViewData["dealerId"] = id;
            return View(feedbacks);
        }

        [Authorize(Roles = "Administrators")]
        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult ModifyFeedback(int id)
        {
            List<FeedbackPresentation> feedbacks = GetFeedbacks(id);
            ViewData["dealerId"] = id;
            return View(feedbacks);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteFeedback(int dealerId, int feedbackId)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                DealerFeedback feedback = context.DealerFeedback.Select(df => df).Where(df => df.Id == feedbackId).First();
                context.DeleteObject(feedback);
                context.SaveChanges();
            }
            return RedirectToAction("ModifyFeedback", "Feedback", new { id = dealerId });
        }

        [Authorize(Roles = "Administrators, Customers, Dealers")]
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult CreateFeedback(int id)
        {
            ViewData["dealerId"] = id;
            return View();
        }

        [Authorize(Roles = "Administrators, Customers, Dealers")]
        [CaptchaValidation("captcha")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateFeedback(int dealerId, string text, bool captchaValid)
        {
            if (!captchaValid)
                return Json(false);
            else
            {
                DealerFeedback feedback = new DealerFeedback();
                ProfileCommon profile = ProfileCommon.Create(User.Identity.Name);
                MembershipUser user = Membership.GetUser();
                feedback.Text = text;
                feedback.UserId = (Guid)user.ProviderUserKey;
                feedback.FirstName = profile.FirstName;
                feedback.Email = user.Email;
                feedback.Date = DateTime.Now;
                List<EntityKeyMember> list = new List<EntityKeyMember>();
                EntityKeyMember member = new EntityKeyMember { Key = "Id", Value = dealerId };
                list.Add(member);
                EntityKey dealer = new EntityKey("ZamovStorage.Dealers", list);
                feedback.DealerReference.EntityKey = dealer;
                using (ZamovStorage context = new ZamovStorage())
                {
                    context.AddToDealerFeedback(feedback);
                    context.SaveChanges();
                }
                return Json(true);
            }
        }

        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult FeedbackProductsAdditional(int prodId)
        {
            ViewData["productId"] = prodId;
            List<FeedbackPresentation> feedbacks = GetFeedbacksProductAdditional(prodId);
            return View(feedbacks);
        }

        [Authorize(Roles = "Administrators, Customers, Dealers")]
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult CreateFeedbackProductAdditional(int prodId)
        {
            ViewData["productId"] = prodId;
            return View();
        }

        [Authorize(Roles = "Administrators, Customers, Dealers")]
        [CaptchaValidation("captcha")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateFeedbackProductAdditional(int productId, string text, bool captchaValid)
        {
            if (!captchaValid)
                return Json(false);
            else
            {
                var feedback = new ProductsAdditionalFeedback();
                ProfileCommon profile = ProfileCommon.Create(User.Identity.Name);
                MembershipUser user = Membership.GetUser();
                feedback.Text = text;
                feedback.UserId = (Guid)user.ProviderUserKey;
                feedback.FirstName = profile.FirstName;
                feedback.Email = user.Email;
                feedback.Date = DateTime.Now;
                feedback.ProductId = productId;
                using (ZamovStorage context = new ZamovStorage())
                {
                    context.AddToProductsAdditionalFeedback(feedback);
                    context.SaveChanges();
                }
                return Json(true);
            }
        }

        [Authorize(Roles = "Administrators")]
        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult ModifyFeedbackProductAdditional(int productId)
        {
            List<FeedbackPresentation> feedbacks = GetFeedbacksProductAdditional(productId);
            ViewData["productId"] = productId;
            return View(feedbacks);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteFeedbackProductAdditional(int productId, int feedbackId)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                ProductsAdditionalFeedback feedback = context.ProductsAdditionalFeedback.Select(f => f).Where(f => f.Id == feedbackId).First();
                context.DeleteObject(feedback);
                context.SaveChanges();
            }
            return RedirectToAction("ModifyFeedbackProductAdditional", "Feedback", new { productId = productId });
        }

        private List<FeedbackPresentation> GetFeedbacksProductAdditional(int productId)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                List<FeedbackPresentation> feedbacks = (from feedback in context.ProductsAdditionalFeedback
                                                        where feedback.ProductId == productId
                                                        orderby feedback.Date descending
                                                        select new FeedbackPresentation
                                                        {
                                                            Text = feedback.Text,
                                                            Email = feedback.Email,
                                                            FirstName = feedback.FirstName,
                                                            Id = feedback.Id,
                                                            Date = feedback.Date

                                                        }).ToList();
                return feedbacks;
            }
        }
    }
}
