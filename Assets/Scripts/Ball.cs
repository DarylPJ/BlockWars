using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 launch = new Vector2(2f, 10f);


    private bool lockedToPaddle = true;
    private bool runningOnAndroid = false;
    private Vector3 ballToPaddle;
   

    private Paddle paddle;
    private Rigidbody2D rigidbodyComponant;

    void Start()
    {
        paddle = FindObjectOfType<Paddle>();
        ballToPaddle = transform.position - paddle.transform.position;
        runningOnAndroid = Application.platform == RuntimePlatform.Android;
        rigidbodyComponant = GetComponent<Rigidbody2D>();
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
            rigidbodyComponant.velocity = launch;
        }
    }

    private bool ShouldFire()
    {
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
}
