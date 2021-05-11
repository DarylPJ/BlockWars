using System.Linq;
using UnityEngine;

public class BossBlock : Block
{
    [SerializeField] private float health = 100;
    [SerializeField] private readonly float[] directionFrequencyRange = new float[] {2, 7};
    [SerializeField] private bool changeDirection = true;

    private float currentHealth;

    [SerializeField] private DirectionSprites[] broken1Sprites;
    [SerializeField] private DirectionSprites[] broken2Sprites;
    [SerializeField] private DirectionSprites[] broken3Sprites;


    protected override void Start()
    {
        if (changeDirection)
        {
            Invoke(nameof(ChangeBlockDirection), Random.Range(directionFrequencyRange[0], directionFrequencyRange[1]));
        }

        currentHealth = health;

        base.Start();
    }

    private void UpdateSpriteToUse()
    {
        if (currentHealth <= 0.75 * health && currentHealth > 0.5 * health)
        {
            spritesToUse = broken1Sprites;
        }

        if (currentHealth <= 0.5 * health && currentHealth > 0.25 * health)
        {
            spritesToUse = broken2Sprites;
        }

        if (currentHealth <= 0.25 * health && currentHealth > 0 * health)
        {
            spritesToUse = broken3Sprites;
        }
    }

    protected override void HitByBall()
    {
        currentHealth--;

        if (currentHealth == 0)
        {
            base.HitByBall();
            return;
        }

        levelState.AddBlockPoint();
        var direction = spritesToUse.First(i => i.sprite == spriteRenderer.sprite).direction; 
        UpdateSpriteToUse();

        spriteRenderer.sprite = spritesToUse.First(i => i.direction == direction).sprite;
    }

    private void ChangeBlockDirection()
    {
        Invoke(nameof(ChangeBlockDirection), Random.Range(directionFrequencyRange[0], directionFrequencyRange[1]));

        var currentV = blocksRigidbody2D.velocity;
        var sign = Mathf.Sign(Random.Range(-1, 1));

        if (Mathf.Abs(currentV.x) > 0)
        {
            blocksRigidbody2D.velocity = new Vector2(0, sign * blocksRigidbody2D.velocity.magnitude);

        }
        else
        {
            blocksRigidbody2D.velocity = new Vector2(sign * blocksRigidbody2D.velocity.magnitude, 0);
        }

        UpdateSprite();
    }

    public float Health() => currentHealth;
}
