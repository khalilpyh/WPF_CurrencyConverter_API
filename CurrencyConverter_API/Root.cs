using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_API
{
    /// <summary>
    /// The main class.
    /// </summary>
    public class Root
    {
        //properties
        public Rate rates { get; set; }         //get all rate records and set in Rate class as currency name
        public long timestamp;
        public string license;
    }
}
