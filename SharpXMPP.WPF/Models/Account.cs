using SharpXMPP.XMPP;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;
using System.Reflection;

namespace SharpXMPP.WPF.Models
{
    public class Account
    {
        public string JID { get; set; }
        public string Password { get; set; }      
    }
}
