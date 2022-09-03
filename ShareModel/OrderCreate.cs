using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModel
{
    public class OrderCreate
    {
        public string OrderName { get; set; }
    }

    public class OrderTiki
    {
        public string OrderName { get; set; }
        public string Source { get; set; } = "Tiki";
    }
}
