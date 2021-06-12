using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float launchSpeed = 15;
    [SerializeField] private Collider2D leftWall;
    [SerializeField] private Collider2D rightWall;
    [SerializeField] private Collider2D roof;
    [SerializeField] private Collider2D paddleCollider;
    [SerializeField] private Collider2D floor;
    [SerializeField] private Collider2D floor2;
    [SerializeField, Range(0, 5)] private float maxRandomToAdd = 1;
    [SerializeField] private bool instaLaunch = false;
    [SerializeField] private Vector2 instaLaunchMe;
    [SerializeField] private bool noDie;
    
    [SerializeField, Range(0, 1)] private float maxXFire = 0.5f;
    [SerializeField] private Vector3 ballToPaddle;

    private bool lockedToPaddle = true;
    private bool runningOnAndroid = false;
    private bool allowLaunch = true;

    private Paddle paddle;
    private Rigidbody2D myRigidbody;
    private AudioSource audioSource;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private BallPowerUpState powerUpState;
    private AudioState audioState;
    private LevelState levelState;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        var paddles = FindObjectsOfType<Paddle>();
        paddle = paddles.OrderBy(i => i.transform.position.y).First();

        myRigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;

        powerUpState = FindObjectOfType<BallPowerUpState>();
        audioState = FindObjectOfType<AudioState>();
        levelState = FindObjectOfType<LevelState>();

        runningOnAndroid = Application.platform == RuntimePlatform.Android;
    }

    private void Update()
    {
        var color = powerUpState.GetCurrentColour();
        spriteRenderer.color = color;
        var scale = powerUpState.GetCurrentScale();
        transform.localScale = new Vector2(scale, scale);

        trailRenderer.startColor = color;
        trailRenderer.startWidth = circleCollider.radius * 2 * scale;

        if (!lockedToPaddle)
        {
            trailRenderer.enabled = true;
            return;
        }

        if (instaLaunch)
        {
            myRigidbody.velocity = instaLaunchMe;
            lockedToPaddle = false;
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

        var bossBlock = collision.GetComponent<BossBlock>();

        if (collision.gameObject.CompareTag("PowerUp") || collision.GetComponent<Ball>() ||
            (powerUpState.IsFireModeActive() &&
            collision.GetComponent<Block>() &&
            (bossBlock == null || bossBlock.Health() < 3) &&
            !collision.GetComponent<NonBreakableBlock>()))
        {
            return;
        }

        if (collision == floor || collision == floor2)
        {
            if (noDie)
            {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Abs(myRigidbody.velocity.y));
                return;
            }

            if (FindObjectsOfType<Ball>().Length == 1)
            {
                trailRenderer.enabled = false;
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
        if (collision.GetComponent<Paddle>())
        {
            HandlePaddleCollision(collision);
            return;
        }

        if (lockedToPaddle || collision.CompareTag("EnemyFloor"))
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

        if (relativePosition.y < ((circleCollider.radius * transform.localScale.x) / 2) && myRigidbody.velocity.y < 0)
        {
            return;
        }

        if (relativePosition.y > ((circleCollider.radius * transform.localScale.x) / 2) && myRigidbody.velocity.y > 0)
        {
            return;
        }

        if(myRigidbody.velocity.y > 0)
        {
            myRigidbody.velocity = new Vector2(xVelocity, -Mathf.Abs(yVelocity));
        }
        else
        {
            myRigidbody.velocity = new Vector2(xVelocity, Mathf.Abs(yVelocity));

        }

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
        var boxSize = new Vector2(1.95f, 0.95f) * scale;
        boxSize = new Vector2(Mathf.Round(boxSize.x * 10) / 10, Mathf.Round(boxSize.y * 10) / 10);

        var collisionPoint = collision.ClosestPoint(transform.position) - (Vector2)collision.transform.position;

        collisionPoint.x = collisionPoint.x < boxSize.x / 2 ? 0 : boxSize.x;
        collisionPoint.y = collisionPoint.y < boxSize.y / 2 ? 0 : boxSize.y;
        
        if (collisionPoint.x == 0 && collisionPoint.y == 0)
        {
            var pointPos = (Vector2)(transform.position - collision.transform.position);

            if (pointPos.y > pointPos.x)
            {
                return Direction.Left;
            }

            return Direction.Down;
        }

        if (collisionPoint.x == boxSize.x && collisionPoint.y == 0)
        {
            var pointPos = (Vector2)(transform.position - collision.transform.position) - new Vector2(boxSize.x, 0);

            if (pointPos.y > -pointPos.x)
            {
                return Direction.Right;
            }

            return Direction.Down;
        }

        if (collisionPoint.x == 0 && collisionPoint.y == boxSize.y)
        {
            var pointPos = (Vector2)(transform.position - collision.transform.position) - new Vector2(0, boxSize.y);

            if (pointPos.y > -pointPos.x)
            {
                return Direction.Up;
            }

            return Direction.Left;
        }

        if (collisionPoint.x == boxSize.x && collisionPoint.y == boxSize.y)
        {
            var pointPos = (Vector2)(transform.position - collision.transform.position) - new Vector2(boxSize.x, boxSize.y);

            if (pointPos.y > pointPos.x)
            {
                return Direction.Up;
            }

            return Direction.Right;
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
