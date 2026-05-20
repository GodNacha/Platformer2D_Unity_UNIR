using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public static AudioManager instance; // Se instancia un audioMananger

    void Awake()
    {
        //Para evitar que hayan dos AudioManager y cause errores en la escena
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No destruirse al carga escena
        }
        else
        {
            Destroy(gameObject); // Se destruye si es que ya existe uno en escena
        }
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f); // El valor minimo es 0.0001 y máximo 1
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20); // Se convierte el valor a decibelios para el AudioMixer
        PlayerPrefs.SetFloat("MasterVolume", value); //Se guarda el valor en PlayerPrefs 
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

}
