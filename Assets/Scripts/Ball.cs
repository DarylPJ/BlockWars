using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 launch = new Vector2(2f, 10f);
    [SerializeField] private Collider2D leftWall;
    [SerializeField] private Collider2D rightWall;
    [SerializeField] private Collider2D roof;
    [SerializeField] private Collider2D paddleCollider;
    [SerializeField, Range(0, 5)] private float maxRandomToAdd = 1;

    private bool lockedToPaddle = true;
    private bool runningOnAndroid = false;
    private Vector3 ballToPaddle;

    private Paddle paddle;
    private Rigidbody2D myRigidbody;

    void Start()
    {
        paddle = FindObjectOfType<Paddle>();
        ballToPaddle = transform.position - paddle.transform.position;
        runningOnAndroid = Application.platform == RuntimePlatform.Android;
        myRigidbody = GetComponent<Rigidbody2D>();
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

    private void OnTriggerEnter2D(Collider2D collision) => SetNewVelocity(collision);

    private void OnTriggerStay2D(Collider2D collision) 
    {
        if (paddleCollider == collision)
        {
            return;
        }

        var direction = HardCodedCollidionDirections(collision);
        SetVelocityByDirection(direction, myRigidbody.velocity.x, myRigidbody.velocity.y);
    }

    private void SetNewVelocity(Collider2D collision)
    {
        if (lockedToPaddle)
        {
            return;
        }

        var direction = GetNewDirection(collision);


        if (collision == paddleCollider)
        {
            HandlePaddleCollision(collision);
            return;
        }

        var randomToAdd = Random.Range(-maxRandomToAdd, maxRandomToAdd);
        var newXVelocity = myRigidbody.velocity.x + randomToAdd;

        newXVelocity = Mathf.Abs(newXVelocity) < 0.1f ? 0.1f :
            Mathf.Abs(newXVelocity) > myRigidbody.velocity.magnitude - 0.1f ?
                newXVelocity > 0 ? myRigidbody.velocity.magnitude - 0.1f : -myRigidbody.velocity.magnitude + 0.1f :
                    newXVelocity;

        float newYVelocity = GetScaledYVelocity(newXVelocity);

        SetVelocityByDirection(direction, newXVelocity, newYVelocity);
    }

    private float GetScaledYVelocity(float newXVelocity)
    {
        var newYVelocity = Mathf.Sqrt(myRigidbody.velocity.sqrMagnitude - Mathf.Pow(newXVelocity, 2));
        newYVelocity = myRigidbody.velocity.y < 0 ? -newYVelocity : newYVelocity;

        if (float.IsNaN(newYVelocity))
        {
            Debug.Log(myRigidbody.velocity.sqrMagnitude);
            Debug.Log(newXVelocity);
        }

        return newYVelocity;
    }

    private void SetVelocityByDirection(Direction direction, float xVelocity, float yVelocity)
    {
        if (direction == Direction.Down)
        {
            myRigidbody.velocity = new Vector2(xVelocity, -Mathf.Abs(yVelocity));
        }

        if (direction == Direction.Right)
        {
            myRigidbody.velocity = new Vector2(Mathf.Abs(xVelocity), yVelocity);
        }

        if (direction == Direction.Left)
        {
            myRigidbody.velocity = new Vector2(-Mathf.Abs(xVelocity), yVelocity);
        }

        if (direction == Direction.Up)
        {
            myRigidbody.velocity = new Vector2(xVelocity, Mathf.Abs(yVelocity));
        }
    }

    private void HandlePaddleCollision(Collider2D collision)
    {
        var relativePosition = transform.position - collision.transform.position;

        if (relativePosition.y < 1 && myRigidbody.velocity.x < 0)
        {
            myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x), -Mathf.Abs(myRigidbody.velocity.y));
            return;
        }

        if (relativePosition.y < 1 && myRigidbody.velocity.x > 0)
        {
            myRigidbody.velocity = new Vector2(-Mathf.Abs(myRigidbody.velocity.x), -Mathf.Abs(myRigidbody.velocity.y));
            return;
        }

        var maximum = myRigidbody.velocity.magnitude / 0.8f;
        var xVelocity = (relativePosition.x / 2f) * maximum;
        var yVelocity = GetScaledYVelocity(xVelocity);
        myRigidbody.velocity = new Vector2(xVelocity, Mathf.Abs(yVelocity));
    }

    private Direction GetNewDirection(Collider2D collision)
    {
        var direction = HardCodedCollidionDirections(collision);

        return direction == Direction.None ? BoxCollidionDirection(collision) : direction;
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
