using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHandler : MonoBehaviour
{
    public static HeartHandler instance;
    [SerializeField] Image[] images;
    public float currentHealth;
    public int maxHealth;
    Player player;
    [SerializeField] Sprite full;
    [SerializeField] Sprite empty;
    [SerializeField] Sprite half;
    //private void Awake()
    //{
    //    if (instance != null && instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}
    void Start()
    {
        images = new Image[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            images[i] = transform.GetChild(i).GetComponentInChildren<Image>();
        }
        player = FindObjectOfType<Player>();
        maxHealth = PlayerPrefs.GetInt("maxHearts", 5);
        currentHealth = PlayerPrefs.GetFloat("currentHearts", maxHealth);
        player.GetComponent<Health>().health = (int) Mathf.Round(currentHealth * 10f);
        UpdateHearts(player.GetComponent<Health>().health);
    }
    public void UpdateHearts(float current)
    {
        currentHealth = Mathf.Floor(current / 10f);
        if (current % 5 == 0 && !(current % 10 == 0)) currentHealth += 0.5f;
        int i = 0;
        foreach(Image image in images)
        {
            if(!image.isActiveAndEnabled) image.gameObject.SetActive(true);
            if (currentHealth % 1 != 0 && (i + 0.5 == currentHealth)) image.sprite = half;
            else if (i + 1 < currentHealth) image.sprite = full;
            else if (!(i + 1 > maxHealth) && (i + 1 > currentHealth)) image.sprite = empty;
            else if (i + 1 > maxHealth) image.gameObject.SetActive(false);
            i++;
        }
    }
}
