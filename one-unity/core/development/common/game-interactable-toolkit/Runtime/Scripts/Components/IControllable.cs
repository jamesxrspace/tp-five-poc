namespace TPFive.Game.Interactable.Toolkit
{
    /// <summary>
    /// Define this interface to make the object controllable and status.
    /// </summary>
    public interface IControllable
    {
        /// <summary>
        /// Gets a value indicating whether get this object is controlled or not.
        /// </summary>
        bool IsControlled { get; }

        /// <summary>
        /// Control this object and set the status to be controlled.
        /// </summary>
        void Control();

        /// <summary>
        /// Release this object and set the status to be not controlled.
        /// </summary>
        void Release();
    }
}