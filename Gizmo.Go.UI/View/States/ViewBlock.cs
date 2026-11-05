using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.States
{
    /// <summary>
    /// State of one independently loaded block of a page: the data, the loading flag and the failure flag.
    /// Mutated only by the view service that owns the page.
    /// </summary>
    public sealed class ViewBlock<T>
    {
        // the block holds the result of a successful load, a reload keeps showing it until the new result arrives
        private bool _isLoaded;

        public T? Value { get; internal set; }

        /// <summary>
        /// Gets whether the block has nothing to show yet and is loading (also before the first load starts).
        /// A reload of a loaded block keeps the old value on screen and does not set it.
        /// </summary>
        public bool IsLoading { get; internal set; } = true;

        public bool HasError { get; internal set; }

        internal void Reset()
        {
            _isLoaded = false;
            Value = default;
            IsLoading = true;
            HasError = false;
        }

        /// <summary>
        /// Sets a value without a load (e.g. the empty result of a dependent block).
        /// </summary>
        internal void SetValue(T? value)
        {
            _isLoaded = true;
            Value = value;
            IsLoading = false;
            HasError = false;
        }

        /// <summary>
        /// Loads the block. A failure is logged and shown as <see cref="HasError"/>; a canceled load leaves the block untouched.
        /// </summary>
        internal async Task LoadAsync(Func<CancellationToken, Task<T?>> load, ILogger logger, string name, Action changed, CancellationToken cancellationToken)
        {
            IsLoading = !_isLoaded;
            HasError = false;
            changed();

            try
            {
                var value = await load(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    return;

                Value = value;
                _isLoaded = true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                logger.LogError(ex, "Cannot load the {Block} block.", name);
                Value = default;
                _isLoaded = false;
                HasError = true;
            }

            IsLoading = false;
            changed();
        }
    }
}
