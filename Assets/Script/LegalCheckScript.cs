using UnityEngine;
using UnityEngine.UI;

public class LegalCheckScript : MonoBehaviour
{
    private GameObject activeLegalNotice;
    [SerializeField]private GameObject Register;
    private UnityEngine.UI.Toggle[] legalConfirmationToggles;

    public bool HasAcceptedAllTerms
    {
        get
        {
            if (legalConfirmationToggles == null || legalConfirmationToggles.Length == 0)
            {
                return false;
            }

            foreach (UnityEngine.UI.Toggle toggle in legalConfirmationToggles)
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
        if (Register != null)
        {
            legalConfirmationToggles = Register.GetComponentsInChildren<UnityEngine.UI.Toggle>(true);
        }
    }

    public void ShowLegalNotice(GameObject legalNotice)
    {
        if (legalNotice == null)
        {
            return;
        }

        CloseLegalNotice();

        activeLegalNotice = legalNotice;
        activeLegalNotice.transform.SetAsLastSibling();
        activeLegalNotice.SetActive(true);

        Register.SetActive(false);

        if (activeLegalNotice.TryGetComponent(out ScrollRect scrollRect))
        {
            scrollRect.StopMovement();
            scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }
    }

    public void CloseLegalNotice()
    {
        if (activeLegalNotice == null)
        {
            return;
        }

        activeLegalNotice.SetActive(false);
        activeLegalNotice = null;
        Register.SetActive(true);
    }
}
