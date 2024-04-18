using System;
using System.Collections.Generic;
using System.Linq;

namespace AKIRA.Behaviour {
    // 状态能改变自身状态机
    // 状态不能获得或改变自身状态机以外的状态机
    // 分层状态机互相之间不受影响

    // who have machine
    public interface IMachineOwner {
        // have a state machine
        IStateMachine StateMachine { get; }
    }

    // base interface for state
    public interface IState : IDisposable {
        // self change machine state
        IStateMachine Machine { get; set; }
        // enter this state
        void OnEnter();
        // exit this state
        void OnExit();
    }

    // base interface for state machine
    public interface IStateMachine : IDisposable {
        // machine owner
        IMachineOwner Owner { get; set; }
        // is working
        bool IsWorking { get; }
        // switch state
        void Switch<T>() where T : IState;
        // stop machine
        void Stop();
    }

    // interface for hfsm
    public interface IHFStateMachine : IStateMachine {
        // regist sub machine
        T RegistSubMachine<T>(string tag) where T : IStateMachine, new();
        // get sub machine
        IStateMachine GetSubMachine(string tag);
        // remove sub machine
        void RemoveSubMachine(string tag);
    }

    // sub state machine
    [Serializable]
    public class StateMachine : IStateMachine {
        // states
        private Dictionary<Type, IState> states = new();
        // cur state type
        public Type CurState { get; private set; }
        // is machine working
        public bool IsWorking => CurState != null;
        // cur working state
        private IState State => IsWorking ? states[CurState] : null;
        // machine owner
        public IMachineOwner Owner { get; set; }

        public StateMachine() {}
        public StateMachine(IMachineOwner owner) => Owner = owner;

        // regist state
        public void RegistState<T>() where T : IState, new() => RegistState(new T());
        public void RegistState<T>(T state) where T : IState {
            var key = typeof(T);
            state.Machine = this;
            states[key] = state;
        }

        // remove state
        public void RemoveState<T>() where T : IState, new() {
            var key = typeof(T);
            if (!states.ContainsKey(key))
                return;
            
            State.Dispose(); 
            states.Remove(key);

            if (CurState == key)
                Stop();
        }

        // switch new state
        public void Switch<T>() where T : IState {
            var newState = typeof(T);
            if (!states.ContainsKey(newState))
                return;
            
            if (CurState == newState)
                return;
            
            State?.OnExit();
            CurState = newState;
            State.OnEnter();
        }

        // stop machine
        public void Stop() {
            State?.OnExit();
            CurState = null;
        }

        // dispose machine
        public void Dispose() {
            CurState = null;
            states.Foreach(kvp => kvp.Value.Dispose());
            states.Clear();
            Owner = null;
        }
    }

    // 分层状态机 HFSM
    [Serializable]
    public class HierarchyFiniteStateMachine : IHFStateMachine {
        // tag - machine
        private Dictionary<string, IStateMachine> machines = new();
        // machine owner
        public IMachineOwner Owner { get; set; }
        // working
        public bool IsWorking => !machines.Values.Any(value => !value.IsWorking);

        public HierarchyFiniteStateMachine() {}
        public HierarchyFiniteStateMachine(IMachineOwner owner) => Owner = owner;

        // add sub state machine
        public T RegistSubMachine<T>(string tag) where T : IStateMachine, new() {
            // 如果存在进行替换
            var machine = new T() { Owner = this.Owner } ;
            machines[tag] = machine;
            return machine;
        }

        // remove sub state machine
        public void RemoveSubMachine(string tag) {
            if (!machines.ContainsKey(tag))
                return;
            machines.Remove(tag);
        }

        // get sub machine
        public IStateMachine GetSubMachine(string tag) {
            if (machines.ContainsKey(tag))
                return machines[tag];
            return default;
        }

        public void Switch<T>() where T : IState {
            foreach (var machine in machines.Values)
                machine.Switch<T>();
        }

        // stop all sub machine
        public void Stop() {
            machines.Values.Foreach(machine => machine.Stop());
        }

        // dispose all sub machine
        public void Dispose() {
            machines.Values.Foreach(machine => machine.Dispose());
            machines.Clear();
            Owner = null;
        }
    }

}