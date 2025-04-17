using BitWave_Labs.AnimatedTextReveal;
using TMPro;
using UnityEngine;

namespace BitWave_Labs.AnimatedTextReveal
{
    public class AnimateText : MonoBehaviour
    {
        [SerializeField] private AnimatedTextReveal animatedTextReveal;

        private Coroutine _fadeCoroutine;
        private int _currentLine;

        private void OnEnable()
        {
            if (!animatedTextReveal)
                animatedTextReveal = GetComponent<AnimatedTextReveal>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space))
                return;

            animatedTextReveal.TextMesh.alignment = TextAlignmentOptions.TopLeft;

            // Set new text based on current textCount.
            animatedTextReveal.TextMesh.text = _currentLine switch
            {
                0 => "1: Smoothly reveal text from left to right",
                1 =>
                    "2: Supports multi-line text animations, allowing you to display longer sentences that seamlessly fade in across multiple lines for a dynamic and engaging effect.",
                2 => "3: Fully customizable speed and spread.",
                3 => "4: Works with any TextMeshProUGUI component.",
                4 => "5: Perfect for dialogue, UI effects, and more!",
                _ => animatedTextReveal.TextMesh.text
            };

            _currentLine = (_currentLine + 1) % 5; // Cycle textCount back to 0 after 4.

            // Stop the currently running coroutine if there is one.
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            // Reset text visibility before starting a new animation.
            animatedTextReveal.ResetTextVisibility();

            // Start the new fade-in animation.
            // _fadeCoroutine = StartCoroutine(animatedTextReveal.FadeInText());
        }
    }
}