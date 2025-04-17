using BitWave_Labs.AnimatedTextReveal;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IntroCutsceneController : MonoBehaviour
{
    [SerializeField]
    private AnimatedTextReveal AnimatedTextReveal;
    [SerializeField]
    private Button ContinueButton;

    public UnityEvent OnFinished = new UnityEvent();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.ContinueButton.gameObject.SetActive(false);
        this.ContinueButton.onClick.AddListener(this.Continue);

        this.AnimatedTextReveal.TextMesh.alignment = TextAlignmentOptions.TopLeft;

        // Set new text based on current textCount.
        var text =
            "You arrive at the corporate headquarters for a local Chicago fast food restaurant, " +
            "eager to find work after losing your data entry job to this new 'Internet' phenomenon. " +
            "Hopefully this is the last time anybody loses their job to a new technology. " +
            "They said you would be interviewing for a pretty low level position so there shouldn't " +
            "be much competition, but maybe the economy is worse than you thought...";
        this.AnimatedTextReveal.TextMesh.text = text.ToUpper();

        // Reset text visibility before starting a new animation.
        this.AnimatedTextReveal.ResetTextVisibility();
        this.AnimatedTextReveal.OnFinished.AddListener(this.TextFinished);

        // Start the new fade-in animation.
        this.AnimatedTextReveal.FadeInText();
    }

    private void TextFinished()
    {
        this.ContinueButton.gameObject.SetActive(true);
    }

    public void OnJump()
    {
        this.AnimatedTextReveal.SkipText();
    }

    private void Continue()
    {
        this.OnFinished.Invoke();
    }
}
