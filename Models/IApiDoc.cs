using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    public interface IApiDoc
    {
        String Name { get; set; }
        String URI { get; set; }
    }
}
