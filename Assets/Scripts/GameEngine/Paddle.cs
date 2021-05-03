using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private bool autoPlay;
    [SerializeField] private Color shootColor;
    [SerializeField] private float shootTimeSpan;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootYOffset = 0.75f;
    [SerializeField] private float shootXOffset = -0.1f;

    private Ball ball;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D myCollider;

    private float cameraScale;
    private float cameraXMin;
    private float cameraXMax;

    private bool runningOnAndroid;
    private float resizeFactor;
    private float currentTimeStep;
    private float targetXScale = 1;
    private float startXScale = 1;

    private float startAlpha = 1;
    private float targetAlpha = 1;
    private float currentAlphaTimeStep;
    private float transparentFactorChange;

    private bool shoot = false;
    private bool paddleOff = false;

    private void Start()
    {
        var cameraSize = Camera.main.orthographicSize;

        var adjustedScreenHeight = Screen.height * Camera.main.rect.height;
        cameraScale = (cameraSize * 2) / adjustedScreenHeight;
        cameraXMin = Camera.main.rect.x * Screen.width;
        cameraXMax = cameraXMin + (Camera.main.rect.width * Screen.width);

        runningOnAndroid = Application.platform == RuntimePlatform.Android;

        ball = FindObjectOfType<Ball>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();

        InvokeRepeating(nameof(Shoot), 0 , shootTimeSpan);
    }

    void Update()
    {
        if (paddleOff)
        {
            return;
        }

        if (transform.localScale.x != targetXScale)
        {
            var scale = Mathf.Lerp(startXScale, targetXScale, currentTimeStep);
            transform.localScale = new Vector2(scale, transform.localScale.y);

            currentTimeStep += resizeFactor * Time.deltaTime;
        }

        if (spriteRenderer.color.a != targetAlpha)
        {
            var alpha = Mathf.Lerp(startAlpha, targetAlpha, currentAlphaTimeStep);
            var colour = spriteRenderer.color;
            spriteRenderer.color = new Color(colour.r, colour.g, colour.b, alpha);
            currentAlphaTimeStep += transparentFactorChange * Time.deltaTime;
        }


        if (runningOnAndroid && Input.touchCount == 0 && !autoPlay)
        {
            return;
        }

        var xPosition = GetPressPosition();
        var clampedXPosition = Mathf.Clamp(xPosition, cameraXMin, cameraXMax);

        var adjustedXPosition = (clampedXPosition - cameraXMin) *cameraScale;
        transform.position = new Vector2(adjustedXPosition, transform.position.y);
    }

    private float GetPressPosition()
    {
        if (autoPlay)
        {
            return ball.transform.position.x/ cameraScale;
        }

        return runningOnAndroid ? Input.GetTouch(0).position.x : Input.mousePosition.x;
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

    public void GhostPaddle(float time, float transparentFactorChange)
    {
        CancelInvoke(nameof(NoGhosts));
        currentAlphaTimeStep = 0;

        startAlpha = spriteRenderer.color.a;
        targetAlpha = 0;
        this.transparentFactorChange = transparentFactorChange;
        Invoke(nameof(NoGhosts), time);
    }

    private void NoGhosts()
    {
        targetAlpha = 1;
        currentAlphaTimeStep = 0;
        startAlpha = spriteRenderer.color.a;
    }

    public void TurnOnShoot(float time)
    {
        CancelInvoke(nameof(TurnOffShoot));

        spriteRenderer.color = shootColor;
        shoot = true;

        Invoke(nameof(TurnOffShoot), time);
    }

    private void TurnOffShoot()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        shoot = false;
    }

    private void Shoot()
    {
        if (!shoot)
        {
            return;
        }

        var width = ((myCollider.size.x / 2) * transform.localScale.x) + shootXOffset;
        var xPositions = new float[] { transform.position.x + width, transform.position.x - width };

        foreach (var xPosition in xPositions)
        {
            var newprojectile = Instantiate(projectile);
            newprojectile.transform.position = new Vector2(xPosition, transform.position.y + shootYOffset);
        }
    }

    public void TurnOffPaddle() => paddleOff = true;

    public void TurnOnPaddle() => paddleOff = false;
}
