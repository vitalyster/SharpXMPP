using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SharpXMPP.WPF.Models
{
    public class XmlLogger
    {
        private readonly XmppClientConnection _connection;
        private readonly XMPPContext _context;
        public XmlLogger(XmppClientConnection connection, XMPPContext context)
        {
            _connection = connection;
            _context = context;
            _connection.Element += (sender, args) =>
                                       {
                                           var raw = new RawXml {IsInput = args.IsInput, Data = args.Stanza.ToString()};
                                           _context.Entry(raw).State = EntityState.Added;
                                           _context.SaveChanges();
                                       };
        }
    }
}
