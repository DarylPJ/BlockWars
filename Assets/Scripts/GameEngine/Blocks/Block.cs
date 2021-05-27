using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private Vector2 initalVelocity = new Vector2(0, 0);
    [SerializeField] private float deltaTheta = 0;
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
    }

    protected void FixedUpdate()
    {
        if (deltaTheta == 0)
        {
            return;
        }

        var relativePos = (Vector2)transform.position - orbitPoint;
        var theta = Mathf.Atan2(relativePos.y, relativePos.x);

        var newTheta = theta + deltaTheta;

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
            SetNewVelocity(collision);
        }

        if (deltaTheta != 0) 
        {
            SetNewDeltaTheta(collision);
        }
    }

    private void SetNewDeltaTheta(Collider2D collision)
    {
        // Change this to calculate the position +- delta theta. change deta theta accordingly. 
        if (collision.IsTouching(leftBoxCollider) && orbitalDirectionOfMotion.x <= 0
            || collision.IsTouching(rightBoxCollider) && orbitalDirectionOfMotion.x >= 0
            || collision.IsTouching(upBoxCollider) && orbitalDirectionOfMotion.y >= 0
            || collision.IsTouching(downBoxCollider) && orbitalDirectionOfMotion.y <= 0)
        {
            deltaTheta = -deltaTheta;
            orbitalDirectionOfMotion = -orbitalDirectionOfMotion;
        }
    }

    private void SetNewVelocity(Collider2D collision)
    {
        var newVelocity = blocksRigidbody2D.velocity;

        if (collision.IsTouching(leftBoxCollider) && blocksRigidbody2D.velocity.x != 0)
        {
            newVelocity.x = Mathf.Abs(newVelocity.x);
            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Right).sprite;
        }

        if (collision.IsTouching(rightBoxCollider) && blocksRigidbody2D.velocity.x != 0)
        {
            newVelocity.x = -Mathf.Abs(newVelocity.x);
            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Left).sprite;
        }

        if (collision.IsTouching(upBoxCollider) && blocksRigidbody2D.velocity.y != 0)
        {
            newVelocity.y = -Mathf.Abs(newVelocity.y);
            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Down).sprite;
        }

        if (collision.IsTouching(downBoxCollider) && blocksRigidbody2D.velocity.y != 0)
        {
            newVelocity.y = Mathf.Abs(newVelocity.y);
            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.Up).sprite;
        }

        blocksRigidbody2D.velocity = newVelocity;
    }

    public void DestoryBlock()
    {
        Destroy(gameObject);
    }
}
