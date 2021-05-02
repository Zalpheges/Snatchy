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

        public enum Face
        {
            East,
            North,
            West,
            South
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
        Output[] m_Outputs;

        public State(Action action)
        {
            this.action = action;

            m_Outputs = new Output[4];
        }

        public void SetOutput(Face face, Output output)
        {
            m_Outputs[(int)face] = output;
        }

        public int CheckCondition(Condition condition)
        {
            for (int i = 0; i < 4; i++) if (m_Outputs[i] != null && m_Outputs[i].condition == condition) return m_Outputs[i].index;

            return -1;
        }
    }

    State m_Current;
    List<State> m_States = new List<State>();

    public void AddState(State state)
    {
        m_States.Add(state);
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
