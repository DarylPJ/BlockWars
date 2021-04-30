using UnityEngine;

public class BlockPowerUpState : MonoBehaviour
{
    private float currentAlpha = 1;

    private float startAlpha = 1;
    private float targetAlpha = 1;
    private float currentTimeStep;
    private float transparentFactorChange;

    private void Update()
    {
        if (currentAlpha == targetAlpha)
        {
            return;
        }

        currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, currentTimeStep);
        currentTimeStep += transparentFactorChange * Time.deltaTime;
    }

    public void GhostBlocks(float time, float transparentFactorChange)
    {
        CancelInvoke(nameof(NoGhosts));
        currentTimeStep = 0;

        startAlpha = currentAlpha;
        targetAlpha = 0;
        this.transparentFactorChange = transparentFactorChange;
        Invoke(nameof(NoGhosts), time);
    }

    private void NoGhosts()
    {
        targetAlpha = 1;
        currentTimeStep = 0;
        startAlpha = currentAlpha;
    }

    public float GetAlpha() => currentAlpha;
}

