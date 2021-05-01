using UnityEngine;

public class Snatchy : MonoBehaviour
{
    private PlayerInputs inputs;

    [SerializeField] private float speed = 150f;

    private Vector2 moveAxis;
    private Rigidbody2D rgdb;

    public bool isFacingRight = true;

    private void Awake()
    {
        rgdb = GetComponent<Rigidbody2D>();

        inputs = new PlayerInputs();

        inputs.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        inputs.Player.Move.canceled += ctx => OnMove(Vector2.zero);

        inputs.Player.Jump.performed += ctx => OnJump();
    }

    private void Start()
    {

    }

    private void Update()
    {
        //if ((rgdb.velocity.x > 0 && !isFacingRight) || (rgdb.velocity.x < 0 && isFacingRight)) Flip();
    }

    private void FixedUpdate()
    {
        rgdb.velocity = moveAxis * speed * Time.deltaTime;
    }

    private void OnMove(Vector2 input)
    {
        moveAxis = input;
        moveAxis.y = 0f;
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private void OnJump()
    {

    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }
}
