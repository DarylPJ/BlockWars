using System.Collections.Generic;
using UnityEngine;

public class SyncedMovement : MonoBehaviour
{
    private readonly List<Block> blocks = new List<Block>();

    public void AddBlock(Block blockToAdd) => blocks.Add(blockToAdd);
    
    public void MoveBlocksDirection(Direction direction)
    {
        foreach (var block in blocks)
        {
            if (block)
            {
                block.SetNewVelocity(direction);
            }
        }
    }

    public void FlipBlocksAngularVelocity()
    {
        foreach (var block in blocks)
        {
            block.ChangeAngularVelocity();
        }
    }
}
