using System;
using System.Collections.Specialized;
using SharpXMPP.XMPP.Client.Elements;
using System.Collections.Generic;

namespace SharpXMPP.XMPP.Client.Register
{
    public class XmppRegisterHandler : PayloadHandler
    {
        public XmppRegisterHandler(IRegistrationHandler registrationHandler)
        {
            _regHandler = registrationHandler;
        }

        private readonly IRegistrationHandler _regHandler;

        public class RegistrationEventArgs
        {
            public RegistrationEventArgs(Dictionary<string, string> fields) { Fields = fields; }
            public Dictionary<string, string> Fields { get; private set; } // readonly
        }

        public delegate bool RegistrationEventHandler (object sender, RegistrationEventArgs args);
        
        
        private Dictionary<string, string> _fields;

        private static Dictionary<string, string> FilterFields(Stanza query, Dictionary<string, string> requestFields)
        {
            var filtered = new Dictionary<string, string>();
            foreach (string field in requestFields.Keys)
            {
                foreach (var val in query.Elements())
                {
                    if (field.Equals(val.Name) && !val.Value.Equals(string.Empty))
                    {
                        filtered[field] = val.Value;
                    }
                    if (!val.Name.Equals("remove")) continue;
                    if (!filtered.ContainsKey("remove"))
                        filtered.Add("remove", "true");
                }
                
            }
            return filtered;
        }

        public string[] Features
        {
            get { return new[] {"jabber:iq:register"}; }
        }

        public override bool Handle(XmppConnection sender, XMPPIq element)
        {
            var type = element.Attribute("type");
            if (type == null) return false;
            if (type.Equals("get"))
            {
                var query = element.Element("query");
                if (query != null)
                {
                    var ns = query.Attribute("xmlns");
                    var num = Array.IndexOf(Features, ns);
                    switch (num)
                    {
                        case 0:
                            var from = new JID(element.Attribute("from").Value);
                            _fields = _regHandler.GetCredentials(from);

                            var reply = element.Reply();
                            var instr = new Stanza("instructions") { Value = _regHandler.GetInstructions() };
                            query.Add(instr);
                            foreach (string field in _fields.Keys)
                            {
                                var f = new Stanza(field) { Value = _fields[field] };
                                query.Add(f);
                            }
                            sender.Send(reply);
                            return true;
                    }
                }
            }
            if (type.Equals("set"))
            {
                var query = element.Element("query");
                if (query != null)
                {
                    var ns = query.Attribute("xmlns");
                    var num = Array.IndexOf(Features, ns);
                    switch (num)
                    {
                        case 0:
                            var queryFields = FilterFields((Stanza)query, _fields);
                            var from = new JID(element.Attribute("from").Value);
                            var requestResult = queryFields.ContainsKey("remove") ? _regHandler.OnRegistrationRemove(from) : _regHandler.OnRegistrationRequest(from, queryFields);

                            if (requestResult)
                            {
                                var reply = element.Reply();
                                sender.Send(reply);
                                var subscribe = new XMPPPresence();
                                subscribe.SetAttributeValue("to", from.BareJid);
                                subscribe.SetAttributeValue("from", sender.Jid.BareJid);
                                if (queryFields.ContainsKey("remove"))
                                {
                                    subscribe.SetAttributeValue("type", "unsubscribed");
                                    sender.Send(subscribe);
                                    subscribe.SetAttributeValue("type", "unsubscribe");
                                    sender.Send(subscribe);
                                }
                                else
                                {
                                    subscribe.SetAttributeValue("type", "subscribe");
                                    sender.Send(subscribe);
                                }

                            }
                            else
                            {
                                sender.Send(element.Error());
                            }
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
