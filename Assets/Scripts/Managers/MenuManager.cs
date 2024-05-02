using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject quitMenu;
    [SerializeField] GameObject difficultyMenu;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensSlider;
    void Start()
    {
        MainMenu();
        LoadPrefs();
    }
    public void LoadPrefs()
    {
        float[] saves = GameManager.Instance.LoadPrefs();
        volumeSlider.value = saves[0];
        sensSlider.value = saves[1];
    }
    public void Play(int difficulty)
    {
        GameManager.Instance.difficulty = difficulty;
        GameManager.Instance.LoadScene(1);
    }
    public void MainMenu()
    {
        difficultyMenu.SetActive(false);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
    }
    public void Settings()
    {
        difficultyMenu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        quitMenu.SetActive(false);
    }
    public void QuitGame()
    {
        difficultyMenu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(true);
    }
    public void SelectDifficulty()
    {
        difficultyMenu.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
    }
    public void ConfirmQuitGame(bool confirm)
    {
        if (confirm)
        {
            Application.Quit();
        }
        else
        {
            MainMenu();
        }
    }
    public void SavePrefs()
    {
        GameManager.Instance.SavePrefs(volumeSlider.value, sensSlider.value);
        GameManager.Instance.LoadPrefs();
    }
}
