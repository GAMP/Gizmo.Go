using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Gizmo.Go.UI.Layout
{
    public partial class MainLayout : IDisposable
    {
        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private string _pageTitle = "Gizmo Go";

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += OnLocationChanged;
            UpdatePageTitle(Navigation.Uri);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            UpdatePageTitle(e.Location);
            StateHasChanged();
        }

        private void UpdatePageTitle(string uri)
        {
            var path = new Uri(uri).AbsolutePath.TrimStart('/').ToLowerInvariant();
            _pageTitle = path switch
            {
                "" => "Gizmo Go",
                "branches" => "Branches",
                "favorites" => "Favorites",
                "account" => "Account",
                "login" => "Login",
                _ => "Gizmo Go"
            };
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= OnLocationChanged;
        }
    }
}
