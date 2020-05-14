namespace RetirementSimulator
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;

    using DevExpress.Mvvm.POCO;

    using Prism.Ioc;
    using Prism.Mvvm;

    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
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
            return this.Container.Resolve<MainWindow>();
        }
    }
}
