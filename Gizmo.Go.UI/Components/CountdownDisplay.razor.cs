using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class CountdownDisplay : ComponentBase
    {
        [Parameter] public int Seconds { get; set; }
        [Parameter] public Func<int, string>? Formatter { get; set; }

        private int _displayed = -1;

        protected override bool ShouldRender()
        {
            if (_displayed != Seconds)
            {
                _displayed = Seconds;
                return true;
            }
            return false;
        }

        private string Format(int s) =>
            Formatter?.Invoke(s) ?? $"{s / 60:D2}:{s % 60:D2}";
    }
}
