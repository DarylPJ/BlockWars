using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageManager : MonoBehaviour
{
    private TMP_Text text;
    [SerializeField] private Button adsButton;

    private void Awake()
    { 
        text = GetComponent<TMP_Text>();
        text.text = "";
    }

    public void ToggleAdsButton(bool active) => adsButton.interactable = active;

    public void UpdateErrorMessage(string errorMessage) => text.text = errorMessage;  
}
