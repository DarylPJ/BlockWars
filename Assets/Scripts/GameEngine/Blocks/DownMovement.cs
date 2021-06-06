using UnityEngine;

public class DownMovement : MonoBehaviour
{
    [SerializeField] private float maxMovedownTimePeriod = 10f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDownTime = 4f;

    private Block[] blocks;

    private void Start()
    {
        blocks = FindObjectsOfType<Block>();
        Invoke(nameof(MoveBlocksDown), Random.Range(2f, maxMovedownTimePeriod));
    }
    
    private void MoveBlocksDown()
    {
        foreach (var block in blocks)
        {
            if (block)
            {
                block.MoveDown(moveSpeed);
            }
        }
        Invoke(nameof(StopMoveDown), moveDownTime);
    }

    public void StopMoveDown()
    {
        foreach (var block in blocks)
        {
            if (block)
            {
                block.StopMoveDown();
            }
        }
        Invoke(nameof(MoveBlocksDown), Random.Range(2f, maxMovedownTimePeriod));
    }
}
