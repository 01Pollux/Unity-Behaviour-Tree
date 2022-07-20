using UnityEngine.UIElements;

namespace BTreeEditor.Core
{
    public class BehaviourTreeSplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeSplitView, UxmlTraits>
        { }
    }
}
