using UnityEngine;

public class AudioState : MonoBehaviour
{
    private const string DontPlaySfx = "DontPlaySfx";
    private const string DontPlayMusic = "DontPlayMusic";

    private bool playMusic;
    private bool playSfx;

    private AudioSource audioSource;

    private void Awake()
    {
        var gameStates = FindObjectsOfType<AudioState>();

        if (gameStates.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        playMusic = PlayerPrefs.GetInt(DontPlayMusic) == 0;
        playSfx = PlayerPrefs.GetInt(DontPlaySfx) == 0;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.time = 2f;

        if (!playMusic)
        {
            audioSource.Stop();
        }
    }

    public void ToggleMusic()
    {
        playMusic = !playMusic;
        ToggleKey(DontPlayMusic);

        if (playMusic)
        {
            audioSource.Play();
            return;
        }

        audioSource.Stop();
    }

    public void ToggleSfx()
    {
        playSfx = !playSfx;
        ToggleKey(DontPlaySfx);
    }

    private void ToggleKey(string key)
    {
        var current = PlayerPrefs.GetInt(key);

        var newValue = current == 1 ? 0 : 1;
        PlayerPrefs.SetInt(key, newValue);
        PlayerPrefs.Save();
    }


    public bool PlaySfx() => playSfx;

    public bool PlayMusic() => playMusic;
}
