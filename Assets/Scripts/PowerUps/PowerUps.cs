using UnityEngine;

public abstract class PowerUps : MonoBehaviour
{
    [SerializeField] private float speed = -5;

    private void Start()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector2(0, speed);
    }
    
    protected bool IsPaddle(Collider2D collision)
    {
        var paddle = collision.gameObject.GetComponent<Paddle>();
        return paddle != null;
    }
}
