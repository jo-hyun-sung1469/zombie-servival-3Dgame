using UnityEngine;
using UnityEngine.UI;

public class LegalCheckScript : MonoBehaviour
{
    private GameObject activeLegalNotice;

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
    }
}
