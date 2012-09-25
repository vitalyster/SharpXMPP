using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.Logging;
using SharpXMPP.WPF.ViewModels;

namespace SharpXMPP.WPF.Util
{
    public class AppBootstrapper : Bootstrapper<ClientViewModel>
    {
        private CompositionContainer _container;
        #region Fields
        private readonly ILog _logger = LogManager.GetLog(typeof(AppBootstrapper));
        #endregion

        #region Constructor
        static AppBootstrapper()
        {
            LogManager.GetLog = type => new DebugLogger(typeof(AppBootstrapper));
        }
        #endregion
        protected override void Configure()
        {
            _container = new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new AppWindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
            _container.Compose(batch);
        }
        
        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Any())
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }
    }
}
