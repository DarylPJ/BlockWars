using UnityEngine;

public class FireBall : PowerUps
{
    [SerializeField] private float effectTime;

    private Color powerUpColour;

    protected override void Start()
    {
        powerUpColour = GetComponent<SpriteRenderer>().color;
        base.Start();
    }

    protected override void HandlePaddleCollision(Collider2D collision) =>
        powerUpState.TriggerFireMode(powerUpColour, effectTime);
}
