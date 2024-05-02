using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    private static IngameMenu _instance;
    public static IngameMenu Instance
    {
        get { return _instance; }
    }
    [SerializeField] GameObject menu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject quitMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject endMenu;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] GameObject player;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        LoadPrefs();
        endMenu.SetActive(false);
        menu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
    }
    public void EscPressed()
    {
        if (menu.activeSelf && endMenu.activeSelf)
        {
            ConfirmQuit(true);
        }
        else if(menu.activeSelf && !mainMenu.activeSelf)
        {
            Pause();
        }
        else if(menu.activeSelf && mainMenu.activeSelf && !deathMenu.activeSelf)
        {
            Continue();
        }
    }
    private void LoadPrefs()
    {
        float[] saves = GameManager.Instance.LoadPrefs();
        volumeSlider.value = saves[0];
        sensSlider.value = saves[1];
    }
    public void Continue()
    {
        player.GetComponent<Player>().paused = false;
        menu.SetActive(false);
        GameManager.Instance.Unpause();
    }
    public void Pause()
    {
        endMenu.SetActive(false);
        menu.SetActive(true);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
        GameManager.Instance.Pause();
    }
    public void Settings()
    {
        endMenu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
    }
    public void Quit()
    {
        endMenu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(true);
        deathMenu.SetActive(false);
    }
    public void End()
    {
        Pause();
        player.GetComponent<Player>().paused = true;
        menu.SetActive(true);
        endMenu.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(false);
    }
    public void ConfirmQuit(bool confirm)
    {
        if (confirm)
        {
            GameManager.Instance.LoadScene(0);
        }
        else
        {
            Pause();
        }
    }
    public void DeathMenu()
    {
        menu.SetActive(true);
        endMenu.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        deathMenu.SetActive(true);
        GameManager.Instance.Pause();
    }
    public void DeathMenuConfirm(bool confirm)
    {
        if (confirm)
        {
            GameManager.Instance.LoadScene(1);
        }
        else
        {
            GameManager.Instance.LoadScene(0);
        }
    }
    public void SavePrefs()
    {
        GameManager.Instance.SavePrefs(volumeSlider.value, sensSlider.value);
        GameManager.Instance.LoadPrefs();
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
