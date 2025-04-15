using DG.Tweening;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer MeshRenderer;

    private Color _startColor;
    private Color _endColor;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#ECE36E8E", out this._startColor);
        ColorUtility.TryParseHtmlString("#B8EC6EB7", out this._endColor);

        this.MeshRenderer.material.color = this._startColor;
    }

    public void Flicker()
    {
        this.MeshRenderer.material.DORewind();
        this.MeshRenderer.material.DOColor(this._endColor, 0.35f).OnComplete(() =>
        {
            this.MeshRenderer.material.DOColor(this._startColor, 0.35f);
        });

        this.transform.DORewind();
        this.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.35f).OnComplete(() =>
        {
            this.transform.DOScale(new Vector3(1f, 1f, 1f), 0.35f);
        });
    }
}
