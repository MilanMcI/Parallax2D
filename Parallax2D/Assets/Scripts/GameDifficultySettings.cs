using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Header("Current Settings")]
    public Difficulty currentDifficulty = Difficulty.Medium;

    [Header("Gameplay Values")]
    public int playerMaxHealth = 100;
    public int playerDamage = 10;
    public int enemyMaxHealth = 30;

    [Header("Flash Effect")]
    [SerializeField] private Image flashOverlay;
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private float flashAlpha = 0.18f;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedDifficulty();
            ApplyDifficulty(currentDifficulty);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetEasy()
    {
        currentDifficulty = Difficulty.Easy;
        ApplyDifficulty(currentDifficulty);
        SaveDifficulty();
        StartFlash(new Color(0.7f, 1f, 0.7f));
    }

    public void SetMedium()
    {
        currentDifficulty = Difficulty.Medium;
        ApplyDifficulty(currentDifficulty);
        SaveDifficulty();
        StartFlash(new Color(1f, 0.95f, 0.6f));
    }

    public void SetHard()
    {
        currentDifficulty = Difficulty.Hard;
        ApplyDifficulty(currentDifficulty);
        SaveDifficulty();
        StartFlash(new Color(1f, 0.7f, 0.7f));
    }

    private void ApplyDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                playerMaxHealth = 150;
                playerDamage = 20;
                enemyMaxHealth = 20;
                break;

            case Difficulty.Medium:
                playerMaxHealth = 100;
                playerDamage = 10;
                enemyMaxHealth = 30;
                break;

            case Difficulty.Hard:
                playerMaxHealth = 70;
                playerDamage = 7;
                enemyMaxHealth = 45;
                break;
        }

        Debug.Log(
            $"=== DIFFICULTY SET: {difficulty} ===\n" +
            $"Player Max Health: {playerMaxHealth}\n" +
            $"Player Damage: {playerDamage}\n" +
            $"Enemy Max Health: {enemyMaxHealth}"
        );
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();

        Debug.Log("Volume set to: " + volume);
    }

    private void SaveDifficulty()
    {
        PlayerPrefs.SetInt("Difficulty", (int)currentDifficulty);
        PlayerPrefs.Save();
    }

    private void LoadSavedDifficulty()
    {
        int savedDifficulty = PlayerPrefs.GetInt("Difficulty", 1);
        currentDifficulty = (Difficulty)savedDifficulty;
    }

    private void StartFlash(Color flashColor)
    {
        if (flashOverlay == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine(flashColor));
    }

    private IEnumerator FlashRoutine(Color baseColor)
    {
        Color startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        Color peakColor = new Color(baseColor.r, baseColor.g, baseColor.b, flashAlpha);

        flashOverlay.color = startColor;
        flashOverlay.gameObject.SetActive(true);

        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.unscaledDeltaTime;
            flashOverlay.color = Color.Lerp(startColor, peakColor, timer / flashDuration);
            yield return null;
        }

        timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.unscaledDeltaTime;
            flashOverlay.color = Color.Lerp(peakColor, startColor, timer / flashDuration);
            yield return null;
        }

        flashOverlay.color = startColor;
    }
}