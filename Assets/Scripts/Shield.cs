using UnityEngine;

public class Shield : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private float startingAlpha;
    private float minAlpha;
    private float maxAlpha;
    private float targetAlpha;
    private float transparentFactorChange;

    private float currentTimeStep;

    private bool shuttngDown = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (maxAlpha == spriteRenderer.color.a && !shuttngDown)
        {
            currentTimeStep = transparentFactorChange * Time.deltaTime;
            targetAlpha = minAlpha;
            startingAlpha = maxAlpha;
        }

        if (minAlpha == spriteRenderer.color.a && !shuttngDown)
        {
            currentTimeStep = transparentFactorChange * Time.deltaTime; 
            targetAlpha = maxAlpha;
            startingAlpha = minAlpha;
        }

        var alpha = Mathf.Lerp(startingAlpha, targetAlpha, currentTimeStep);
        currentTimeStep += transparentFactorChange * Time.deltaTime;

        if (alpha == 0)
        {
            Destroy(gameObject);
        }

        var c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
    }

    private void ShutDown()
    {
        shuttngDown = true;
        startingAlpha = spriteRenderer.color.a;
        targetAlpha = 0;
        currentTimeStep = transparentFactorChange * Time.deltaTime;
    }

    public void SetStartingAlpha(float startingAlpha, float minAlpha, float maxAlpha, bool targetMax, float transparentFactorChange, float duration)
    {
        this.startingAlpha = startingAlpha;
        this.minAlpha = minAlpha;
        this.maxAlpha = maxAlpha;

        targetAlpha = targetMax ? maxAlpha : minAlpha;
        this.transparentFactorChange = transparentFactorChange;

        Invoke(nameof(ShutDown), duration);
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

}
