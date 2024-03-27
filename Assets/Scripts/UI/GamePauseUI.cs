using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            OptionsUI.Instance.Show(OptionsUI.Instance.gameObject, Show);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += LocalGameManagerOnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += LocalGameManagerOnLocalGameUnpaused;

        Hide();
    }

    private void LocalGameManagerOnLocalGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LocalGameManagerOnLocalGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);

        resumeButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
