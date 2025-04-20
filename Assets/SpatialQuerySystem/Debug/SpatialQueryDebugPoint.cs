using TMPro;
using UnityEngine;

namespace SpatialQuery
{
    [SelectionBase]
    public class SpatialQueryDebugPoint : MonoBehaviour
    {
        public bool IsActive { get; set; } = false;

        [Header("References")]
        [SerializeField] private TextMeshPro _valueText;
        [SerializeField] private SpriteRenderer _lineImage;
        [SerializeField] private Gradient _gradientColor;
        [SerializeField] private MeshRenderer _mesh;

        [Header("Animation")]
        [SerializeField] private Animation _popUpAnimation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;

        [Header("Values")]
        [SerializeField] private float _delayMultiplier = 1f;

        [HideInInspector] public string Report;
        
        private float _displayDuration = 4f;

        private Material _material;
        private DebugPointSettings _settings;


        private void Awake()
        {
            _material = _mesh.materials[0];
        }

        public void SetValue(DebugPointSettings settings)
        {
            _settings = settings;

            if (!settings.HidePointsAfterShowing)
            {
                Report = settings.Point.Report.ToString();
                settings.Point.Report.Clear();
            }

            Show();

            IsActive = true;

            _displayDuration = settings.Duration;

            _valueText.text = GetScoreText(settings);

            Color color = GetColor(settings);
            _lineImage.color = color;

            SetSize();

            if (_material == null)
            {
                _material = _mesh.material;
            }

            _material.color = color;
        }

        private void Show()
        {
            if (_settings.Animate)
            {
                _popUpAnimation.clip = _showClip;
                _popUpAnimation.Play();
            }
            else
            {
                transform.localScale = _settings.Point.IsWinner? Vector3.one * 1.5f : Vector3.one;
            }

            if (_settings.HidePointsAfterShowing)
            {
                Invoke(nameof(Hide), _displayDuration);
            }
        }

        public void Hide()
        {
            _popUpAnimation.clip = _hideClip;
            _popUpAnimation.Play();

            IsActive = false;
        }

        public void Hide_Debug()
        {
            transform.localScale = Vector3.zero;
            IsActive = false;
        }

        private Color GetColor(DebugPointSettings settings)
        {
            if (settings.Point.IsFilteredOut)
            {
                return Color.gray;
            }

            if(settings.Point.IsWinner)
            {
                return Color.green;
            }

            Gradient gradient = settings.UseDefaultGradient ? _gradientColor : settings.ScoreColorGradient;
            return gradient.Evaluate(settings.Point.Score);
        }

        private string GetScoreText(DebugPointSettings settings)
        {
            bool showText = settings.ScoreDisplayMode is ScoreDisplayMode.ShowScoreText or ScoreDisplayMode.ShowScoreTextAndSize;

            if(!showText)
            {
                return string.Empty;
            }

            if (settings.Point.IsFilteredOut)
            {
                return "Filtered";
            }

            if (settings.Point.IsWinner)
            {
                return  $"Winner";
            }
            else
            {
                return $"{settings.Point.Score:0.0}";
            }
        }

        private void SetSize()
        {
            if(_settings.ScoreDisplayMode is ScoreDisplayMode.ShowScoreAsSize or ScoreDisplayMode.ShowScoreTextAndSize)
            {
                transform.localScale = Vector3.one * Mathf.Lerp(_settings.ScalingRange.x, _settings.ScalingRange.y, _settings.Point.Score);
            }
        }
    }
}
