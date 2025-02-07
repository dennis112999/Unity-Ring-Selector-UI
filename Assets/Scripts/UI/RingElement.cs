using UnityEngine;
using UnityEngine.UI;

namespace Dennis.UI
{
    public class RingElement : MonoBehaviour
    {
        public float Degree { get; set; }

        public RectTransform Rect => GetComponent<RectTransform>();

        [Header("UI")]
        [SerializeField] private Button _button;

        public void Init()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        public void SetInteractable(bool isActive)
        {
            _button.interactable = isActive;
        }

        #region Button

        private void OnButtonClick()
        {
#if UNITY_EDITOR
            Debug.Log($"Clicked : {gameObject.name}");
#endif
        }

        #endregion Button
    }
}