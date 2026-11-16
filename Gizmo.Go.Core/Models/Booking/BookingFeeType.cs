namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// How the club reservation fee is calculated.
    /// </summary>
    public enum BookingFeeType
    {
        /// <summary>
        /// No fee.
        /// </summary>
        None,

        /// <summary>
        /// A fixed amount.
        /// </summary>
        Fixed,

        /// <summary>
        /// A percentage of the cost.
        /// </summary>
        Percentage
    }
}
