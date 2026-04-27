using Gizmo.Go.Maui.Services;

namespace Gizmo.Go.Maui
{
    public partial class App : Application
    {
        private readonly MauiAppLifecycleService _lifecycleService;

        public App(MauiAppLifecycleService lifecycleService)
        {
            InitializeComponent();
            _lifecycleService = lifecycleService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage());
            window.Resumed += (_, _) => _lifecycleService.RaiseResumed();
            return window;
        }
    }
}
