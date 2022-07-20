using BTree.Core;
using System.Linq;

namespace BTree.Nodes
{
    [BehaviourNode("Composite", "Random Fallback")]
    public class RandomFallbackNode : FallbackNode
    {
        private System.Random m_Random;

        public override void Initialize()
        {
            m_Random = new();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Childrens = Childrens.OrderBy(c => m_Random.Next()).ToList();
        }
    }
}