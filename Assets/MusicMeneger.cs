using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Музыка для сцен")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;

    [Header("Настройки")]
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool loop = true;

    private AudioSource audioSource;
    private AudioClip currentClip;

    void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Создаем AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.loop = loop;

        // Подписываемся на события смены сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    void PlayMusicForScene(int sceneIndex)
    {
        AudioClip newClip = null;

        // Определяем музыку для сцены
        switch (sceneIndex)
        {
            case 0: // Главное меню
                newClip = mainMenuMusic;
                break;
            case 1: // Игровая сцена
                newClip = gameSceneMusic;
                break;
                // Добавь другие сцены по необходимости
        }

        // Если музыка изменилась - меняем
        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            audioSource.clip = currentClip;
            audioSource.Play();
            Debug.Log($"Музыка изменена на: {currentClip.name}");
        }
        // Если музыка та же, но остановилась - продолжаем
        else if (!audioSource.isPlaying && currentClip != null)
        {
            audioSource.Play();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
