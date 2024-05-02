using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public float mouseSens;
    public float volume;
    bool load;
    public bool loudShot;
    public int difficulty;
    [SerializeField] AudioMixer audioMixer;
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
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        if(!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1f);
        }
        if (!PlayerPrefs.HasKey("Sens"))
        {
            PlayerPrefs.SetFloat("Sens", 50f);
        }
    }
    public float[] LoadPrefs()
    {
        volume = PlayerPrefs.GetFloat("Volume");
        mouseSens = PlayerPrefs.GetFloat("Sens");
        float[] prefsValues = { volume, mouseSens };
        load = true;
        return prefsValues;
    }
    void Update()
    {
        if(load)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            load = false;
        }
    }
    public void Pause()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Unpause()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void SavePrefs(float volumeInput, float sens)
    {
        volume = volumeInput;
        mouseSens = sens;
        PlayerPrefs.SetFloat("Volume", volumeInput);
        PlayerPrefs.SetFloat("Sens", sens);
        PlayerPrefs.Save();
    }
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
