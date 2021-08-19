namespace RetirementSimulator
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;

    using DevExpress.Mvvm.POCO;

    using Prism.Ioc;
    using Prism.Mvvm;

    using Core.Services;
    using Views;

    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<PersistenceService>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(
                viewType =>
                    {
                        var viewName = viewType.FullName;
                        viewName = viewName?.Replace(".Views.", ".ViewModels.");
                        var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                        var suffix = viewName != null && viewName.EndsWith("View") ? "Model" : "ViewModel";
                        var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
                        return ViewModelSource.GetPOCOType(Type.GetType(viewModelName));
                    });
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<HomeView>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.Container.Resolve<PersistenceService>()?.Close();

            base.OnExit(e);
        }
    }
}
