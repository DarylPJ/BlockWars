using TMPro;
using UnityEngine;

public class ErrorMessageManager : MonoBehaviour
{
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = "";
    }

    public void UpdateErrorMessage(string errorMessage) => text.text = errorMessage;  
}
