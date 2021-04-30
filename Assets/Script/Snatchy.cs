using UnityEngine;
using UnityEngine.InputSystem;

public class Snatchy : MonoBehaviour
{
    private float gravityMultiplier = 1f;

    private Rigidbody2D rgdb;
    private PlayerInputs inputs;

    private Vector2 moveAxis;

    Vector2 velocity;

    private void Awake()
    {
        rgdb = GetComponent<Rigidbody2D>();
        
        inputs = new PlayerInputs();

        inputs.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        inputs.Player.Move.canceled += ctx => OnMove(Vector2.zero);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        rgdb.velocity = velocity;
    }

    private void FixedUpdate()
    {
        velocity += gravityMultiplier * Physics2D.gravity * Time.deltaTime;
    }

    private void OnMove(Vector2 input)
    {
        moveAxis = input;
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
