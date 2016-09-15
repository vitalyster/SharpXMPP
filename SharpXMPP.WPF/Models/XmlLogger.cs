using System;
using System.Windows;

namespace SharpXMPP.WPF.Models
{
    public class XmlLogger
    {
        DateTime _lastUpDateTime = DateTime.Now;
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
                                                   if ((_lastUpDateTime - DateTime.Now).TotalMilliseconds > 500)
                                                   {
                                                       context.SaveChanges();
                                                       _lastUpDateTime = DateTime.Now;
                                                   }
                                               }));
                                           
                                       };
        }
    }
}
