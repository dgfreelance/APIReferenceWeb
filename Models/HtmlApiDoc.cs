using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models
{
    public class HtmlApiDoc : IApiDoc
    {
        public String Name { get; set; }
        public String URI { get; set; }
    }
}