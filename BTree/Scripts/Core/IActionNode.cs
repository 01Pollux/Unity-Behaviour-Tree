
namespace BTree.Core
{
    public abstract class IActionNode : INodeBehaviour
    {
        public sealed override INodeBehaviour[] GetChildrens() => null;
    }
}