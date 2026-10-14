using UnityEngine;

public class InputUI : MonoBehaviour
{
    private void OnPasswordStart()
    {
        Input.imeCompositionMode = IMECompositionMode.Off;
    }

    private void OnPasswordEnd()
    {
        Input.imeCompositionMode = IMECompositionMode.On;
    }
}
