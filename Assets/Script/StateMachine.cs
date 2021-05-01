using System.Collections.Generic;

public class StateMachine
{
    public enum Action
    {
        MoveRight,
        MoveUp,
        MoveLeft,
        MoveDown
    }

    public class State
    {
        public enum Condition
        {
            Intersection,
            Plate,
            Button
        }

        public class Output
        {
            public int outputIndex;
            public Condition condition;

            public Output(int outputIndex, Condition condition)
            {
                this.outputIndex = outputIndex;
                this.condition = condition;
            }
        }

        Action action;
        List<Output> outputsIndices;

        public State(Action action, params int[] indices)
        {
            this.action = action;
            outputsIndices = new List<Output>(4);
        }
    }

    State m_Current;
    List<State> m_States = new List<State>();

    public void AddState(State state)
    {
        m_States.Add(state);
    }

    public void Start()
    {
        if (m_States.Count > 0)
        {
            m_Current = m_States[0];
        }
    }
}
