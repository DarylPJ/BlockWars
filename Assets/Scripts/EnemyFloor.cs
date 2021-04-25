using UnityEngine;

public class EnemyFloor : MonoBehaviour
{
    private void Start()
    {
        var ballCollider = FindObjectOfType<Ball>().GetComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(ballCollider, GetComponent<BoxCollider2D>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        }
    }
}
