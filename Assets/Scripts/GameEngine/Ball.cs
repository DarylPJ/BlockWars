using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float launchSpeed = 15;
    [SerializeField] private Collider2D leftWall;
    [SerializeField] private Collider2D rightWall;
    [SerializeField] private Collider2D roof;
    [SerializeField] private Collider2D paddleCollider;
    [SerializeField] private Collider2D enemyFloor;
    [SerializeField] private Collider2D floor;
    [SerializeField, Range(0, 5)] private float maxRandomToAdd = 1;
    [SerializeField] private bool instaLaunch = false;
    [SerializeField, Range(0, 1)] private float maxXFire = 0.5f;
    [SerializeField] private Vector3 ballToPaddle;

    private bool lockedToPaddle = true;
    private bool runningOnAndroid = false;
    private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private bool allowLaunch = true;

    private Paddle paddle;
    private Rigidbody2D myRigidbody;
    private AudioSource audioSource;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private BallPowerUpState powerUpState;
    private AudioState audioState;
    private LevelState levelState;

    private void Start()
    {
        paddle = FindObjectOfType<Paddle>();
        myRigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        powerUpState = FindObjectOfType<BallPowerUpState>();
        audioState = FindObjectOfType<AudioState>();
        levelState = FindObjectOfType<LevelState>();

        runningOnAndroid = Application.platform == RuntimePlatform.Android;
        stopwatch.Start();
    }

    private void Update()
    {
        spriteRenderer.color = powerUpState.GetCurrentColour();
        var scale = powerUpState.GetCurrentScale();
        transform.localScale = new Vector2(scale, scale);
        
        if (!lockedToPaddle)
        {
            return;
        }

        transform.position = paddle.transform.position + ballToPaddle;
        if (ShouldFire())
        {
            var xClick = (runningOnAndroid ? Input.GetTouch(0).position.x : Mathf.Clamp(Input.mousePosition.x, 0, Screen.width));
            var relativePos = ((xClick / Screen.width) - maxXFire) / 0.7f;

            var x = relativePos * launchSpeed;
            var y = Mathf.Sqrt(Mathf.Pow(launchSpeed, 2) - Mathf.Pow(x, 2));

            lockedToPaddle = false;
            myRigidbody.velocity = new Vector2(x, y);
        }
    }

    public void NotLocked()
    {
        lockedToPaddle = false;
    }

    private bool ShouldFire()
    {
        if(!allowLaunch)
        {
            return false;
        }

        if (instaLaunch)
        {
            return true;
        }

        if (!runningOnAndroid)
        {
            return Input.GetMouseButton(0);
        }

        if (Input.touchCount == 1)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Projectile>() != null)
        {
            return;
        }

        if (collision.GetComponent<Shield>() != null)
        {
            HandleShieldTrigger();
            return;
        }

        if (collision.gameObject.CompareTag("PowerUp") || collision.GetComponent<Ball>() != null ||
            (powerUpState.IsFireModeActive() && collision.GetComponent<Block>()))
        {
            return;
        }

        if (collision == floor)
        {
            if (FindObjectsOfType<Ball>().Length == 1)
            {
                powerUpState.RemoveAllPowerUps(); 
                levelState.LooseLife();
                lockedToPaddle = true;
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }

        SetNewVelocity(collision);
    }

    private void OnTriggerStay2D(Collider2D collision) 
    {
        if (collision.GetComponent<Projectile>() != null)
        {
            return;
        }

        if (collision.GetComponent<Shield>() != null)
        {
            HandleShieldTrigger();
            return;
        }

        if (paddleCollider == collision  || lockedToPaddle)
        {
            return;
        }

        var direction = HardCodedCollidionDirections(collision);
        myRigidbody.velocity = GetNewVelocityByDirection(direction, myRigidbody.velocity.x, myRigidbody.velocity.y);
    }

    private void SetNewVelocity(Collider2D collision)
    {
        if (collision == paddleCollider)
        {
            HandlePaddleCollision(collision);
            return;
        }

        if (lockedToPaddle || stopwatch.ElapsedMilliseconds < (Time.deltaTime * 1000) || collision == enemyFloor)
        {
            return;
        }

        PlayCollisionNoise();

        var direction = GetNewDirection(collision);

        var randomToAdd = Random.Range(-maxRandomToAdd, maxRandomToAdd);
        var newXVelocity = myRigidbody.velocity.x + randomToAdd;

        newXVelocity = Mathf.Abs(newXVelocity) < 0.1f ? 0.1f :
            Mathf.Abs(newXVelocity) > myRigidbody.velocity.magnitude - 0.1f ?
                newXVelocity > 0 ? myRigidbody.velocity.magnitude - 0.1f : -myRigidbody.velocity.magnitude + 0.1f :
                    newXVelocity;

        var newYVelocity = GetScaledYVelocity(newXVelocity);

        var newVelocity = GetNewVelocityByDirection(direction, newXVelocity, newYVelocity);
        newVelocity = CorrectForMovingCollision(collision, direction, newVelocity);

        myRigidbody.velocity = newVelocity;

        if (!lockedToPaddle)
        {
            PlayCollisionNoise();
        }

        stopwatch.Reset();
        stopwatch.Start();
    }

    private Vector2 CorrectForMovingCollision(Collider2D collision, Direction direction, Vector2 newVelocity)
    {
        var collisionRidgidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        if (collisionRidgidbody != null)
        {
            var collisionObjecVelocity = collisionRidgidbody.velocity;

            if ((direction == Direction.Left || direction == Direction.Right) &&
                Mathf.Abs(newVelocity.x) < Mathf.Abs(collisionObjecVelocity.x) &&
                Mathf.Sign(newVelocity.x) == Mathf.Sign(collisionObjecVelocity.x))
            {
                newVelocity.x = collisionObjecVelocity.x * 1.1f;
                newVelocity.x = Mathf.Abs(newVelocity.x) < 0.1f ? 0.1f :
                    Mathf.Abs(newVelocity.x) > myRigidbody.velocity.magnitude - 0.1f ?
                        newVelocity.x > 0 ? myRigidbody.velocity.magnitude - 0.1f : -myRigidbody.velocity.magnitude + 0.1f :
                            newVelocity.x;

                newVelocity.y = GetScaledYVelocity(newVelocity.x);
            }

            if((direction == Direction.Up || direction == Direction.Down) &&
                Mathf.Abs(newVelocity.y) < Mathf.Abs(collisionObjecVelocity.y) &&
                Mathf.Sign(newVelocity.y) == Mathf.Sign(collisionObjecVelocity.y))
            {
                newVelocity.y = collisionObjecVelocity.y * 1.1f;
                newVelocity.y = Mathf.Abs(newVelocity.y) < 0.1f ? 0.1f :
                    Mathf.Abs(newVelocity.y) > myRigidbody.velocity.magnitude - 0.1f ?
                        newVelocity.y > 0 ? myRigidbody.velocity.magnitude - 0.1f : -myRigidbody.velocity.magnitude + 0.1f :
                            newVelocity.y;

                newVelocity.x = GetScaledYVelocity(newVelocity.y);
            }
        }

        return newVelocity;
    }

    private float GetScaledYVelocity(float newXVelocity)
    {
        var newYVelocity = Mathf.Sqrt(myRigidbody.velocity.sqrMagnitude - Mathf.Pow(newXVelocity, 2));
        newYVelocity = myRigidbody.velocity.y < 0 ? -newYVelocity : newYVelocity;

        if (float.IsNaN(newYVelocity))
        {
            Debug.Log(myRigidbody.velocity.magnitude);
            Debug.Log(newXVelocity);
        }

        return newYVelocity;
    }

    private Vector2 GetNewVelocityByDirection(Direction direction, float xVelocity, float yVelocity)
    {
        if (direction == Direction.Down)
        {
            return new Vector2(xVelocity, -Mathf.Abs(yVelocity));
        }

        if (direction == Direction.Right)
        {
            return new Vector2(Mathf.Abs(xVelocity), yVelocity);
        }

        if (direction == Direction.Left)
        {
            return new Vector2(-Mathf.Abs(xVelocity), yVelocity);
        }

        if (direction == Direction.Up)
        {
            return new Vector2(xVelocity, Mathf.Abs(yVelocity));
        }

        return myRigidbody.velocity;
    }

    private void HandlePaddleCollision(Collider2D collision)
    {
        var boxCollider = (BoxCollider2D)collision;

        var relativePosition = transform.position - collision.transform.position;

        if (relativePosition.y < ((circleCollider.radius * transform.localScale.x) / 2))
        {
            return;
        }

        var maximum = myRigidbody.velocity.magnitude * 0.8f;
        var xVelocity = (relativePosition.x / ((boxCollider.size.x/2)*boxCollider.transform.localScale.x)) * maximum;

        if (xVelocity > myRigidbody.velocity.magnitude) 
        {
            xVelocity = myRigidbody.velocity.magnitude * 0.95f;
        }

        if (xVelocity < -myRigidbody.velocity.magnitude)
        {
            xVelocity = -myRigidbody.velocity.magnitude * 0.95f;
        }

        var yVelocity = GetScaledYVelocity(xVelocity);
        myRigidbody.velocity = new Vector2(xVelocity, Mathf.Abs(yVelocity));

        if (!lockedToPaddle)
        {
            PlayCollisionNoise();
        }
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
        var scale = collision.gameObject.transform.localScale;
        var boxSize = new Vector2(1.95f / 2, 0.95f / 2) * scale;

        var relativePosition = (Vector2)collision.transform.position - (Vector2)transform.position + boxSize;
        var angle = Vector2.SignedAngle(relativePosition, Vector2.right);

        if (myRigidbody.velocity.x >= 0 && myRigidbody.velocity.y > 0)
        {
            return angle > -30 && angle < 100 ? Direction.Left : Direction.Down;
        }

        if (myRigidbody.velocity.x <= 0 && myRigidbody.velocity.y > 0)
        {
            return angle > -150 && angle < 100 ? Direction.Down : Direction.Right;
        }

        if (myRigidbody.velocity.x > 0 && myRigidbody.velocity.y <= 0)
        {
            return angle > 30 ? Direction.Up : Direction.Left;
        }

        if (myRigidbody.velocity.x < 0 && myRigidbody.velocity.y <= 0)
        {
            return angle > -50 && angle < 150 ? Direction.Up : Direction.Right;
        }

        return Direction.None;
    }

    private void HandleShieldTrigger()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Abs(myRigidbody.velocity.y));
        PlayCollisionNoise();
    }

    private void PlayCollisionNoise()
    {
        if (!audioState.PlaySfx())
        {
            return;
        }

        audioSource.Play();
    }

    public void AllowLaunch(bool allowed) => allowLaunch = allowed;
}
