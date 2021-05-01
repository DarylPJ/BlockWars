using UnityEngine;
using UnityEngine.UI;

public class MenuAudioState : MonoBehaviour
{
    [SerializeField] private Image menuSfxImage;
    [SerializeField] private Image menuMusicImage;

    [SerializeField] private Sprite playSfxSprite;
    [SerializeField] private Sprite dontPlaySfxSprite;
    [SerializeField] private Sprite playMusicSprite;
    [SerializeField] private Sprite dontPlayMusicSprite;

    private AudioState audioState;
    private AudioSource sfxAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioState = FindObjectOfType<AudioState>();
        sfxAudioSource = GetComponent<AudioSource>();

        SetAudioImages();
    }

    private void SetAudioImages()
    {
        if (audioState.PlayMusic())
        {
            menuMusicImage.sprite = playMusicSprite;
        }
        else
        {
            menuMusicImage.sprite = dontPlayMusicSprite;
        }

        if (audioState.PlaySfx())
        {
            menuSfxImage.sprite = playSfxSprite;
        }
        else
        {
            menuSfxImage.sprite = dontPlaySfxSprite;
        }
    }

    public void ToggleMusic()
    {
        audioState.ToggleMusic();
        SetAudioImages();
    }

    public void ToggleSfx()
    {
        sfxAudioSource.Play();
        audioState.ToggleSfx();
        SetAudioImages();
    }
}
