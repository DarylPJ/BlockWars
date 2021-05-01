using UnityEngine;

public class Shoot : PowerUps
{
    [SerializeField] private float time;

    protected override void HandlePaddleCollision(Collider2D collision)
    {
        var paddles = FindObjectsOfType<Paddle>();

        foreach (var paddle in paddles)
        {
            paddle.TurnOnShoot(time);
        }
    }
}
