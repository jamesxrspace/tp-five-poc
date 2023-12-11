using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor.Widgets
{
    [Widget(typeof(PlaySoundNode))]
	public sealed class PlaySoundUnitWidget : UnitWidget<PlaySoundNode>
	{
        public PlaySoundUnitWidget(FlowCanvas canvas, PlaySoundNode unit) : base(canvas, unit)
		{
		}
    }
}
