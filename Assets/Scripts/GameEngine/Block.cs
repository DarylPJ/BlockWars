using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private DirectionSprites[] spritesToUse;
    [SerializeField] private Vector2 initalVelocity = new Vector2(0, 0);
    [SerializeField] private AudioClip soundOnDestroy;
    [SerializeField, Range(0, 1)] private float volume = 0.5f;
    [SerializeField] private float chanceOfPowerUp = 0.1f;
    [SerializeField] private GameObject[] powerUps;
    
    private Rigidbody2D blocksRigidbody2D;
    private SpriteRenderer spriteRenderer;
    private LevelState levelState;

    private BlockPowerUpState blockPowerUpState;
    private AudioState audioState;

    private float powerupOffset;

    private void Start()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>() != null || collision.gameObject.GetComponent<Projectile>())
        {
            if (audioState.PlaySfx())
            {
                AudioSource.PlayClipAtPoint(soundOnDestroy, Camera.main.transform.position, volume);
            }

            if (Random.value < chanceOfPowerUp)
            {
                var powerupToInstantiate = powerUps[Random.Range(0, powerUps.Length)];
                var powerup = Instantiate(powerupToInstantiate);

                powerup.transform.position = (Vector2)transform.position + new Vector2(powerupOffset, 0);
            }

            levelState.BlockDestroyed(gameObject.name);
            Destroy(gameObject);
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

        var relativePosition = collision.transform.position - transform.position;

        float directionToMove = 1;

        if (blocksRigidbody2D.velocity.x == 0)
        {
            directionToMove = -Mathf.Sign(relativePosition.y);
        }

        if (blocksRigidbody2D.velocity.y == 0)
        {
            directionToMove = -Mathf.Sign(relativePosition.x);
        }

        blocksRigidbody2D.velocity = directionToMove * new Vector2(Mathf.Abs(blocksRigidbody2D.velocity.x), Mathf.Abs(blocksRigidbody2D.velocity.y));
    }

    public void DestoryBlock()
    {
        Destroy(gameObject);
    }
}
