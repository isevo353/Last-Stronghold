using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("������ ��� ����")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;

    [Header("���������")]
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool loop = true;

    private AudioSource audioSource;
    private AudioClip currentClip;

    void Awake()
    {
        // ��������
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ������� AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.loop = loop;

        // ������������� �� ������� ����� �����
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    void PlayMusicForScene(int sceneIndex)
    {
        AudioClip newClip = null;

        // ���������� ������ ��� �����
        switch (sceneIndex)
        {
            case 0: // ������� ����
                newClip = mainMenuMusic;
                break;
            case 1: // ������� �����
                newClip = gameSceneMusic;
                break;
                // ������ ������ ����� �� �������������
        }

        // ���� ������ ���������� - ������
        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            audioSource.clip = currentClip;
            audioSource.Play();
            Debug.Log($"������ �������� ��: {currentClip.name}");
        }
        // ���� ������ �� ��, �� ������������ - ����������
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

    // В методе ValueMusic() замените:
    private void ValueMusic()
    {
        // Вместо audioSource.volume = volume;
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
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
