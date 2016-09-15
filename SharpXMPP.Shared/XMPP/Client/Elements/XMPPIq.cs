using System;
using System.Globalization;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class XMPPIq : Stanza
    {
        public enum IqTypes
        {
            get, set, result, error
        }

        public IqTypes IqType 
        { 
            get
            {
                var iqtype = Attribute("type").Value;
                switch (iqtype)
                {
                    case "get":
                        return IqTypes.get;
                    case "set":
                        return IqTypes.set;
                    case "result":
                        return IqTypes.result;
                    default:
                        return IqTypes.error;
                }

            }
            set
            {
                SetAttributeValue("type", value); 
            }
        }
        public XMPPIq(IqTypes type, string id = "") : base(XNamespace.Get(Namespaces.JabberClient) + "iq")
        {
            IqType = type;
            SetAttributeValue("id", string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id);
        }

        public XMPPIq() : base(XNamespace.Get(Namespaces.JabberClient) + "iq") { }

        public string ID
        {
            get
            {
                return Attribute("id").Value;
            }
        }

        public XMPPIq Reply()
        {
            IqType = IqTypes.result;
            var to = Attribute("from").Value;
            SetAttributeValue("from", Attribute("to").Value);
            SetAttributeValue("to", to);
            return this;
        }

        public XMPPIq Error()
        {
            var result = Reply();
            result.IqType = IqTypes.error;
            var error = new XElement(XNamespace.Get(Namespaces.StanzaErrors) + "error", new XElement(XNamespace.Get(Namespaces.StanzaErrors) + "service-unavailable"));
            error.SetAttributeValue("type", "cancel");
            result.Add(error);
            return result;
        }
    }
}
