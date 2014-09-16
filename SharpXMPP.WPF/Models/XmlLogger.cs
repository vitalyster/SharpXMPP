using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace SharpXMPP.WPF.Models
{
    public class XmlLogger
    {
        public XmlLogger(XmppClient connection, XMPPContext context)
        {
            connection.Element += (sender, args) =>
                                       {
                                           var raw = new RawXml 
                                           { 
                                               IsInput = args.IsInput, 
                                               Data = args.Stanza.ToString() 
                                           };
                                           Application.Current.Dispatcher.BeginInvoke(
                                               new Action(() => 
                                               { 
                                                   context.Log.Add(raw);
                                                   context.SaveChanges();
                                               }));
                                           
                                       };
        }
    }
}
