using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    InputSubscription GetInput;
    HeartHandler heartHandler;
    Player player;
    MusicHandler musicHandler;
    private void Awake()
    {
        GetInput = GetComponent<InputSubscription>();
        heartHandler = FindObjectOfType<HeartHandler>();
        musicHandler = FindObjectOfType<MusicHandler>();
        player = FindObjectOfType<Player>();
    }
    public void Save(GameObject defaultElement)
    {
        if (defaultElement) {
            UIReturn(defaultElement);
            PlayerPrefs.SetInt("FromLoadingZone", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FromLoadingZone", 0);
        }
        PlayerPrefs.SetFloat("locationX", player.transform.position.x);
        PlayerPrefs.SetFloat("locationY", player.transform.position.y);
        PlayerPrefs.SetFloat("locationZ", player.transform.position.z);
        PlayerPrefs.SetString("LastSceneIn", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("maxHearts", heartHandler.maxHealth);
        PlayerPrefs.SetFloat("currentHearts", heartHandler.currentHealth);
        
        PlayerPrefs.Save();
    }
    public void SaveOnSceneLoad()
    {
        Save(null);
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", musicHandler.slider.value);
        PlayerPrefs.Save();
    }
    public void UIReturn(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(target);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void LoadLastScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("LastSceneIn", "SampleScene"));
    }
    public void ResetProgress()
    {
       PlayerPrefs.DeleteAll();
    }
}
