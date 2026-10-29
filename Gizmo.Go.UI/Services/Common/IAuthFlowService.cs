namespace Gizmo.Go.UI.Services.Common
{
    /// <summary>
    /// Shared actions of the registration and password recovery flows.
    /// </summary>
    public interface IAuthFlowService
    {
        /// <summary>
        /// Leaves the registration or password recovery flow: clears both sessions
        /// (the step view services reset on Cleared / on navigation) and opens Welcome.
        /// </summary>
        void Leave();
    }
}
