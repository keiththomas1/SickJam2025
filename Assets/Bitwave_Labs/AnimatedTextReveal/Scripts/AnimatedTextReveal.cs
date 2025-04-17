using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BitWave_Labs.AnimatedTextReveal
{
    /// <summary>
    /// This class animates the fade-in effect of a TextMeshProUGUI component, 
    /// smoothly revealing the text from left to right with adjustable speed and spread.
    /// </summary>
    public class AnimatedTextReveal : MonoBehaviour
    {
        /// <summary>
        /// The TextMeshProUGUI component to animate.
        /// </summary>
        [SerializeField] private TextMeshProUGUI textMesh;

        /// <summary>
        /// The speed at which the text fades in. Higher values result in faster fading.
        /// </summary>
        [SerializeField] private float fadeSpeed = 20.0f;

        /// <summary>
        /// The number of characters affected at a time, creating a smoother transition effect.
        /// </summary>
        [SerializeField] private int characterSpread = 10;

        /// <summary>
        /// Stores the running coroutine instance.
        /// </summary>
        private Coroutine _fadeCoroutine;

        /// <summary>
        /// Tracks which text to display.
        /// </summary>
        private int _textCount;
        
        public TextMeshProUGUI TextMesh => textMesh;

        private float TYPE_DELAY = 0.02f;
        private float _typingTimer = 0;

        public UnityEvent OnFinished = new UnityEvent();

        private void Start()
        {
        }

        private void Update()
        {
            if (this._typingTimer > 0f)
            {
                this._typingTimer -= Time.deltaTime;

                if (this._typingTimer <= 0f)
                {
                    this.TypeOutText();
                }
            }
        }

        /// <summary>
        /// Coroutine to gradually fade in the text from left to right.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        public void FadeInText()
        {
            // Ensure the text mesh updates immediately so we get valid character data.
            textMesh.ForceMeshUpdate();
            // Start by setting the text fully transparent.
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);

            this.TypeOutText();
        }

        private int _visibleCharacters = 0;

        private void TypeOutText()
        {
            TMP_TextInfo textInfo = textMesh.textInfo;

            // Holds the vertex color data for modifying character transparency.
            Color32[] newVertexColors = null;

            // Get the total number of characters in the text.
            int totalCharacters = textInfo.characterCount;

            bool fullyRevealed = false; // Determines when the text is fully revealed.

            // Determines how much each character fades per update cycle.
            byte fadeAmount = (byte)Mathf.Max(1, 255 / characterSpread);

            // Loop through visible characters up to the current limit.
            for (int i = 0; i < this._visibleCharacters + 1 && i < totalCharacters; i++)
            {
                // Skip characters that are not visible (like spaces or line breaks).
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                // Get the material index for the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Retrieve the vertex color array for this character's material.
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the starting vertex index for this character.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Increase the alpha value of the character gradually to make it visible.
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex].a + fadeAmount, 0, 255);

                // Apply the new alpha value to all four vertices of the character.
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;
            }

            // Apply the modified vertex colors to the text mesh.
            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // Increase the number of characters that are being revealed.
            if (this._visibleCharacters < totalCharacters)
                this._visibleCharacters++;

            // Determine if all characters are fully visible.
            fullyRevealed = this._visibleCharacters >= totalCharacters && newVertexColors != null &&
                            newVertexColors[textInfo.characterInfo[totalCharacters - 1].vertexIndex].a == 255;

            if (!fullyRevealed)
            {
                this._typingTimer = TYPE_DELAY;
            } else
            {
                this.OnFinished.Invoke();
            }
        }

        public void SkipText()
        {
            this._visibleCharacters = textMesh.textInfo.characterCount;
            this._typingTimer = 0f;
            this.TypeOutText();
        }

        /// <summary>
        /// Resets all characters to be fully transparent before starting the animation.
        /// </summary>
        public void ResetTextVisibility()
        {
            textMesh.ForceMeshUpdate(); // Ensure valid text data.
            TMP_TextInfo textInfo = textMesh.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue; // Skip spaces or hidden characters.

                int materialIndex = charInfo.materialReferenceIndex;
                Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = charInfo.vertexIndex;

                // Set all vertices to fully transparent.
                newVertexColors[vertexIndex + 0].a = 0;
                newVertexColors[vertexIndex + 1].a = 0;
                newVertexColors[vertexIndex + 2].a = 0;
                newVertexColors[vertexIndex + 3].a = 0;
            }

            // Apply the reset vertex colors.
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            }
        }
    }
}