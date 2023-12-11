namespace TPFive.Game.AvatarEdit.Entry
{
    internal class PresetItem
    {
        public PresetItem(bool selected, string styleID, string assetId)
        {
            Selected = selected;
            StyleID = styleID;
            AssetId = assetId;
        }

        public bool Selected { get; }

        // Tab ID
        public string StyleID { get; }

        public string AssetId { get; }
    }
}