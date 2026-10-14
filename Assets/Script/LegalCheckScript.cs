using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LegalCheckScript : MonoBehaviour
{
    private GameObject _activeLegalNotice;
    [FormerlySerializedAs("Register")]
    [SerializeField]private GameObject _registerPanel;
    private UnityEngine.UI.Toggle[] _legalConfirmationToggles;

    public bool HasAcceptedAllTerms
    {
        get
        {
            if (_legalConfirmationToggles == null || _legalConfirmationToggles.Length == 0)
            {
                return false;
            }

            foreach (UnityEngine.UI.Toggle toggle in _legalConfirmationToggles)
            {
                if (toggle == null || !toggle.isOn)
                {
                    return false;
                }
            }

            return true;
        }
    }

    private void Awake()
    {
        if (_registerPanel != null)
        {
            _legalConfirmationToggles = _registerPanel.GetComponentsInChildren<UnityEngine.UI.Toggle>(true);
        }
    }

    public void ShowLegalNotice(GameObject legalNotice)
    {
        if (legalNotice == null)
        {
            return;
        }

        CloseLegalNotice();

        _activeLegalNotice = legalNotice;
        _activeLegalNotice.transform.SetAsLastSibling();
        _activeLegalNotice.SetActive(true);

        _registerPanel.SetActive(false);

        if (_activeLegalNotice.TryGetComponent(out ScrollRect scrollRect))
        {
            scrollRect.StopMovement();
            scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }
    }

    public void CloseLegalNotice()
    {
        if (_activeLegalNotice == null)
        {
            return;
        }

        _activeLegalNotice.SetActive(false);
        _activeLegalNotice = null;
        _registerPanel.SetActive(true);
    }
}
