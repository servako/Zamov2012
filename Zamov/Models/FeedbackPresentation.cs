using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class FeedbackPresentation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public string FirstName { get; set; }
        public DateTime Date { get; set; }
    }
}
