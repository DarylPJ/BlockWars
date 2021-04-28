using UnityEngine;

public class PaddleResise : PowerUps
{
    [SerializeField] private float fractionalResize;
    [SerializeField] private float resizeTime = 10;
    [SerializeField] private float resizeFactor = 0.5f; 
    
    private Paddle[] paddles;

    protected override void HandlePaddleCollision(Collider2D collision)
    {
        foreach (var paddle in paddles)
        {
            paddle.TempResize(fractionalResize, resizeTime, resizeFactor);
        }
    }

    protected override void Start()
    {
        base.Start();
        paddles = FindObjectsOfType<Paddle>();
    }
}
