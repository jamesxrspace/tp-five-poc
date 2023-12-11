using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor.Widgets
{
    public class UnitWidgetBase<T> : UnitWidget<T> where T : Unit
    {
        public UnitWidgetBase(FlowCanvas canvas, T unit) : base(canvas, unit)
        {
        }
    }
}
