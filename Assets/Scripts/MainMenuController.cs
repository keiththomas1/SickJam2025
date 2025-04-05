using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer VideoPlayer;
    [SerializeField]
    private PlayerInput PlayerInput;

    [SerializeField]
    private Canvas Canvas;
    [SerializeField]
    private EventSystem EventSystem;
    [SerializeField]
    private Button PlaygroundButton;
    [SerializeField]
    private Button MeatballsButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button QuitButton;

    private bool _videoPlaying = true;
    private string _currentControlScheme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.VideoPlayer.loopPointReached += this.VideoEnded;

        this._currentControlScheme = this.PlayerInput.currentControlScheme;
        this.Canvas.enabled = false;

        this.PlaygroundButton.onClick.AddListener(this.PlayGame);
        this.MeatballsButton.onClick.AddListener(this.LoadMeatballMinigame);
        this.SettingsButton.onClick.AddListener(this.ShowSettings);
        this.QuitButton.onClick.AddListener(this.Quit);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.PlayerInput.currentControlScheme != this._currentControlScheme)
        {
            this._currentControlScheme = this.PlayerInput.currentControlScheme;
            if (this._currentControlScheme != "Keyboard&Mouse")
            {
                if (this.Canvas.enabled)
                {
                    this.EventSystem.SetSelectedGameObject(this.PlaygroundButton.gameObject);
                }
            } else
            {
                this.EventSystem.SetSelectedGameObject(null);
            }
        }
    }
     
    private void VideoEnded(VideoPlayer source)
    {
        if (this._videoPlaying)
        {
            this.VideoPlayer.gameObject.SetActive(false);

            this.Canvas.enabled = true;
            this.StartCoroutine(this.DelayCanvas());

            this._videoPlaying = false;
        }
    }

    private IEnumerator DelayCanvas()
    {
        yield return new WaitForSeconds(0.2f);

        this.EventSystem.SetSelectedGameObject(this.PlaygroundButton.gameObject);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene("Playground");
    }
    private void LoadMeatballMinigame()
    {
        SceneManager.LoadScene("Meatballs");
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
