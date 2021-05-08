using UnityEngine;

public abstract class SideAvoidBlock : Block
{
    //Needed to stop squshing balls on wall.
    [SerializeField] protected float minDistToWalls = 1.5f;
    
    private Vector2 boxSize;

    protected override void Start()
    {
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

    protected RaycastHit2D? GetNoneBallHit(float distanceFromBlock)
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
}
