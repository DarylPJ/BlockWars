using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private bool autoPlay;

    private Ball ball;

    private float cameraScale;
    private bool runningOnAndroid;
    private float xOffset;

    private void Start()
    {
        var cameraSize = Camera.main.orthographicSize;
        cameraScale = cameraSize / (Screen.height / 2);

        runningOnAndroid = Application.platform == RuntimePlatform.Android;

        ball = FindObjectOfType<Ball>();
        xOffset = GetComponent<SpriteRenderer>().bounds.size.x/ 2;
    }

    void Update()
    {
        if (runningOnAndroid && Input.touchCount == 0 && !autoPlay)
        {
            return;
        }

        var xPosition = (GetPressPosition() * cameraScale) - xOffset;
        transform.position = new Vector2(xPosition, transform.position.y);
    }

    private float GetPressPosition()
    {
        if (autoPlay)
        {
            return ball.transform.position.x/ cameraScale;
        }

        return runningOnAndroid ? Input.GetTouch(0).position.x : Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
    }
}
