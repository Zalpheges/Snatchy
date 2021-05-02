using System.Collections;
using UnityEngine;

public class Snatchy : MonoBehaviour
{
    private PlayerInputs inputs;

    [SerializeField] private float speed = 150f;

    public StateMachine.Action state;
    public Vector3 direction;
    public Vector3 goal;
    public LayerMask ground;
    public bool isTurning = false;
    private Rigidbody2D rgdb;
    Animator animator;

    StateMachine machine;

    private void Awake()
    {
        rgdb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        inputs = new PlayerInputs();

        inputs.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
    }

    private void Start()
    {
        machine = new StateMachine();

        direction = Vector2.right;
        StateMachine.State state = new StateMachine.State(StateMachine.Action.TurnRight);
        StateMachine.State state1 = new StateMachine.State(StateMachine.Action.TurnLeft);
        state.SetOutput(StateMachine.State.Face.North, new StateMachine.State.Output(0, StateMachine.State.Condition.Intersection));
        state.SetOutput(StateMachine.State.Face.South, new StateMachine.State.Output(1, StateMachine.State.Condition.Wall));

        state1.SetOutput(StateMachine.State.Face.North, new StateMachine.State.Output(0, StateMachine.State.Condition.Intersection));
        state1.SetOutput(StateMachine.State.Face.South, new StateMachine.State.Output(1, StateMachine.State.Condition.Wall));
        machine.AddState(state);
        machine.AddState(state1);

        goal = rgdb.position;
        direction = Vector2.right;
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        SetAction(machine.Start());
    }

    private void FixedUpdate()
    {
        if (!isTurning)
        {
            Vector3 position = rgdb.position;

            if (position == goal) OnGoal();
            else
            {
                position = Vector2.MoveTowards(position, goal, Time.deltaTime);
                rgdb.MovePosition(position);
            }
        }
        else rgdb.velocity = Vector2.zero;

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }

    public void SetAction(StateMachine.Action action)
    {
        state = action;

        switch (state)
        {
            case StateMachine.Action.TurnAround:
                direction *= -1;
                animator.SetFloat("To", ((animator.GetFloat("Current") + 180f) + 360f) % 360f);
                break;

            case StateMachine.Action.TurnLeft:
                direction = new Vector2(direction.y, direction.x);
                animator.SetFloat("To", ((animator.GetFloat("Current") + 90f) + 360f) % 360f);
                break;

            case StateMachine.Action.TurnRight:
                direction = new Vector2(direction.y, -direction.x);
                animator.SetFloat("To", ((animator.GetFloat("Current") - 90f) + 360f) % 360f);
                break;

            case StateMachine.Action.MoveRight:
                direction = Vector2.right;
                animator.SetFloat("To", 0f);
                break;

            case StateMachine.Action.MoveUp:
                direction = Vector2.up;
                animator.SetFloat("To", 90f);
                break;

            case StateMachine.Action.MoveLeft:
                direction = Vector2.left;
                animator.SetFloat("To", 180f);
                break;

            case StateMachine.Action.MoveDown:
                direction = Vector2.down;
                animator.SetFloat("To", 270f);
                break;

            case StateMachine.Action.None:
            default:
                direction = direction.normalized * 0.5f;
                break;
        }

        if (animator.GetFloat("Current") != animator.GetFloat("To")) StartCoroutine(Turn());
    }

    private IEnumerator Turn()
    {
        animator.SetTrigger("Turn");
        isTurning = true;

        yield return new WaitForSeconds(0.1f);

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        yield return new WaitForSeconds(7 / 6f - 0.1f);

        animator.SetFloat("Current", animator.GetFloat("To"));
        isTurning = false;
    }

    private void OnGoal()
    {
        bool intersection = false;

        Vector2 _direction = new Vector2(direction.y, direction.x);
        if (!Physics2D.Raycast(rgdb.position, _direction, 1f, ground)) intersection = true;

        _direction = new Vector2(_direction.y, _direction.x);
        if (!Physics2D.Raycast(rgdb.position, _direction, 1f, ground)) intersection = true;

        _direction = new Vector2(_direction.y, _direction.x);
        if (!Physics2D.Raycast(rgdb.position, _direction, 1f, ground)) intersection = true;

        if (intersection) SetAction(machine.OnEvent(StateMachine.State.Condition.Intersection));
        else
        {
            if (!Physics2D.Raycast(rgdb.position, direction, 1f, ground)) goal += direction;
            else SetAction(machine.OnEvent(StateMachine.State.Condition.Wall));
        }
    }

    private void OnMove(Vector2 input)
    {
        direction = input;
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
}
