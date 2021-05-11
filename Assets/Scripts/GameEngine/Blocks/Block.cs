using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] protected DirectionSprites[] spritesToUse;
    [SerializeField] private Vector2 initalVelocity = new Vector2(0, 0);
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

    protected virtual void Start()
    {
        blocksRigidbody2D = GetComponent<Rigidbody2D>();
        blocksRigidbody2D.velocity = initalVelocity;

        spriteRenderer = GetComponent<SpriteRenderer>();
        powerupOffset = transform.localScale.x;

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
        if (collision.gameObject.CompareTag("PowerUp") || collision.gameObject.GetComponent<Ball>() != null
            || blocksRigidbody2D.velocity.sqrMagnitude == 0)
        {
            return;
        }

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
