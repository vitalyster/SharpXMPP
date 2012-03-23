using System;
using System.Globalization;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class Iq : XElement
    {
        public enum IqTypes
        {
            get, set, result, error
        }

        private IqTypes _type;

        public IqTypes IqType 
        { 
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                SetAttributeValue("type", value); 
            }
        }
        public Iq(IqTypes type, string id = "") : base(XNamespace.Get(Namespaces.JabberClient) + "iq")
        {
            IqType = type;
            SetAttributeValue("id", string.IsNullOrEmpty(id) ? DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) : id);
        }

        public Iq() : base(XNamespace.Get(Namespaces.JabberClient) + "iq") { }

        public Iq(XElement element) : this((IqTypes)Enum.Parse(typeof(IqTypes), element.Attribute("type").Value))
        {
            ReplaceAttributes(element.Attributes());
            ReplaceNodes(element.Nodes());
            Attribute("xmlns").Remove();
        }
        
        public Iq Reply()
        {
            IqType = IqTypes.result;
            var to = Attribute("from").Value;
            SetAttributeValue("from", Attribute("to").Value);
            SetAttributeValue("to", to);
            return this;
        }

        public Iq Error()
        {
            var result = Reply();
            result.IqType = IqTypes.error;
            var error = new XElement("error", new XElement(XNamespace.Get(Namespaces.StanzaErrors) + "service-unavailable"));
            error.SetAttributeValue("type", "cancel");
            result.Add(error);
            return result;
        }
    }
}
