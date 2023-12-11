namespace AKIRA.Behaviour.AI {
    public class RootNode : Node {
        public Node child;

        public override void OnCreate() { }

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() {
            return child.Update();
        }

        public override Node Clone() {
            RootNode node = base.Clone() as RootNode;
            node.child = child.Clone();
            return node;
        }
    }
}