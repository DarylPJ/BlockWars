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
    
    protected Rigidbody2D blocksRigidbody2D;
    private SpriteRenderer spriteRenderer;
    protected LevelState levelState;

    private BlockPowerUpState blockPowerUpState;
    private AudioState audioState;

    private float powerupOffset;

    protected virtual void Start()
    {
        blocksRigidbody2D = GetComponent<Rigidbody2D>();
        blocksRigidbody2D.velocity = initalVelocity;

        spriteRenderer = GetComponent<SpriteRenderer>();

        powerupOffset = (GetComponent<BoxCollider2D>().size.x / 2);

        blockPowerUpState = FindObjectOfType<BlockPowerUpState>();
        audioState = FindObjectOfType<AudioState>();
        levelState = FindObjectOfType<LevelState>();
    }

    private void Update()
    {
        var colour = spriteRenderer.color;
        spriteRenderer.color = new Color(colour.r, colour.g, colour.b, blockPowerUpState.GetAlpha());

        UpdateSprite();
    }

    protected virtual void UpdateSprite()
    {
        if (blocksRigidbody2D.velocity.magnitude == 0)
        {
            spriteRenderer.sprite = spritesToUse.First(i => i.direction == Direction.None).sprite;
        }

        if (blocksRigidbody2D.velocity.x != 0)
        {
            spriteRenderer.sprite = blocksRigidbody2D.velocity.x < 0 ?
                spritesToUse.First(i => i.direction == Direction.Left).sprite :
                spritesToUse.First(i => i.direction == Direction.Right).sprite;
        }

        if (blocksRigidbody2D.velocity.y != 0)
        {
            spriteRenderer.sprite = blocksRigidbody2D.velocity.y < 0 ?
                spritesToUse.First(i => i.direction == Direction.Down).sprite :
                spritesToUse.First(i => i.direction == Direction.Up).sprite;
        }
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

    private void MoveBlockAway(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            return;
        }

        var relativePos = transform.position - collision.transform.position;
        var newXSpeed = Mathf.Sign(relativePos.x) * Mathf.Abs(blocksRigidbody2D.velocity.x);
        var newYSpeed = Mathf.Sign(relativePos.y) * Mathf.Abs(blocksRigidbody2D.velocity.y);

        blocksRigidbody2D.velocity = new Vector2(newXSpeed, newYSpeed);
    }

    public void DestoryBlock()
    {
        Destroy(gameObject);
    }
}
