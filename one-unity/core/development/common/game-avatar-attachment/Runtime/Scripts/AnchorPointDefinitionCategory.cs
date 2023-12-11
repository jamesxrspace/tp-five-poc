using UnityEngine;

namespace TPFive.Game.Avatar.Attachment
{
    [CreateAssetMenu(fileName = "AnchorPointDefinitionCategory", menuName = "TPFive/Avatar/Create AnchorPointDefinitionCategory")]
    public class AnchorPointDefinitionCategory : ScriptableObject
    {
        [SerializeField]
        private string categoryName;

        [SerializeField]
        private AnchorPointDefinition[] definitions;

        public string CategoryName => categoryName;

        public AnchorPointDefinition[] Definitions => definitions;
    }
}