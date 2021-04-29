using UnityEngine;

public class ScaleBall : PowerUps
{
    [SerializeField] private float fractionalResize;
    [SerializeField] private float resizeTime = 10;
    [SerializeField] private float resizeFactor = 0.5f;

    private Color powerUpColour;

    protected override void Start()
    {
        powerUpColour = GetComponent<SpriteRenderer>().color;
        base.Start();
    }

    protected override void HandlePaddleCollision(Collider2D collision)
    {
        var balls = FindObjectsOfType<Ball>();

        foreach (var ball in balls)
        {
            ball.ScaleBall(fractionalResize, resizeTime, resizeFactor, powerUpColour);
        }
    }
}
