using UnityEngine;

public abstract class PowerUps : MonoBehaviour
{
    [SerializeField] private float speed = -5;

    protected virtual void Start()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector2(0, speed);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }        
        
        var paddle = collision.gameObject.GetComponent<Paddle>();
        if (paddle == null)
        {
            return;
        }

        HandlePaddleCollision(collision);
        Destroy(gameObject);
    }

    protected abstract void HandlePaddleCollision(Collider2D collision);
}
