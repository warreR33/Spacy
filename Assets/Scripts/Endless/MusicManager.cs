using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [System.Serializable]
    public class LevelMusic
    {
        public AudioClip clip;
    }

    public AudioClip tutorialMusic;

    public List<LevelMusic> musicByLevel;
    public AudioSource audioSource;
    public float fadeDuration = 2f;

    private Coroutine fadeRoutine;

    private void OnEnable()
    {
        PlayerStats.OnPlayerDeath += StopMusicOnDeath;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.OnStartTransition += HandleFadeOut;
            GameProgressManager.Instance.OnCompleteTransition += HandleFadeIn;

            if (GameProgressManager.Instance.currentState == GameState.Tutorial)
            {
                PlayTutorialMusic();
            }
        }
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerDeath -= StopMusicOnDeath;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.OnStartTransition -= HandleFadeOut;
            GameProgressManager.Instance.OnCompleteTransition -= HandleFadeIn;
        }
    }
    private void StopMusicOnDeath()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.OnStartTransition += HandleFadeOut;
            GameProgressManager.Instance.OnCompleteTransition += HandleFadeIn;


            if (GameProgressManager.Instance.currentState == GameState.Tutorial)
            {
                PlayTutorialMusic();
            }
        }
    }

    private void HandleFadeOut()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    private void HandleFadeIn()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        int currentLevel = GameProgressManager.Instance.currentLevel;
        fadeRoutine = StartCoroutine(FadeInNewTrack(currentLevel));
    }

    private IEnumerator FadeOutAndStop()
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private IEnumerator FadeInNewTrack(int level)
    {
        if (level == -1 && tutorialMusic != null)
        {
            audioSource.clip = tutorialMusic;
        }
        else if (level >= 0 && level < musicByLevel.Count && musicByLevel[level].clip != null)
        {
            audioSource.clip = musicByLevel[level].clip;
        }
        else
        {
            yield break; 
        }

        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0.1f;
    }

    private void OnDestroy()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.OnStartTransition -= HandleFadeOut;
            GameProgressManager.Instance.OnCompleteTransition -= HandleFadeIn;
        }
    }

    private void PlayTutorialMusic()
    {
        if (tutorialMusic == null) return;

        audioSource.clip = tutorialMusic;
        audioSource.volume = 0.1f;
        audioSource.loop = true;
        audioSource.Play();
    }
}
