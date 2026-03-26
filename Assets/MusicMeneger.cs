using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Музыка для сцен")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;

    private AudioSource audioSource;
    private AudioClip currentClip;
    private float currentVolume = 0.5f;

    private const string MUSIC_KEY = "MusicVolume";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        // ПРИНУДИТЕЛЬНО СТАВИМ ГРОМКОСТЬ 0.5 (50%) ПРИ ЗАПУСКЕ
        currentVolume = 0.5f;
        audioSource.volume = currentVolume;
        PlayerPrefs.SetFloat(MUSIC_KEY, currentVolume);
        PlayerPrefs.Save();

        Debug.Log($"MusicManager создан. Громкость: {currentVolume * 100}%");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    void PlayMusicForScene(int sceneIndex)
    {
        AudioClip newClip = null;

        switch (sceneIndex)
        {
            case 0:
                newClip = mainMenuMusic;
                break;
            case 1:
                newClip = gameSceneMusic;
                break;
        }

        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            audioSource.clip = currentClip;
            audioSource.Play();
            Debug.Log($"Музыка запущена: {currentClip.name}, громкость: {currentVolume}");
        }
        else if (!audioSource.isPlaying && currentClip != null)
        {
            audioSource.Play();
        }
    }

    public void SetVolume(float newVolume)
    {
        currentVolume = Mathf.Clamp01(newVolume);
        audioSource.volume = currentVolume;

        PlayerPrefs.SetFloat(MUSIC_KEY, currentVolume);
        PlayerPrefs.Save();

        Debug.Log($"Громкость музыки: {currentVolume * 100}%");
    }

    public float GetVolume()
    {
        return currentVolume;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}