using System.Collections;
using UnityEngine;

public class Snatchy : MonoBehaviour
{
    private PlayerInputs inputs;

    [SerializeField] private float speed = 150f;

    public StateMachine.Action state;
    private Vector3 direction;
    private Vector3 goal;
    public LayerMask ground;
    public LayerMask button;
    private bool isTurning = false;
    private bool last = false;
    public int balade = 0;
    private Rigidbody2D rgdb;
    Animator animator;
    public AudioClip[] clips;
    private AudioSource source;

    StateMachine machine;

    private void Awake()
    {
        rgdb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        source = GetComponent<AudioSource>();

        inputs = new PlayerInputs();

        inputs.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
    }

    private void Start()
    {
        machine = new StateMachine();

        StateMachine.State state = new StateMachine.State(StateMachine.Action.TurnLeft);
        StateMachine.State state1 = new StateMachine.State(StateMachine.Action.TurnRight);
        StateMachine.State state2 = new StateMachine.State(StateMachine.Action.Push);
        state.SetOutput(new StateMachine.State.Output(0, StateMachine.State.Condition.Intersection));
        state.SetOutput(new StateMachine.State.Output(1, StateMachine.State.Condition.Wall));
        state.SetOutput(new StateMachine.State.Output(2, StateMachine.State.Condition.Button));

        state1.SetOutput(new StateMachine.State.Output(0, StateMachine.State.Condition.Intersection));
        state1.SetOutput(new StateMachine.State.Output(1, StateMachine.State.Condition.Wall));
        state1.SetOutput(new StateMachine.State.Output(2, StateMachine.State.Condition.Button));

        state2.SetOutput(new StateMachine.State.Output(0, StateMachine.State.Condition.Wall));

        machine.AddState(state);
        machine.AddState(state1);
        machine.AddState(state2);

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
                position = Vector2.MoveTowards(position, goal, 2 * Time.deltaTime);
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
                direction = Rotate(direction, 90f);
                animator.SetFloat("To", ((animator.GetFloat("Current") + 90f) + 360f) % 360f);
                break;

            case StateMachine.Action.TurnRight:
                direction = Rotate(direction, -90f);
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

            case StateMachine.Action.Push:
                StartCoroutine(Push());
                break;

            case StateMachine.Action.None:
            default:
                direction = direction.normalized * 0.5f;
                break;
        }

        if (animator.GetFloat("Current") != animator.GetFloat("To")) StartCoroutine(Turn());
    }

    private IEnumerator Push()
    {
        animator.SetTrigger("Push");
        isTurning = true;

        yield return new WaitForSeconds(0.4f);

        RaycastHit2D _button = Physics2D.Raycast(rgdb.position, direction, 1f, button);
        _button.collider.SendMessageUpwards("Push");

        source.clip = clips[0];
        source.Play();

        yield return new WaitForSeconds(7 / 6f - 0.4f);

        isTurning = false;
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
        if (Physics2D.Raycast(rgdb.position, direction, 1f, button)) SetAction(machine.OnEvent(StateMachine.State.Condition.Button));
        else
        {
            bool wall = false;
            int ways = 0;

            if (!Physics2D.Raycast(rgdb.position, direction, 1f, ground))
            {
                wall = true;
                ways++;
            }

            Vector2 _direction = Rotate(direction, -90f);
            if (!Physics2D.Raycast(rgdb.position, _direction, 1f, ground)) ways++;

            _direction = Rotate(_direction, -180f);
            if (!Physics2D.Raycast(rgdb.position, _direction, 1f, ground)) ways++;

            if (!last && ways > 1)
            {
                SetAction(machine.OnEvent(StateMachine.State.Condition.Intersection));
                last = true;
            }
            else
            {
                if (!Physics2D.Raycast(rgdb.position, direction, 1f, ground))
                {
                    if (balade < 3)
                    {
                        balade++;
                        return;
                    }
                    else
                    {
                        //source.clip = clips[3];
                        //source.Play();
                    }

                    goal += direction;
                    last = false;
                }
                else
                {
                    SetAction(machine.OnEvent(StateMachine.State.Condition.Wall));
                    source.clip = clips[1];
                    source.Play();
                }
            }
        }

        balade = 0;
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

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
