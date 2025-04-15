using TMPro;
using UnityEngine;

public class FinishedPlayersText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetText(int playersFinished, int total)
    {
        this.Text.text = $"{playersFinished}/{total}";
    }
}
