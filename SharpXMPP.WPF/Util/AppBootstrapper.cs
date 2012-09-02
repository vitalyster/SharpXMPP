using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Caliburn.Micro.Logging;
using SharpXMPP.WPF.ViewModels;

namespace SharpXMPP.WPF.Util
{
    public class AppBootstrapper : Bootstrapper<ClientViewModel>
    {
        #region Fields
        private readonly ILog _logger = LogManager.GetLog(typeof(AppBootstrapper));
        #endregion

        #region Constructor
        static AppBootstrapper()
        {
            LogManager.GetLog = type => new DebugLogger(typeof(AppBootstrapper));
        }
        #endregion
    }
}
