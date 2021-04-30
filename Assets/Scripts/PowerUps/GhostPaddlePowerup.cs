using UnityEngine;

public class GhostPaddlePowerup : PowerUps
{
    [SerializeField] private float transparentFactorChange = 1;
    [SerializeField] private float resizeTime = 10;

    protected override void HandlePaddleCollision(Collider2D collision)
    {
        var paddles = FindObjectsOfType<Paddle>();

        foreach (var paddle in paddles)
        {
            paddle.GhostPaddle(resizeTime, transparentFactorChange);
        }
    }
}
