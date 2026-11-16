namespace Gizmo.Go.UI.Services.Dialog
{
    /// <summary>
    /// A host shown in the "host already taken" sheet.
    /// </summary>
    public sealed class BookingHostTakenItem
    {
        /// <summary>
        /// Gets the host number.
        /// </summary>
        public int Number { get; init; }

        /// <summary>
        /// Gets the host name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the host group name with the hourly rate, e.g. "Standart · 3.50/h".
        /// </summary>
        public string Details { get; init; } = string.Empty;
    }

    /// <summary>
    /// What the user chose in the "host already taken" sheet.
    /// </summary>
    public sealed class BookingHostTakenResult
    {
        /// <summary>
        /// Gets or sets whether the user wants to change the session time (otherwise the user returns to the PC selection).
        /// </summary>
        public bool ChangeTime { get; set; }
    }

    /// <summary>
    /// What the user chose in the "Booked!" dialog.
    /// </summary>
    public sealed class BookingSuccessResult
    {
        /// <summary>
        /// Gets or sets whether the user opens the reservation (otherwise goes Home).
        /// </summary>
        public bool OpenReservation { get; set; }
    }
}
