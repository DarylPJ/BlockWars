using UnityEngine;

public class FreezeBlock : MonoBehaviour
{
    [SerializeField] private Block blockToFreeze;
    private BoxCollider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == nameof(Ball))
        {
            blockToFreeze.FreezeBlock();
            Destroy(gameObject);
            return;
        }

        Physics2D.IgnoreCollision(collision.GetComponent<BoxCollider2D>(), myCollider);
    }
}
