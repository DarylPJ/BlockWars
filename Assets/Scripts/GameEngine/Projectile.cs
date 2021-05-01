using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float velocity;

    private float maxHeight;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, velocity);

        maxHeight = Camera.main.orthographicSize * 2;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.position.y > maxHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Block>() != null)
        {
            Destroy(gameObject);
        }
    }
}
