using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private Vector2 initalVelocity = new Vector2(0, 0);
    [SerializeField] private float angularVelocity = 0;
    [SerializeField] private Vector2 relativeOrbitPoint = new Vector2(0, 0);

    [SerializeField] protected DirectionSprites[] spritesToUse;
    [SerializeField] private AudioClip soundOnDestroy;
    [SerializeField, Range(0, 1)] private float volume = 0.5f;
    [SerializeField] private float chanceOfPowerUp = 0.1f;
    [SerializeField] private GameObject[] powerUps;

    [SerializeField] private BoxCollider2D leftBoxCollider;
    [SerializeField] private BoxCollider2D rightBoxCollider;
    [SerializeField] private BoxCollider2D upBoxCollider;
    [SerializeField] private BoxCollider2D downBoxCollider;

    [SerializeField] private SyncedMovement syncedMovement;

    protected Rigidbody2D blocksRigidbody2D;
    protected LevelState levelState;
    protected SpriteRenderer spriteRenderer;

    private BlockPowerUpState blockPowerUpState;
    private AudioState audioState;

    private float powerupOffset;
    private Vector2 orbitPoint;
    private Vector2 orbitalDirectionOfMotion = new Vector2(0, 0);

    protected virtual void Start()
    {
        if (syncedMovement)
        {
            syncedMovement.AddBlock(this);
        }

        blocksRigidbody2D = GetComponent<Rigidbody2D>();
        blocksRigidbody2D.velocity = initalVelocity;

        spriteRenderer = GetComponent<SpriteRenderer>();
        powerupOffset = transform.localScale.x;
        orbitPoint = (Vector2)transform.position + relativeOrbitPoint;

        blockPowerUpState = FindObjectOfType<BlockPowerUpState>();
        audioState = FindObjectOfType<AudioState>();
        levelState = FindObjectOfType<LevelState>();
        UpdateSprite();
    }

    protected virtual void Update()
    {
        var colour = spriteRenderer.color;
        spriteRenderer.color = new Color(colour.r, colour.g, colour.b, blockPowerUpState.GetAlpha());

        if (angularVelocity == 0)
        {
            return;
        }

        var relativePos = (Vector2)transform.position - orbitPoint;
        var theta = Mathf.Atan2(relativePos.y, relativePos.x);

        if (theta < Mathf.PI / 4 && theta > -Mathf.PI / 4)
        {
            if (angularVelocity > 0)
            {
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Up).sprite;
                return;
            }

            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Down).sprite;
            return;
        }

        if (theta > Mathf.PI / 4 && theta < (3 * Mathf.PI) / 4)
        {
            if (angularVelocity > 0)
            {
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Left).sprite;
                return;
            }

            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Right).sprite;
            return;
        }

        if (theta < -Mathf.PI / 4 && theta > -(3* Mathf.PI) / 4)
        {
            if (angularVelocity > 0)
            {
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Right).sprite;
                return;
            }

            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Left).sprite;
            return;
        }

        if (angularVelocity > 0)
        {
            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Down).sprite;
            return;
        }

        spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Up).sprite;
        return;
    }

    protected void FixedUpdate()
    {
        if (angularVelocity == 0)
        {
            return;
        }

        var relativePos = (Vector2)transform.position - orbitPoint;
        var theta = Mathf.Atan2(relativePos.y, relativePos.x);

        var newTheta = theta + ((angularVelocity/relativePos.magnitude) * Time.deltaTime);

        var newX = relativePos.magnitude * Mathf.Cos(newTheta);
        var newY = relativePos.magnitude * Mathf.Sin(newTheta);

        var newPosition = new Vector2(newX, newY) + orbitPoint;
        orbitalDirectionOfMotion = (newPosition - (Vector2)transform.position).normalized;

        blocksRigidbody2D.MovePosition(newPosition);
    }

    protected void UpdateSprite()
    {
        if (blocksRigidbody2D.velocity.x != 0 && 
            Mathf.Abs(blocksRigidbody2D.velocity.x) > Mathf.Abs(blocksRigidbody2D.velocity.y))
        {
            spriteRenderer.sprite = blocksRigidbody2D.velocity.x < 0 ?
                spritesToUse.First(i => i.direction == Direction.Left).sprite :
                spritesToUse.First(i => i.direction == Direction.Right).sprite;
            return;
        }

        if (blocksRigidbody2D.velocity.y != 0)
        {
            spriteRenderer.sprite = blocksRigidbody2D.velocity.y < 0 ?
                spritesToUse.First(i => i.direction == Direction.Down).sprite :
                spritesToUse.First(i => i.direction == Direction.Up).sprite;

            return;
        }
        
        spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.None).sprite;
    }

    protected virtual void HitByBall()
    {
        if (audioState.PlaySfx())
        {
            AudioSource.PlayClipAtPoint(soundOnDestroy, Camera.main.transform.position, volume);
        }

        levelState.BlockDestroyed(gameObject.name);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>() != null || collision.gameObject.GetComponent<Projectile>())
        {
            if (Random.value < chanceOfPowerUp)
            {
                var powerupToInstantiate = powerUps[Random.Range(0, powerUps.Length)];
                var powerup = Instantiate(powerupToInstantiate);

                powerup.transform.position = (Vector2)transform.position + new Vector2(powerupOffset, 0);
            }

            HitByBall();
            return;
        }

        MoveBlockAway(collision);                
    }

    private void OnTriggerStay2D(Collider2D collision) => MoveBlockAway(collision);

    protected void MoveBlockAway(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PowerUp") || collision.gameObject.GetComponent<Ball>() != null)
        {
            return;
        }

        if (blocksRigidbody2D.velocity.sqrMagnitude != 0)
        {
            var direction = GetDirectionToMove(collision);

            if (syncedMovement)
            {
                syncedMovement.MoveBlocksDirection(direction);
            }
            else
            {
                SetNewVelocity(direction);
            }
        }

        if (angularVelocity != 0) 
        {
            SetNewDeltaTheta(collision);
        }
    }

    private void SetNewDeltaTheta(Collider2D collision)
    {
        var leftTouching = collision.IsTouching(leftBoxCollider);
        var rightTouching = collision.IsTouching(rightBoxCollider);
        var upTouching = collision.IsTouching(upBoxCollider);
        var downTouching = collision.IsTouching(downBoxCollider);

        if (new[] { leftTouching, rightTouching, upTouching, downTouching }.Count(i => i) > 1)
        {
            return;
        }

        if (leftTouching && orbitalDirectionOfMotion.x <= 0
            || rightTouching && orbitalDirectionOfMotion.x >= 0
            || upTouching && orbitalDirectionOfMotion.y >= 0
            || downTouching && orbitalDirectionOfMotion.y <= 0)
        {
            angularVelocity = -angularVelocity;
            orbitalDirectionOfMotion = -orbitalDirectionOfMotion;
        }
    }

    private Direction GetDirectionToMove(Collider2D collision)
    {
        if (collision.IsTouching(leftBoxCollider) && blocksRigidbody2D.velocity.x != 0)
        {
            return Direction.Right;
        }

        if (collision.IsTouching(rightBoxCollider) && blocksRigidbody2D.velocity.x != 0)
        {
            return Direction.Left;
        }

        if (collision.IsTouching(upBoxCollider) && blocksRigidbody2D.velocity.y != 0)
        {
            return Direction.Down;
        }

        if (collision.IsTouching(downBoxCollider) && blocksRigidbody2D.velocity.y != 0)
        {
            return Direction.Up;
        }

        return Direction.None;
    }

    public void SetNewVelocity(Direction direction)
    {
        var newVelocity = blocksRigidbody2D.velocity;

        switch (direction)
        {
            case Direction.None:
                break;
            case Direction.Left:
                newVelocity.x = -Mathf.Abs(newVelocity.x);
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Left).sprite;
                break;
            case Direction.Right:
                newVelocity.x = Mathf.Abs(newVelocity.x);
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Right).sprite;
                break;
            case Direction.Up:
                newVelocity.y = Mathf.Abs(newVelocity.y);
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Up).sprite;
                break;
            case Direction.Down:
                newVelocity.y = -Mathf.Abs(newVelocity.y);
                spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Down).sprite;
                break;
        }

        blocksRigidbody2D.velocity = newVelocity;
    }

    public void DestoryBlock()
    {
        Destroy(gameObject);
    }
}
