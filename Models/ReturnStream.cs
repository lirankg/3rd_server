using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ReturnStream
    {
        public resultStatus messageStatus { get; set; }
        public messageType messageType { get; set; }
        public object data { get; set; }
    }


}
