using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    public UnityEvent OnFinished = new UnityEvent();

    private void ShowNumber3()
    {
        this.Text.text = "3";
    }
    private void ShowNumber2()
    {
        this.Text.text = "2";
    }
    private void ShowNumber1()
    {
        this.Text.text = "1";
    }
    private void ShowStart()
    {
        this.Text.text = "Start!";
        this.OnFinished.Invoke();
    }
}
