using UnityEngine;

public class Link : MonoBehaviour
{
    public void OpenLink(string url) => Application.OpenURL(url);
}
