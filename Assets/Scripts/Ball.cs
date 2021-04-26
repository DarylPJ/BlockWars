using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 launch = new Vector2(2f, 10f);

    private bool lockedToPaddle = true;
    private bool runningOnAndroid = false;
    private Vector3 ballToPaddle;
   
    private Paddle paddle;
    private Rigidbody2D myRigidbody;
    private CircleCollider2D myCollider;

    void Start()
    {
        paddle = FindObjectOfType<Paddle>();
        ballToPaddle = transform.position - paddle.transform.position;
        runningOnAndroid = Application.platform == RuntimePlatform.Android;
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if(!lockedToPaddle)
        {
            return;
        }

        transform.position = paddle.transform.position + ballToPaddle;
        if (ShouldFire())
        {
            lockedToPaddle = false;
            myRigidbody.velocity = launch;
        }
    }

    private bool ShouldFire()
    {
        return true;
        if (!runningOnAndroid)
        {
            return Input.GetMouseButton(0);
        }

        if (Input.touchCount == 0)
        {
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("HideFromBall"))
        {
            Physics2D.IgnoreCollision(collision.collider, myCollider);
            return;
        }

        var boxCollider = collision.collider as BoxCollider2D;

        // must be paddle
        if (boxCollider == null)
        {
            myRigidbody.velocity *= new Vector2(1, -1);
            return;
        }

        var contact = collision.contacts[0];
        var relativePoint = contact.point - (Vector2)collision.transform.position;

        var ballRelativePoint = contact.point - (Vector2)transform.position;
        Debug.Log(Vector2.Angle(ballRelativePoint, Vector2.left));


        ChangeVelocity(relativePoint, boxCollider);
    }

    private void ChangeVelocity(Vector2 relativePoint, BoxCollider2D boxCollider)
    {
        if (ApproxametlyEqual(relativePoint.x, 0) && myRigidbody.velocity.x > 0)
        {
            myRigidbody.velocity = new Vector2(-Mathf.Abs(myRigidbody.velocity.x), myRigidbody.velocity.y);
            return;
        }

        if (ApproxametlyEqual(relativePoint.x, boxCollider.size.x) && myRigidbody.velocity.x < 0)
        {
            myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x), myRigidbody.velocity.y);
            return;
        }

        if (ApproxametlyEqual(relativePoint.y, 0) && myRigidbody.velocity.y > 0)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -Mathf.Abs(myRigidbody.velocity.y));
            return;
        }

        if (ApproxametlyEqual(relativePoint.y, boxCollider.size.y) && myRigidbody.velocity.y < 0)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Abs(myRigidbody.velocity.y));
            return;
        }
    }

    private bool ApproxametlyEqual(float numerToApproxamate, float shouldEqual) =>
        numerToApproxamate > shouldEqual - 0.2 && numerToApproxamate < shouldEqual + 0.2;
}
