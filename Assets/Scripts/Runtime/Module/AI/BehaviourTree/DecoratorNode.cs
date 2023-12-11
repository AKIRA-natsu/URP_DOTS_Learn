using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public abstract class DecoratorNode : Node {
        [HideInInspector]
        public Node child;

        public override Node Clone() {
            DecoratorNode node = base.Clone() as DecoratorNode;
            node.child = child.Clone();
            return node;
        }
    }
}