using System;
using System.Globalization;
using System.Xml.Linq;

namespace SharpXMPP.Client
{
    public class Iq : XElement
    {
        public enum IqType
        {
            Get, Set, Result, Error
        }
        public Iq(IqType type, string id = "") : base("iq")
        {
           SetAttributeValue("type", type.ToString("g").ToLower()); 
           SetAttributeValue("id", string.IsNullOrEmpty(id) ? DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) : id);
        }
    }
}
