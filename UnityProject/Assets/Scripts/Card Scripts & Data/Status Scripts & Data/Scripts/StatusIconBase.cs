using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Statuses;

namespace UI
{
    public class StatusIconBase : MonoBehaviour
    {
        [SerializeField] private Image statusImage;
        [SerializeField] private TextMeshProUGUI statusValueText;

        public StatusIconData MyStatusIconData { get; private set; } = null;

        public Image StatusImage => statusImage;

        public TextMeshProUGUI StatusValueText => statusValueText;

        public void SetStatus(StatusIconData statusIconData)
        {
            MyStatusIconData = statusIconData;
            StatusImage.sprite = statusIconData.IconSprite;
/*            GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);*/
        }

        public void SetStatusValue(int statusValue)
        {
            StatusValueText.text = statusValue.ToString();
        }
    }
}
