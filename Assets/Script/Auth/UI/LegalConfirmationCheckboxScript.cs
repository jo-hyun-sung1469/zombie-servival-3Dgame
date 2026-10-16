using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(UnityEngine.UI.Toggle))]
public class LegalConfirmationCheckboxScript : MonoBehaviour
{
    [FormerlySerializedAs("legalCheck")]
    [SerializeField] private LegalCheckScript _legalCheck;
    [FormerlySerializedAs("legalNotice")]
    [SerializeField] private GameObject _legalNotice;

    public void OnToggleValueChanged(bool isOn)
    {
        if (!isOn || _legalCheck == null || _legalNotice == null)
        {
            return;
        }

        _legalCheck.ShowLegalNotice(_legalNotice);
    }
}
