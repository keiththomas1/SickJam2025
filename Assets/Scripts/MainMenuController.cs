using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    private GameObject MainMenuButtons;
    [SerializeField]
    private Button PlayGameButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button QuitButton;

    [SerializeField]
    private GameObject Settings;
    [SerializeField]
    private Slider MusicSlider;
    [SerializeField]
    private Slider SfxSlider;
    [SerializeField]
    private Button SettingsBackButton;

    // Debug
    [SerializeField]
    private GameObject DebugGroup;
    [SerializeField]
    private Button SpatulasButton;
    [SerializeField]
    private Button MeatballsButton;
    [SerializeField]
    private Button GreaseFryersButton;

    private bool _videoPlaying = true;
    private string _currentControlScheme;
    private float _previousSFXVolume = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.VideoPlayer.loopPointReached += this.EndVideo;

        this._currentControlScheme = this.PlayerInput.currentControlScheme;
        this.Canvas.enabled = false;

        this._videoPlaying = true;

        this.PlayGameButton.onClick.AddListener(this.PlayGame);
        this.SettingsButton.onClick.AddListener(this.ShowSettings);
        this.QuitButton.onClick.AddListener(this.Quit);

        this.MusicSlider.onValueChanged.AddListener(this.MusicChanged);
        this.SfxSlider.onValueChanged.AddListener(this.SfxChanged);
        this.SettingsBackButton.onClick.AddListener(this.ExitSettings);

#if UNITY_EDITOR
        this.DebugGroup.SetActive(true);
        this.SpatulasButton.onClick.AddListener(() => { ContestController.Instance.SetupGames(MinigameType.Spatulas); });
        this.MeatballsButton.onClick.AddListener(() => { ContestController.Instance.SetupGames(MinigameType.Meatballs); });
        this.GreaseFryersButton.onClick.AddListener(() => { ContestController.Instance.SetupGames(MinigameType.SmashBurger); });
#else
        this.DebugGroup.SetActive(false);
#endif

        if (AudioController.Instance == null)
        {
            GameObject.Instantiate(Resources.Load("AudioController") as GameObject);
        }
        if (ContestController.Instance == null)
        {
            GameObject.Instantiate(Resources.Load("ContestController") as GameObject);
        }
        else // We are returning to the main menu so no need to show boot video
        {
            this.EndVideo(null);
        }

        this.MusicSlider.value = 0.5f;
        this.SfxSlider.value = 0.5f;

        this.ShowMainMenuButtons();
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
                    this.EventSystem.SetSelectedGameObject(this.PlayGameButton.gameObject);
                }
            } else
            {
                this.EventSystem.SetSelectedGameObject(null);
            }
        }
    }

    private void PlayGame()
    {
        MusicController.Instance.FadeOutCurrentMusic(2f);
        ContestController.Instance.SetupGames();
        AudioController.Instance.LoadNewSFXAndPlay("Click", null, 1f);
    }
     
    private void EndVideo(VideoPlayer source)
    {
        if (this._videoPlaying)
        {
            this.VideoPlayer.gameObject.SetActive(false);

            this.Canvas.enabled = true;
            this.StartCoroutine(this.DelayCanvas());

            MusicController.Instance.PlayMusic("MainMenuMusic", 0.6f);

            this._videoPlaying = false;
        }
    }

    private IEnumerator DelayCanvas()
    {
        yield return new WaitForSeconds(0.4f);

        this.EventSystem.SetSelectedGameObject(this.PlayGameButton.gameObject);
    }

    private void ShowSettings()
    {
        this.MainMenuButtons.SetActive(false);
        this.Settings.SetActive(true);
        this.EventSystem.SetSelectedGameObject(this.MusicSlider.gameObject);
        AudioController.Instance.LoadNewSFXAndPlay("Click", null, 1f);
    }

    private void Quit()
    {
        AudioController.Instance.LoadNewSFXAndPlay("Click", null, 1f);
        Application.Quit();
    }

    private void MusicChanged(float value)
    {
        AudioController.Instance.SetMusicVolume(value);
        AudioController.Instance.SetAmbientVolume(value);
    }
    private void SfxChanged(float value)
    {
        AudioController.Instance.SetSFXVolume(value);

        if (Mathf.Abs(this._previousSFXVolume - value) > 0.05f) {
            this._previousSFXVolume = value;
            AudioController.Instance.LoadNewSFXAndPlay("Click", null, 1f);
        }
    }
    private void ExitSettings()
    {
        AudioController.Instance.LoadNewSFXAndPlay("Click", null, 1f);
        this.ShowMainMenuButtons();
    }

    private void ShowMainMenuButtons()
    {
        this.Settings.SetActive(false);
        this.MainMenuButtons.SetActive(true);
        this.EventSystem.SetSelectedGameObject(this.PlayGameButton.gameObject);
    }

    // Input handling

    public void OnJump(InputValue value)
    {
        this.EndVideo(null);
    }
    public void OnInteract(InputValue value)
    {
        this.EndVideo(null);
    }
    public void OnNext(InputValue value)
    {
        this.EndVideo(null);
    }
}
