using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Toggle))]
public class LegalConfirmationCheckboxScript : MonoBehaviour
{
    [SerializeField] private LegalCheckScript legalCheck;
    [SerializeField] private GameObject legalNotice;

    public void OnToggleValueChanged(bool isOn)
    {
        if (!isOn || legalCheck == null || legalNotice == null)
        {
            return;
        }

        legalCheck.ShowLegalNotice(legalNotice);
    }
}
