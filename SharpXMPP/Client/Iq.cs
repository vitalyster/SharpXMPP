using System;
using System.Globalization;
using System.Xml.Linq;

namespace SharpXMPP.Client
{
    public class Iq : XElement
    {
        public enum IqTypes
        {
            Get, Set, Result, Error
        }

        private IqTypes _type;

        public IqTypes IqType 
        { 
            get { return _type; }
            set
            {
                _type = value;
                SetAttributeValue("type", value.ToString("g").ToLower()); 
            }
        }
        public Iq(IqTypes type, string id = "") : base("iq")
        {
            IqType = type;
            SetAttributeValue("id", string.IsNullOrEmpty(id) ? DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) : id);
        }
    }
}
