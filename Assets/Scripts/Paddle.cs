using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private bool autoPlay;

    private Ball ball;

    private float cameraScale;
    private bool runningOnAndroid;

    private void Start()
    {
        var cameraSize = Camera.main.orthographicSize;
        cameraScale = cameraSize / (Screen.height / 2);

        runningOnAndroid = Application.platform == RuntimePlatform.Android;

        ball = FindObjectOfType<Ball>();
    }

    void Update()
    {
        if (runningOnAndroid && Input.touchCount == 0 && !autoPlay)
        {
            return;
        }

        var clampedXPosition = Mathf.Clamp(GetPressPosition(), 0, Screen.width);
        transform.position = new Vector2(clampedXPosition * cameraScale, transform.position.y);
    }

    private float GetPressPosition()
    {
        if (autoPlay)
        {
            return ball.transform.position.x/ cameraScale;
        }

        return runningOnAndroid ? Input.GetTouch(0).position.x : Input.mousePosition.x;
    }
}
