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

        var speedCorrection = initalVelocity.sqrMagnitude / blocksRigidbody2D.velocity.sqrMagnitude;
        blocksRigidbody2D.velocity *= speedCorrection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball")
        {
            Destroy(gameObject);
        }
    }
}
