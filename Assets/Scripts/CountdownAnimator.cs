using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownAnimator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI NumberText;
    [SerializeField]
    private GameObject MinigameParent;
    [SerializeField]
    private GameObject RemainingParent;
    public UnityEvent OnFinished = new UnityEvent();

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
