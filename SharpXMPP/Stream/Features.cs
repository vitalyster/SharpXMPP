using System.ComponentModel;
using System.Xml.Serialization;

namespace SharpXMPP.Stream
{
    [XmlRoot("features", Namespace = Namespaces.Streams)]
    public class Features
    {
        [XmlElement("starttls", Namespace = Namespaces.XmppTls, IsNullable = true)]
        public StartTls Tls { get; set; }
        [XmlArray(ElementName = "mechanisms", Namespace = Namespaces.XmppSasl)]
        [XmlArrayItem(ElementName = "mechanism")]
        public Mechanism[] Mechanisms { get; set; }
    }

    [XmlRoot("starttls", Namespace = Namespaces.XmppTls, IsNullable = true)]
    public class StartTls
    {
        [XmlIgnore]
        public bool IsRequired { get; set; }
        
        [XmlElement("required")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string Required 
        { 
            get { return IsRequired ? "" : null; } 
            set { IsRequired = (value != null); } 
        }
    }
    
    [XmlRoot("mechanism")]
    public class Mechanism
    {
        [XmlText]
        public string Value { get; set; }
    }
}
