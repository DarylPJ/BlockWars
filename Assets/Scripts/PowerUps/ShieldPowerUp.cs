using UnityEngine;

public class ShieldPowerUp : PowerUps
{
    [SerializeField] private GameObject shield;
    [SerializeField] private float startingAlpha;
    [SerializeField] private float minAlpha;
    [SerializeField] private float maxAlpha;
    [SerializeField] private bool targetMax;
    [SerializeField] private float transparentFactorChange;
    [SerializeField] private float durationMinusShutDownTime;

    protected override void HandlePaddleCollision(Collider2D collision) 
    {
        var existingShield = FindObjectOfType<Shield>();
        float? existingAlpha = null;
        
        if (existingShield != null)
        {
            existingAlpha = existingShield.GetComponent<SpriteRenderer>().color.a;
            existingShield.DestroyMe();
        }

        var newShield = Instantiate(shield);
        var script = newShield.GetComponent<Shield>();

        script.SetStartingAlpha(existingAlpha != null? existingAlpha.Value: startingAlpha, minAlpha, maxAlpha, targetMax, transparentFactorChange, durationMinusShutDownTime);
    }
}
