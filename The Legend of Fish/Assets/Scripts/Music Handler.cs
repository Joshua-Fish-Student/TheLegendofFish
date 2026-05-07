using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicHandler : MonoBehaviour
{
    [SerializeField] Dictionary<string, AudioSource> music = new Dictionary<string, AudioSource>();
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < transform.childCount; i++)
        {
            music.Add(transform.GetChild(i).name, transform.GetChild(i).GetComponentInChildren<AudioSource>());
        }
        ResetVolume();
    }

    public void MusicSwitch(string targetToPlay)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name != targetToPlay) music[transform.GetChild(i).name].Stop();
        }
        if (!music[targetToPlay].isPlaying)music[targetToPlay].Play();
    }
    public void VolumeSwitch(float volume)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (music[transform.GetChild(i).name].isPlaying) music[transform.GetChild(i).name].volume = volume;
        }
    }
    public void VolumeSwitch()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            music[transform.GetChild(i).name].volume = slider.value;
        }
    }
    public AudioSource GetActive()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (music[transform.GetChild(i).name].isPlaying) return music[transform.GetChild(i).name];
        }
        return null;
    }
    public void ResetSlider()
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Volume", 1));
    }
    public void ResetVolume()
    {
        ResetSlider();
        VolumeSwitch();
    }
}
