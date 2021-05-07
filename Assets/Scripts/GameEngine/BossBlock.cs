using UnityEngine;

public class BossBlock : Block
{
    [SerializeField] private float health = 100;
    [SerializeField] private readonly float[] directionFrequencyRange = new float[] {2, 7};

    private float currentHealth;

    [SerializeField] private DirectionSprites[] broken1Sprites;
    [SerializeField] private DirectionSprites[] broken2Sprites;
    [SerializeField] private DirectionSprites[] broken3Sprites;

    //Needed to stop squshing balls on wall.
    [SerializeField] private float minDistToWalls = 1.5f;
    private Vector2 boxSize;

    protected override void Start()
    {
        Invoke(nameof(ChangeBlockDirection), Random.Range(directionFrequencyRange[0], directionFrequencyRange[1]));
        currentHealth = health;

        boxSize = GetComponent<BoxCollider2D>().size * transform.localScale;

        base.Start();
    }

    protected override void Update()
    {
        CastRays();
        base.Update();
    }

    private void CastRays()
    {
        var hit = GetNoneBallHit(minDistToWalls);

        if (hit != null)
        {
            MoveBlockAway(hit.Value.collider);
        }
    }

    private RaycastHit2D? GetNoneBallHit(float distanceFromBlock)
    {
        var hitsUp = Physics2D.RaycastAll(transform.position, new Vector2(0, 1), boxSize.y + distanceFromBlock);
        foreach (var hitUp in hitsUp)
        {
            if (hitUp.collider.gameObject.GetComponent<Ball>() == null && 
                hitUp.collider.gameObject.GetComponent<Block>() == null)
            {
                return hitUp;
            }
        }

        var hitsDown = Physics2D.RaycastAll(transform.position, new Vector2(0, -1), distanceFromBlock);
        foreach (var hitDown in hitsDown)
        {
            if (hitDown.collider.gameObject.GetComponent<Ball>() == null &&
                hitDown.collider.gameObject.GetComponent<Block>() == null)
            {
                return hitDown;
            }
        }

        var hitsLeft = Physics2D.RaycastAll(transform.position, new Vector2(-1, 0), +distanceFromBlock);
        foreach (var hitLeft in hitsLeft)
        {
            if (hitLeft.collider.gameObject.GetComponent<Ball>() == null &&
                hitLeft.collider.gameObject.GetComponent<Block>() == null)
            {
                return hitLeft;
            }
        }

        var hitsRight = Physics2D.RaycastAll(transform.position, new Vector2(1, 0), boxSize.x + distanceFromBlock);
        foreach (var hitRight in hitsRight)
        {
            if (hitRight.collider.gameObject.GetComponent<Ball>() == null &&
                hitRight.collider.gameObject.GetComponent<Block>() == null)
            {
                return hitRight;
            }
        }

        return null;
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
