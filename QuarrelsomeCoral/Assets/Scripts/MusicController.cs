using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{

    public Slider volumeSlider = null;
    public Dropdown songSelector = null;

    //Start is called before the first frame update
    void Start()
    {
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        songSelector.onValueChanged.AddListener(delegate {
            ChangeSong(songSelector);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateVolume(float value){
        GameObject musicObject = GameObject.FindWithTag("Music");
        MusicPlayer musicPlayer = musicObject.GetComponent<MusicPlayer>();
        musicPlayer.UpdateVolume(value);
    }

    void ChangeSong(Dropdown change) {
        GameObject musicObject = GameObject.FindWithTag("Music");
        MusicPlayer musicPlayer = musicObject.GetComponent<MusicPlayer>();
        musicPlayer.ChangeSong(change.options[change.value].text);
    }
}
