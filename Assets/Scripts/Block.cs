using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private DirectionSprites[] spritesToUse;
    [SerializeField] private Vector2 initalVelocity = new Vector2(0, 0);
    
    private Rigidbody2D blocksRigidbody2D;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        blocksRigidbody2D = GetComponent<Rigidbody2D>();
        blocksRigidbody2D.velocity = initalVelocity;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
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
        if (collision.gameObject.GetComponent<Ball>() != null)
        {
            Destroy(gameObject);
        }

        MoveBlockAway(collision);                
    }

    private void OnTriggerStay2D(Collider2D collision) => MoveBlockAway(collision);

    private void MoveBlockAway(Collider2D collision)
    {
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
}
