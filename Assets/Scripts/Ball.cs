using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 launch = new Vector2(2f, 10f);
    [SerializeField] private Collider2D leftWall;
    [SerializeField] private Collider2D rightWall;
    [SerializeField] private Collider2D roof;
    [SerializeField] private Collider2D paddleCollider;


    private bool lockedToPaddle = true;
    private bool runningOnAndroid = false;
    private Vector3 ballToPaddle;
   
    private Paddle paddle;
    private Rigidbody2D myRigidbody;
    private CircleCollider2D myCollider;

    void Start()
    {
        paddle = FindObjectOfType<Paddle>();
        ballToPaddle = transform.position - paddle.transform.position;
        runningOnAndroid = Application.platform == RuntimePlatform.Android;
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (!lockedToPaddle)
        {
            return;
        }

        transform.position = paddle.transform.position + ballToPaddle;
        if (ShouldFire())
        {
            lockedToPaddle = false;
            myRigidbody.velocity = launch;
        }
    }

    private bool ShouldFire()
    {
        if (!runningOnAndroid)
        {
            return Input.GetMouseButton(0);
        }

        if (Input.touchCount == 0)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var direction = HardCodedCollidionDirections(collision);

        direction = direction == Direction.None ? BoxCollidionDirection(collision) : direction;
        SetNewVelocity(direction);
    }

    private void SetNewVelocity(Direction direction)
    {
        if (direction == Direction.Down)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -Mathf.Abs(myRigidbody.velocity.y));
        }

        if (direction == Direction.Right)
        {
            myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x), myRigidbody.velocity.y);
        }

        if (direction == Direction.Left)
        {
            myRigidbody.velocity = new Vector2(-Mathf.Abs(myRigidbody.velocity.x), myRigidbody.velocity.y);
        }

        if (direction == Direction.Up)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Abs(myRigidbody.velocity.y));
        }
    }

    private Direction HardCodedCollidionDirections(Collider2D collision) => 
        collision == roof ? Direction.Down : 
            collision == leftWall ? Direction.Right :
                collision == rightWall ? Direction.Left :
                    collision == paddleCollider ? Direction.Up : Direction.None;

    private Direction BoxCollidionDirection(Collider2D collision)
    {
        var relativePosition = (Vector2)collision.transform.position - (Vector2)transform.position + new Vector2(1.95f/2, 0.95f/2);
        var angle = Vector2.SignedAngle(relativePosition, Vector2.right);

        if (myRigidbody.velocity.x > 0 && myRigidbody.velocity.y > 0)
        {
            return angle > -30 && angle < 100 ? Direction.Left : Direction.Down;
        }

        if (myRigidbody.velocity.x < 0 && myRigidbody.velocity.y > 0)
        {
            return angle > -150 && angle < 100 ? Direction.Down : Direction.Right;
        }

        if (myRigidbody.velocity.x > 0 && myRigidbody.velocity.y < 0)
        {
            return angle > 30 ? Direction.Up : Direction.Left;
        }

        if (myRigidbody.velocity.x < 0 && myRigidbody.velocity.y < 0)
        {
            return angle > -50 && angle < 150 ? Direction.Up : Direction.Right;
        }

        return Direction.None;
    }
}
