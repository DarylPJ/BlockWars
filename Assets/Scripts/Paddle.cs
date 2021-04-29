using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private bool autoPlay;

    private Ball ball;

    private float cameraScale;
    private bool runningOnAndroid;
    private float resizeFactor;
    private float currentTimeStep;
    private float targetXScale;
    private float startXScale = 1;

    private void Start()
    {
        var cameraSize = Camera.main.orthographicSize;
        cameraScale = cameraSize / (Screen.height * (Camera.main.rect.height/2));

        runningOnAndroid = Application.platform == RuntimePlatform.Android;

        ball = FindObjectOfType<Ball>();
    }

    void Update()
    {
        if (transform.localScale.x != targetXScale)
        {
            var scale = Mathf.Lerp(startXScale, targetXScale, currentTimeStep);
            transform.localScale = new Vector2(scale, transform.localScale.y);

            currentTimeStep += resizeFactor * Time.deltaTime;
        }

        if (runningOnAndroid && Input.touchCount == 0 && !autoPlay)
        {
            return;
        }

        var xPosition = GetPressPosition() * cameraScale;
        transform.position = new Vector2(xPosition - (Camera.main.rect.x * Screen.width * cameraScale), transform.position.y);
    }

    private float GetPressPosition()
    {
        if (autoPlay)
        {
            return ball.transform.position.x/ cameraScale;
        }

        return runningOnAndroid ? Input.GetTouch(0).position.x : Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
    }

    public void TempResize(float fractionWidth, float time, float resizeFactor)
    {
        CancelInvoke(nameof(ScaleBackWith));

        currentTimeStep = 0;
        startXScale = transform.localScale.x;
        targetXScale = fractionWidth;
        this.resizeFactor = resizeFactor;

        Invoke(nameof(ScaleBackWith), time);
    }

    private void ScaleBackWith()
    {
        targetXScale = 1;
        currentTimeStep = 0;
        startXScale = transform.localScale.x;
    }
}
