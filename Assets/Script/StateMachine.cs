using System.Collections.Generic;

public class StateMachine
{
    public enum Action
    {
        None,
        MoveRight,
        MoveUp,
        MoveLeft,
        MoveDown,
        TurnRight,
        TurnLeft,
        TurnAround,
        Push
    }

    public class State
    {
        public enum Condition
        {
            Intersection,
            Plate,
            Button,
            Wall
        }

        public class Output
        {
            public int index;
            public Condition condition;

            public Output(int outputIndex, Condition condition)
            {
                index = outputIndex;
                this.condition = condition;
            }
        }

        public Action action;
        public List<Output> m_Outputs;

        public State(Action action)
        {
            this.action = action;

            m_Outputs = new List<Output>();
        }

        public void SetOutput(Output output)
        {
            m_Outputs.Add(output);
        }

        public void DeleteOutput(int index)
        {
            m_Outputs.RemoveAt(index);
        }

        public int CheckCondition(Condition condition)
        {
            for (int i = 0; i < m_Outputs.Count; i++) if (m_Outputs[i] != null && m_Outputs[i].condition == condition) return m_Outputs[i].index;

            return -1;
        }
    }

    State m_Current;
    List<State> m_States = new List<State>();

    public int AddState(State state)
    {
        m_States.Add(state);
        return m_States.Count - 1;
    }

    public State.Output[] GetOutputs(int index)
    {
        return m_States[index].m_Outputs.ToArray();
    }

    public Action Start()
    {
        if (m_States.Count > 0) return (m_Current = m_States[0]).action;
        else return Action.None;
    }

    public Action OnEvent(State.Condition condition)
    {
        int index = m_Current.CheckCondition(condition);

        if (index >= 0) m_Current = m_States[index];

        return m_Current.action;
    }
}
