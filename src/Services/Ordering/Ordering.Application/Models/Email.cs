using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Models
{
    public class Email
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public List<string> CC { get; set; }
        public string Body { get; set; }
    }
}
