using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI MinigameNameText;
    [SerializeField]
    private TextMeshProUGUI RemainingPlayersText;
    [SerializeField]
    private GameObject MinigameParent;
    [SerializeField]
    private GameObject RemainingParent;
    [SerializeField]
    private CountdownAnimator CountdownAnimator;

    public UnityEvent OnFinished = new UnityEvent();

    private void Awake()
    {
        this.CountdownAnimator.OnFinished.AddListener(() => { this.OnFinished.Invoke(); });
    }

    public void SetText(string minigameName, int playersRemaining)
    {
        this.MinigameNameText.text = minigameName;
        this.RemainingPlayersText.text = playersRemaining.ToString();

        this.MinigameParent.SetActive(true);
        this.RemainingParent.SetActive(true);
    }
}
