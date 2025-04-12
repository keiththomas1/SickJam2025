using TMPro;
using UnityEngine;

public class FinishText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    public void SetText(string text)
    {
        this.Text.text = text;
    }
}
