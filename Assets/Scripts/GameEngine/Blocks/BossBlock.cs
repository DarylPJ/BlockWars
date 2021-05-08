using UnityEngine;

public class BossBlock : SideAvoidBlock
{
    [SerializeField] private float health = 100;
    [SerializeField] private readonly float[] directionFrequencyRange = new float[] {2, 7};

    private float currentHealth;

    [SerializeField] private DirectionSprites[] broken1Sprites;
    [SerializeField] private DirectionSprites[] broken2Sprites;
    [SerializeField] private DirectionSprites[] broken3Sprites;


    protected override void Start()
    {
        Invoke(nameof(ChangeBlockDirection), Random.Range(directionFrequencyRange[0], directionFrequencyRange[1]));
        currentHealth = health;

        base.Start();
    }

    protected override void UpdateSprite()
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

        base.UpdateSprite();
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
    }

    private void ChangeBlockDirection()
    {
        Invoke(nameof(ChangeBlockDirection), Random.Range(directionFrequencyRange[0], directionFrequencyRange[1]));

        var hits = GetNoneBallHit(minDistToWalls + 0.1f);
        if (hits != null)
        {
            return;
        }

        var currentV = blocksRigidbody2D.velocity;
        var sign = Mathf.Sign(Random.Range(-1, 1));

        if (Mathf.Abs(currentV.x) > 0)
        {
            blocksRigidbody2D.velocity = new Vector2(0, sign * blocksRigidbody2D.velocity.magnitude);
            return;
        }

        blocksRigidbody2D.velocity = new Vector2(sign * blocksRigidbody2D.velocity.magnitude, 0);
    }
}
