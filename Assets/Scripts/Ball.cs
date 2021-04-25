using UnityEngine;

public class Ball : MonoBehaviour
{
    private Paddle paddle;

    private bool lockedToPaddle = true;
    private Vector3 ballToPaddle;

    void Start()
    {
        paddle = FindObjectOfType<Paddle>();
        ballToPaddle = transform.position - paddle.transform.position;
    }

    void Update()
    {
        if (lockedToPaddle)
        {
            transform.position = paddle.transform.position + ballToPaddle;
        }   
    }
}
