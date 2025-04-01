using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer VideoPlayer;

    [SerializeField]
    private Canvas Canvas;
    [SerializeField]
    private Button PlayButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button QuitButton;

    private bool _videoPlaying = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.VideoPlayer.loopPointReached += this.VideoEnded;

        this.Canvas.enabled = false;

        this.PlayButton.onClick.AddListener(this.PlayGame);
        this.SettingsButton.onClick.AddListener(this.ShowSettings);
        this.QuitButton.onClick.AddListener(this.Quit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     
    private void VideoEnded(VideoPlayer source)
    {
        if (this._videoPlaying)
        {
            this.VideoPlayer.gameObject.SetActive(false);
            this.Canvas.enabled = true;

            this._videoPlaying = false;
        }
    }

    private void PlayGame()
    {
        SceneManager.LoadScene("Playground");
    }

    private void ShowSettings()
    {

    }

    private void Quit()
    {
        Application.Quit();
    }

    // Input handling

    public void OnJump(InputValue value)
    {
        this.VideoEnded(null);
    }
    public void OnAttack(InputValue value)
    {
    }
    public void OnInteract(InputValue value)
    {
        this.VideoEnded(null);
    }
    public void OnCrouch(InputValue value)
    {
        this.VideoEnded(null);
    }
}
