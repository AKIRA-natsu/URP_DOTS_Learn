using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public abstract class CompositeNode : Node {
        [HideInInspector]
        public List<Node> children = new List<Node>();
    
        public override Node Clone() {
            CompositeNode node = base.Clone() as CompositeNode;
            node.children = children.ConvertAll(child => child.Clone());
            return node;
        }

    }
}