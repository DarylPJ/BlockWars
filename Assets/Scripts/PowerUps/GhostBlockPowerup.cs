using UnityEngine;

public class GhostBlockPowerup : PowerUps
{
    [SerializeField] private float transparentFactorChange = 1;
    [SerializeField] private float resizeTime = 10;

    protected override void HandlePaddleCollision(Collider2D collision) => blockPowerUpState.GhostBlocks(resizeTime, transparentFactorChange);
}
