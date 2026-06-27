using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    public Slider music;
    public Slider sfx;
    public float musicVolume;
    public float sfxVolume;
    public TMP_Text title;
    public float flashingInterval;
    private bool menuOpen = true;
    public GameObject container;
    public TMP_Text menuButton;
    public EventSystem eventSystem;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(eventSystem);
        StartCoroutine(TitleAnimation());
        //LoadLevel("Level One");
    }
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
        ToggleMenu();
    }

    public void SetMusicVolume()
    {
        musicVolume = music.value;
        Debug.Log(musicVolume);
    }
    public void SetSFXVolume()
    {
        sfxVolume = sfx.value;
        Debug.Log(sfxVolume);
    }

    IEnumerator TitleAnimation()
    {
        while(true)
        {
            yield return new WaitForSeconds(flashingInterval);
            title.text = "Tension\nAssisted\nSpaceflight";
            yield return new WaitForSeconds(flashingInterval);
            title.text = "Tension\nAssisted\nSpaceflight_";
        }

    }
    public void ToggleMenu()
    {
        menuOpen = !menuOpen;
        container.SetActive(menuOpen);
        if(menuOpen)
        {
            menuButton.text = "X";
        }
        else
        {
            menuButton.text = "Menu";
        }
    }
}
