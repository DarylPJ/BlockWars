using UnityEngine;

public class SplitBall : PowerUps
{
    protected override void HandlePaddleCollision(Collider2D collision)
    {
        var balls = FindObjectsOfType<Ball>();

        if (balls.Length > 9)
        {
            return;
        }

        foreach (var ball in balls)
        {
            var origanalRigidBody = ball.GetComponent<Rigidbody2D>();
            CreateBall(ball, origanalRigidBody, -20);
            CreateBall(ball, origanalRigidBody, 20);
        }

        Destroy(gameObject);
    }

    private void CreateBall(Ball origanalBall, Rigidbody2D origanalRigidBody, float rotation)
    {
        var ball = Instantiate(origanalBall);
        ball.NotLocked();

        var rigidBody1 = ball.GetComponent<Rigidbody2D>();
        var vel = origanalRigidBody.velocity;
        var theta = Mathf.Asin(vel.y / vel.magnitude) + rotation;

        rigidBody1.velocity = new Vector2(vel.magnitude * Mathf.Cos(theta), vel.magnitude * Mathf.Sin(theta));
    }
}
