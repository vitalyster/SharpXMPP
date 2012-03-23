using System;
using System.Globalization;
using System.Xml.Linq;

namespace SharpXMPP.Client.Elements
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

        public static Iq CreateFrom(XElement element)
        {
            var result = new Iq((IqTypes)Enum.Parse(typeof (IqTypes), element.Attribute("type").Value));
            result.ReplaceAttributes(element.Attributes());
            result.ReplaceNodes(element.Nodes());
            result.Attribute("xmlns").Remove();
            return result;
        }
    }
}
