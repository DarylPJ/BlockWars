using UnityEngine;

public class BallPowerUpState : MonoBehaviour
{
    private Color curentColour;
    private bool fireModeActive = false;
    private float currentScale = 1;

    private float currentTimeStep;
    private float startScale = 1;
    private float targetScale = 1;
    private float resizeFactor;

    private Color origanalSpriteColour;
    private Color currentFireModeColour;
    private Color currentScaleUpColour;

    private void Start()
    {
        origanalSpriteColour = FindObjectOfType<Ball>().GetComponent<SpriteRenderer>().color;
        curentColour = origanalSpriteColour;
    }

    private void Update()
    {
        if (currentScale == targetScale)
        {
            return;
        }

        currentScale = Mathf.Lerp(startScale, targetScale, currentTimeStep);
        currentTimeStep += resizeFactor * Time.deltaTime;
    }

    public void TriggerFireMode(Color colour, float time)
    {
        CancelInvoke(nameof(TurnOffFireMode));
        fireModeActive = true;
        currentFireModeColour = colour;
        curentColour = colour;
        Invoke(nameof(TurnOffFireMode), time);
    }

    private void TurnOffFireMode()
    {
        if (curentColour == currentFireModeColour)
        {
            curentColour = origanalSpriteColour;
        }
        fireModeActive = false;
    }

    public void ScaleBall(float scale, float time, float resizeFactor, Color colour)
    {
        CancelInvoke(nameof(ScaleBack));

        currentTimeStep = 0;
        startScale = currentScale;
        targetScale = scale;
        this.resizeFactor = resizeFactor;
        curentColour = colour;
        currentScaleUpColour = colour;

        Invoke(nameof(ScaleBack), time);
    }

    private void ScaleBack()
    {
        if (curentColour == currentScaleUpColour)
        {
            curentColour = origanalSpriteColour;
        }

        targetScale = 1;
        currentTimeStep = 0;
        startScale = currentScale;
    }

    public bool IsFireModeActive() => fireModeActive;

    public float GetCurrentScale() => currentScale;

    public Color GetCurrentColour() => curentColour;

    public void RemoveAllPowerUps()
    {
        CancelInvoke(nameof(TurnOffFireMode));
        CancelInvoke(nameof(ScaleBack));
        ScaleBack();
        TurnOffFireMode();
    }
}
