using UnityEngine;

public class Paddle : MonoBehaviour
{
    private float cameraScale;

    private void Start()
    {
        var cameraSize = Camera.main.orthographicSize;
        cameraScale = cameraSize / (Screen.height / 2);
    }

    void Update()
    {
        float xPosition;
        
        if (Application.platform != RuntimePlatform.Android)
        {
            xPosition = Input.mousePosition.x;
        }
        else
        {
            if (Input.touchCount == 0)
            {
                return;
            }
            else
            {
                var touch = Input.GetTouch(0);
                xPosition = touch.position.x;
            }
        }

        var clampedXPosition = Mathf.Clamp(xPosition, 0, Screen.width);
        transform.position = new Vector2(clampedXPosition * cameraScale, transform.position.y);
    }
}
