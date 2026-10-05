using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Features.TextTypeLinker
{
    [System.Serializable]
    public class HyperlinkAction
    {
        public string LinkID;
        public UnityEvent OnClick;
    }

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPHyperlinkHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private HyperlinkAction[] hyperlinkActions;

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, null);

            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();

                foreach (var hyperlinkAction in hyperlinkActions)
                {
                    if (hyperlinkAction.LinkID == linkID)
                    {
                        hyperlinkAction.OnClick?.Invoke();
                        return;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No link clicked.");
            }
        }
    }
}