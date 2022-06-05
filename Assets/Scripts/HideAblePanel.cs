using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public enum ToHideDirection
    {
        ToRight,
        ToTop,
        ToBottom,
        ToLeft
    }
    [RequireComponent(typeof(RectTransform))]
    public class HideAblePanel : MonoBehaviour
    {
        [SerializeField]
        private Button _showHideButton;
        [SerializeField]
        ToHideDirection _toHide;
        [SerializeField]
        private bool _showing = true;
        private RectTransform _thisRect;
        [SerializeField]
        private float _secondsToShowPanel = 0.3f;

        private void OnEnable()
        {
            _showHideButton.onClick.AddListener(ShowHide);
        }
        public void Start()
        {
            _thisRect = GetComponent<RectTransform>();
            if (_showing)
            {
                ShowHide();
            }
        }

        private void ShowHide()
        {
            if (_showing)
            {
                Hide();
            }
            else
            {
                Show();
            }
            _showing = !_showing;
        }

        private void Show()
        {
            StartCoroutine(ShowPanelAnimation());
        }
        private float GetMoveWeight(int countChangeScaleAnimation)
        {
            float moveweight;
            switch (_toHide)
            {
                case ToHideDirection.ToTop:
                case ToHideDirection.ToBottom:
                    {
                        moveweight = _thisRect.sizeDelta.y / countChangeScaleAnimation;
                        break;
                    }
                case ToHideDirection.ToLeft:
                case ToHideDirection.ToRight:
                default:
                    {
                        moveweight = _thisRect.sizeDelta.x / countChangeScaleAnimation;
                        break;
                    }

            }

            return moveweight;
        }

        private Vector2 GetWhatChanges(float moveweight, Vector2 pos, int i)
        {
            switch (_toHide)
            {
                case ToHideDirection.ToTop:
                    {
                        pos.y = moveweight * i;
                        break;
                    }
                case ToHideDirection.ToBottom:
                    {
                        pos.y = -moveweight * i;
                        break;
                    }
                case ToHideDirection.ToLeft:
                    {
                        pos.x = -moveweight * i;
                        break;
                    }
                case ToHideDirection.ToRight:
                default:
                    {
                        pos.x = moveweight * i;
                        break;
                    }

            }

            return pos;
        }

        IEnumerator HidePanelAnimation()
        {
            int countChangeScaleAnimation = 100;
            float SecondsToAnimate = (_secondsToShowPanel / 2) / countChangeScaleAnimation;
            float moveweight = 0;
            moveweight = GetMoveWeight(countChangeScaleAnimation);
            var pos = _thisRect.anchoredPosition;
            for (int i = 0; i < countChangeScaleAnimation; i += 1)
            {
                pos = GetWhatChanges(moveweight, pos, i);
                _thisRect.anchoredPosition = pos;
                yield return new WaitForSecondsRealtime(SecondsToAnimate);
            }
            switch (_toHide)
            {
                case ToHideDirection.ToTop:
                case ToHideDirection.ToBottom:
                    {
                        pos.y = _thisRect.sizeDelta.y;
                        break;
                    }
                case ToHideDirection.ToLeft:
                case ToHideDirection.ToRight:
                default:
                    {
                        pos.x = _thisRect.sizeDelta.x;
                        break;
                    }

            }
            _thisRect.anchoredPosition = pos;
        }

        IEnumerator ShowPanelAnimation()
        {
            int countChangeScaleAnimation = 100;
            float SecondsToAnimate = (_secondsToShowPanel / 2) / countChangeScaleAnimation;
            float moveweight = 0;
            moveweight = GetMoveWeight(countChangeScaleAnimation);
            var pos = _thisRect.anchoredPosition;
            for (int i = countChangeScaleAnimation; i > 0; i -= 1)
            {
                pos = GetWhatChanges(moveweight, pos, i);
                _thisRect.anchoredPosition = pos;
                yield return new WaitForSecondsRealtime(SecondsToAnimate);
            }
            switch (_toHide)
            {
                case ToHideDirection.ToTop:
                case ToHideDirection.ToBottom:
                    {
                        pos.y = 0;
                        break;
                    }
                case ToHideDirection.ToLeft:
                case ToHideDirection.ToRight:
                default:
                    {
                        pos.x = 0;
                        break;
                    }

            }
            _thisRect.anchoredPosition = pos;
        }

        private void Hide()
        {
            StartCoroutine(HidePanelAnimation());
        }

        private void OnDisable()
        {
            _showHideButton.onClick.RemoveListener(ShowHide);
        }
    }
}
