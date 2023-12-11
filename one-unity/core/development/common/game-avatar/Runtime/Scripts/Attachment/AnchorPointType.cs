namespace TPFive.Game.Avatar.Attachment
{
    /// <summary>
    /// Define the anchors that avatar can use.
    /// </summary>
    public enum AnchorPointType
    {
        /// <summary>
        /// Between the Avatar's feet.
        /// </summary>
        Root,

        /// <summary>
        /// base on head bone.
        /// </summary>
        Hair,

        /// <summary>
        /// base on head bone.
        /// </summary>
        Glasses,

        /// <summary>
        /// base on head bone.
        /// Example: left earrings
        /// </summary>
        LeftEar,

        /// <summary>
        /// base on head bone.
        /// Example: right earrings
        /// </summary>
        RightEar,

        /// <summary>
        /// base on left lower arm bone.
        /// Examplt: left bracelet
        /// </summary>
        LeftWrist,

        /// <summary>
        /// base on right lower arm bone.
        /// Example: right bracelet
        /// </summary>
        RightWrist,

        /// <summary>
        /// base on left hand bone.
        /// </summary>
        LeftPalm,

        /// <summary>
        /// base on right hand bone.
        /// </summary>
        RightPalm,

        /// <summary>
        /// base on spine bone.
        /// </summary>
        Abdomen,
    }
}