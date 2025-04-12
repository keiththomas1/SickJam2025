using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
    [SerializeField]
    private Button MainMenuButton;
    [SerializeField]
    private EventSystem EventSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.MainMenuButton.onClick.AddListener(this.GoToMainMenu);
        this.EventSystem.SetSelectedGameObject(this.MainMenuButton.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
