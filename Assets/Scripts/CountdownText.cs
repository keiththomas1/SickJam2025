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
    private TextMeshProUGUI NumberText;
    [SerializeField]
    private GameObject MinigameParent;
    [SerializeField]
    private GameObject RemainingParent;

    public UnityEvent OnFinished = new UnityEvent();

    public void SetText(string minigameName, int playersRemaining)
    {
        this.MinigameNameText.text = minigameName;
        this.RemainingPlayersText.text = playersRemaining.ToString();

        this.MinigameParent.SetActive(true);
        this.RemainingParent.SetActive(true);
    }

    private void ShowNumber3()
    {
        this.NumberText.text = "3";
        AudioController.Instance.LoadNewSFXAndPlay("CountdownNumber", null, 1f);
    }
    private void ShowNumber2()
    {
        this.NumberText.text = "2";
        AudioController.Instance.LoadNewSFXAndPlay("CountdownNumber", null, 1f);
    }
    private void ShowNumber1()
    {
        this.NumberText.text = "1";
        AudioController.Instance.LoadNewSFXAndPlay("CountdownNumber", null, 1f);
    }
    private void ShowStart()
    {
        this.MinigameParent.SetActive(false);
        this.RemainingParent.SetActive(false);
        this.NumberText.text = "START!";
        AudioController.Instance.LoadNewSFXAndPlay("CountdownGo", null, 1f);
        this.OnFinished.Invoke();
    }
}
