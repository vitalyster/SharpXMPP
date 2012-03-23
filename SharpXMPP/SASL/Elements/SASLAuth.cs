using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.SASL.Elements
{
    public class SASLAuth : XElement
    {
        public SASLAuth() : base(XNamespace.Get(Namespaces.XmppSasl) + "auth")
        {
        }
    }
}
