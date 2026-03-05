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
    private void Awake()
    {
        GetInput = GetComponent<InputSubscription>();
        heartHandler = FindObjectOfType<HeartHandler>();
        player = FindObjectOfType<Player>();
    }
    public void Save(GameObject defaultElement)
    {
        UIReturn(defaultElement);
        PlayerPrefs.SetInt("maxHearts", heartHandler.maxHealth);
        PlayerPrefs.SetFloat("currentHearts", heartHandler.currentHealth);
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
    public void ResetProgress()
    {
       PlayerPrefs.DeleteAll();
    }
}
