namespace AKIRA.Behaviour.AI {
    public class RepeatNode : DecoratorNode {
        public override void OnCreate() { }

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            child.Update();
            return State.Running;
        }
    }
}