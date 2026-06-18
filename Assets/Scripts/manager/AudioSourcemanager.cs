using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSourcemanager : MonoBehaviour
{
    private GameObject panal_volum;
    //mixer
    private AudioMixer mixer;
    //master
    private Slider slider_master;private AudioMixerGroup group_master;private float volum_master;
    //bg
    private Slider slider_bg;private AudioSource source_bg;private AudioMixerGroup group_bg;
    //effect
    private Slider slider_effect;private List<AudioSource> list_effect;
    private AudioMixerGroup group_effect;
    // Start is called before the first frame update
    void Start()
    {
        panal_volum = transform.Find("panal_main/panal_volum").gameObject;
        list_effect = new List<AudioSource>();
        mixer = Resources.Load<AudioMixer>("mixer/mixer");
        group_master = mixer.FindMatchingGroups("Master")[0];
        group_bg = mixer.FindMatchingGroups("Bg")[0];
        group_effect = mixer.FindMatchingGroups("Effect")[0];
        slider_master = panal_volum.transform.Find("master").GetComponent<Slider>();
        slider_bg = panal_volum.transform.Find("bg").GetComponent<Slider>();
        slider_effect = panal_volum.transform.Find("effect").GetComponent<Slider>();
        slider_master.onValueChanged.AddListener(Change_master);
        slider_bg.onValueChanged.AddListener(Change_bg);
        slider_effect.onValueChanged.AddListener(Change_effect);
        source_bg = transform.AddComponent<AudioSource>();
        source_bg.outputAudioMixerGroup = group_bg;
        source_bg.loop = true;source_bg.playOnAwake = false;
        Start_gam();GameManager.Instance.Set_audio(this);
        panal_volum.SetActive(false);
        //注册开始和结束函数
        GameManager.Instance.Add_end(End_gam); GameManager.Instance.Add_start(Start_gam);
    }
    private void Change_master(float value)
    {
        volum_master = value;
        mixer.SetFloat("Master", value); 
    }
    private void Change_bg(float value)
    {
        mixer.SetFloat("Bg", value*volum_master);
    }
    private void Change_effect(float value)
    {
        mixer.SetFloat("Effect", value * volum_master);
    }
    public void Play_bg(AudioClip clip)
    {
        source_bg.Stop();
        source_bg.clip = clip;
        source_bg.Play();
    }
    public void Play_effect(AudioClip clip)
    {
        AudioSource temp=null;
        if (list_effect.Count > 0)
        {
            foreach (AudioSource op in list_effect)
            {
                if (!op.isPlaying)
                {
                    temp = op; break;
                }
            }
        }
        if (temp == null)
        {
            temp = transform.AddComponent<AudioSource>();
            temp.outputAudioMixerGroup = group_effect;
            temp.playOnAwake = false;
        }
        temp.PlayOneShot(clip);
    }
    private void Start_gam()
    {
        if (PlayerPrefs.HasKey("Master"))
            Change_master(PlayerPrefs.GetFloat("Master"));
        if (PlayerPrefs.HasKey("Bg"))
        {
            Change_bg(PlayerPrefs.GetFloat("Bg"));
        }
        if (PlayerPrefs.HasKey("Effect"))
        {
            Change_bg(PlayerPrefs.GetFloat("Effect"));
        }
    }
    private void End_gam()
    {
        PlayerPrefs.SetFloat("Master", slider_master.value);
        PlayerPrefs.SetFloat("Bg", slider_bg.value);
        PlayerPrefs.SetFloat("Effect", slider_master.value);
    }
}
