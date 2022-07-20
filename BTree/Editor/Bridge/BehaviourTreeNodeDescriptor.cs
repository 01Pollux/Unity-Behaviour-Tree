using BTree.Core;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BTreeEditor.Bridge
{
    public class BehaviourTreeNodeDescriptor
    {
        public string[] ClassNames;

        public bool HasInput = true;
        public Port.Capacity InputCapacity;

        public bool HasOuput = true;
        public Port.Capacity OuputCapacity;
    }
}
