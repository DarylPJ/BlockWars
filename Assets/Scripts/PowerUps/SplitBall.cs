using UnityEngine;

public class SplitBall : PowerUps
{
    [SerializeField] GameObject ballPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (!IsPaddle(collision))
        {
            return;
        }

        var balls = FindObjectsOfType<Ball>();

        if (balls.Length > 27)
        {
            return;
        }

        foreach (var ball in balls)
        {
            var origanalRigidBody = ball.GetComponent<Rigidbody2D>();

            var ball1 = Instantiate(ball);
            ball1.NotLocked();
            ball1.transform.position = (Vector2)ball1.transform.position - new Vector2(-0.8f, 0f);

            var rigidBody1 = ball1.GetComponent<Rigidbody2D>();
            rigidBody1.velocity = origanalRigidBody.velocity;

            var ball2 = Instantiate(ball);
            ball2.NotLocked();
            ball2.transform.position = (Vector2)ball2.transform.position - new Vector2(0.8f, 0f);

            var rigidBody2 = ball2.GetComponent<Rigidbody2D>();
            rigidBody2.velocity = origanalRigidBody.velocity;
        }

        Destroy(gameObject);
    }
}
