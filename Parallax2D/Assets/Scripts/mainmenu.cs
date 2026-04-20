using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Settings Main Buttons")]
    [SerializeField] private GameObject difficultyButton;
    [SerializeField] private GameObject volumeButton;
    [SerializeField] private GameObject settingsReturnButton;

    [Header("Difficulty Option Buttons")]
    [SerializeField] private GameObject easyButton;
    [SerializeField] private GameObject mediumButton;
    [SerializeField] private GameObject hardButton;
    [SerializeField] private GameObject difficultyReturnButton;

    [Header("Volume Option Objects")]
    [SerializeField] private GameObject volumeText;
    [SerializeField] private GameObject volumeSlider;
    [SerializeField] private GameObject volumeReturnButton;

    private void Start()
    {
        ShowMainMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);

        ShowSettingsMainButtonsOnly();
    }

    public void OpenCredits()
    {
        mainMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OpenDifficultyMenu()
    {
        difficultyButton.SetActive(false);
        volumeButton.SetActive(false);
        settingsReturnButton.SetActive(false);

        easyButton.SetActive(true);
        mediumButton.SetActive(true);
        hardButton.SetActive(true);
        difficultyReturnButton.SetActive(true);

        volumeText.SetActive(false);
        volumeSlider.SetActive(false);
        volumeReturnButton.SetActive(false);
    }

    public void OpenVolumeMenu()
    {
        difficultyButton.SetActive(false);
        volumeButton.SetActive(false);
        settingsReturnButton.SetActive(false);

        easyButton.SetActive(false);
        mediumButton.SetActive(false);
        hardButton.SetActive(false);
        difficultyReturnButton.SetActive(false);

        volumeText.SetActive(true);
        volumeSlider.SetActive(true);
        volumeReturnButton.SetActive(true);
    }

    public void BackToSettingsMain()
    {
        ShowSettingsMainButtonsOnly();
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void ShowSettingsMainButtonsOnly()
    {
        difficultyButton.SetActive(true);
        volumeButton.SetActive(true);
        settingsReturnButton.SetActive(true);

        easyButton.SetActive(false);
        mediumButton.SetActive(false);
        hardButton.SetActive(false);
        difficultyReturnButton.SetActive(false);

        volumeText.SetActive(false);
        volumeSlider.SetActive(false);
        volumeReturnButton.SetActive(false);
    }

    private void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        ShowSettingsMainButtonsOnly();
    }
}