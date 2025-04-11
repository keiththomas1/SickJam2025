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
    private Button PlayGameButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button QuitButton;

    // Debug
    [SerializeField]
    private GameObject DebugGroup;
    [SerializeField]
    private Button SpatulasButton;
    [SerializeField]
    private Button MeatballsButton;
    [SerializeField]
    private Button GreaseFryersButton;
    [SerializeField]
    private Button SodaMountainButton;
    [SerializeField]
    private Button BurgerSmashButton;

    [SerializeField]
    private ContestController ContestController;

    private bool _videoPlaying = true;
    private string _currentControlScheme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.VideoPlayer.loopPointReached += this.VideoEnded;

        this._currentControlScheme = this.PlayerInput.currentControlScheme;
        this.Canvas.enabled = false;

        this.PlayGameButton.onClick.AddListener(() => { this.ContestController.SetupGames(); });
        this.SettingsButton.onClick.AddListener(this.ShowSettings);
        this.QuitButton.onClick.AddListener(this.Quit);

#if UNITY_EDITOR 
        this.DebugGroup.SetActive(true);
        this.SpatulasButton.onClick.AddListener(() => { this.ContestController.SetupGames(MinigameType.Spatulas); });
        this.MeatballsButton.onClick.AddListener(() => { this.ContestController.SetupGames(MinigameType.Meatballs); });
        this.GreaseFryersButton.onClick.AddListener(() => { this.ContestController.SetupGames(MinigameType.GreaseFryers); });
        this.SodaMountainButton.onClick.AddListener(() => { this.ContestController.SetupGames(MinigameType.SodaMountain); });
        this.BurgerSmashButton.onClick.AddListener(() => { this.ContestController.SetupGames(MinigameType.BurgerSmash); });
#else
        this.DebugGroup.SetActive(false);
#endif
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

        this.EventSystem.SetSelectedGameObject(this.PlayGameButton.gameObject);
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
