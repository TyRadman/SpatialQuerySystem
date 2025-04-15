using TMPro;
using UnityEngine;

namespace SpatialQuery
{
    [SelectionBase]
    public class SpatialQueryDebugPoint : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _valueText;
        [SerializeField] private SpriteRenderer _lineImage;
        [SerializeField] private Gradient _gradientColor;
        [SerializeField] private MeshRenderer _mesh;

        [SerializeField] private Animation _popUpAnimation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;

        [SerializeField] private float _delayMultiplier = 1f;
        [SerializeField] private float _displayDuration = 4f;

        private Material _material;


        public bool IsActive { get; private set; } = false;

        private void Awake()
        {
            _material = _mesh.materials[0];
        }

        public void SetValue(SpatialQuerySamplePoint point)
        {
            //Invoke(nameof(Show), 1f * _delayMultiplier);
            Show();

            IsActive = true;

            if (point.IsFilteredOut)
            {
                _valueText.text = $"Filtered ({point.Score::0.0})";
                _lineImage.color = Color.gray;
                _material.color = Color.gray;
                return;
            }

            if(point.IsWinner)
            {
                _valueText.text = $"Winner {point.Score:0.0}";
            }
            else
            {
                _valueText.text = $"{point.Score:0.0}";
            }

            _lineImage.color = _gradientColor.Evaluate(point.Score);
            _material.color = _lineImage.color;
        }

        private void Show()
        {
            _popUpAnimation.clip = _showClip;
            _popUpAnimation.Play();

            Invoke(nameof(Hide), _displayDuration);
        }

        private void Hide()
        {
            _popUpAnimation.clip = _hideClip;
            _popUpAnimation.Play();

            IsActive = false;
        }
    }
}
