using UnityEngine;

public class GameState : MonoBehaviour
{
    private void Start()
    {
        GetComponent<AudioSource>().time = 2f;
    }
}
