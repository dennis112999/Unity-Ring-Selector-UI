using DG.Tweening;
using Dennis.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dennis.UI
{
    public class RingSelectorUI : MonoBehaviour
    {
        private enum Direction
        {
            Right = 1, // Rotate clockwise
            Left = -1, // Rotate counterclockwise
        }

        [Header("Ring Element")]
        [SerializeField] private List<RingElement> _ringElementList = new List<RingElement>();

        [Header("Ring Dimensions")]
        [SerializeField] private float _ringRadiusX;
        [SerializeField] private float _ringRadiusY;

        [Header("Animation Settings")]
        [SerializeField] private static float _rotationSpeed = 0.18f;
        [SerializeField] private float _minScaleFactor = 0.5f;

        [Header("Internal State")]
        private float _rotationOffset = 0f;
        private float _angleStep = 0f;
        private int _selectedIndex = 0;

        [Header("Cache & Animation")]
        private List<RingElement> _sortedRingElements = new List<RingElement>();
        private Tween _DoTweenAnim;

        [Header("UI")]
        [SerializeField] Button _leftButton;
        [SerializeField] Button _rightButton;

        private void Start()
        {
            // Initialization is currently placed here, but this is just a reference.
            Initialization();
        }


        private void OnDestroy()
        {
            _DoTweenAnim.Kill();
        }

        public void Initialization()
        {
            if (_ringElementList == null || _ringElementList.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("ItemList is empty. Initialization is skipped.");
#endif
                return;
            }

            _angleStep = 360.0f / _ringElementList.Count;
            _sortedRingElements.Clear();

            for (int i = 0; i < _ringElementList.Count; i++)
            {
                RingElement item = _ringElementList[i];

                if (item == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"ItemList[{i}] is null. Skipping this item.");
#endif
                    continue;
                }

                item.Degree = (_angleStep * i) + 270.0f;
                _sortedRingElements.Add(item);
                item.Init();
            }

            UpdateRingElementPosition();
            InitUI();
        }

        private void InitUI()
        {
            _leftButton.onClick.AddListener(OnTurnLeftClick);
            _rightButton.onClick.AddListener(OnTurnRightClick);
        }

        private void UpdateRingElementPosition()
        {
            foreach (RingElement item in this._ringElementList)
            {
                if (item == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Element is null.");
#endif
                    continue;
                }

                // Calculate the element's current angle
                float elementAngle = (item.Degree + _rotationOffset) % 360.0f;

                // Calculate the Z-axis depth for proper 3D layering effect
                float pos_z = Mathf.Min(Mathf.Abs((elementAngle + 90.0f) % 360.0f),
                                        360.0f - Mathf.Abs((elementAngle + 90.0f) % 360.0f));

                // Set the object's Z-axis position
                Vector3 anchoredPos = item.Rect.anchoredPosition3D;
                anchoredPos.z = pos_z;
                item.Rect.anchoredPosition3D = anchoredPos;

                // Adjust UI element scale based on depth
                Vector3 scale = item.Rect.localScale;
                scale.x = scale.y = Mathf.Lerp(_minScaleFactor, 1.0f, 1.0f - Mathf.InverseLerp(0, 180.0f, pos_z));
                item.Rect.localScale = scale;

                // Calculate the new 2D position for the UI object
                Vector2 pos = MathUtils.GetPositionFromDegrees(elementAngle);
                item.Rect.anchoredPosition = new Vector2(pos.x * _ringRadiusX, pos.y * _ringRadiusY);
            }

            // Sort elements by depth to ensure correct UI layering
            _sortedRingElements.Sort(CompareByDepth);
            int expectedIndex = (-_selectedIndex % _ringElementList.Count + _ringElementList.Count) % _ringElementList.Count;
            for (int i = 0; i < _sortedRingElements.Count; i++)
            {
                _sortedRingElements[i].Rect.SetSiblingIndex(i);

                // Reverse wrapping order: Ensure front-most element selection
                bool isSelected = (expectedIndex == _ringElementList.IndexOf(_sortedRingElements[i]));
                _sortedRingElements[i].SetInteractable(isSelected);
            }

        }

        /// <summary>
        /// Compares two ring elements based on their Z position (depth ordering)
        /// </summary>
        private int CompareByDepth(RingElement a, RingElement b)
        {
            return b.Rect.anchoredPosition3D.z.CompareTo(a.Rect.anchoredPosition3D.z);
        }

        private void RotateRing(Direction direction)
        {
            _selectedIndex += (int)direction;
            float endValue = _selectedIndex * _angleStep;

            _DoTweenAnim.Kill();

            // Animate the rotation smoothly over time
            var seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => _rotationOffset, val =>
            {
                _rotationOffset = val;
                UpdateRingElementPosition();
            }, endValue, _rotationSpeed));
            seq.AppendCallback(() => OnRotationCompleted());
            _DoTweenAnim = seq;
        }

        private void OnRotationCompleted()
        {
#if UNITY_EDITOR
            Debug.Log($"Rotation complete. Selected Index: {_selectedIndex}");
#endif

            // please add here, if need OnRotationCompleted SE
        }

        #region Button

        private void OnTurnRightClick()
        {
            RotateRing(Direction.Right);
        }

        private void OnTurnLeftClick()
        {
            RotateRing(Direction.Left);
        }

        #endregion Button
    }
}
