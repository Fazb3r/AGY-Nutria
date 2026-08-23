using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

[System.Serializable]
public struct IntroStep
{
    [TextArea(2, 5)]
    public string dialogText;

    public Sprite sceneImage;

    [Header("Audio (opcional)")]
    public AudioClip musicIntro;
    public AudioClip musicLoop;
    public AudioClip sfx;
}

public class IntroManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogTextField;
    public Image sceneImageField;

    [Header("Data")]
    public IntroStep[] introSteps;
    public string gameSceneName = "OtterVsRats";

    [Header("Typewriter Effect")]
    public float charactersPerSecond = 40f;

    [Header("Audio")]
    public bool sceneHasMusic = true;
    public AudioSource musicIntroSource;
    public AudioSource musicLoopSource;
    public AudioSource sfxSource;

    [Header("Volumen (0 a 1)")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private AudioClip currentMusicIntro;
    private AudioClip currentMusicLoop;

    void Start()
    {
        ApplyVolumes();

        if (introSteps != null && introSteps.Length > 0)
        {
            ShowStep(currentIndex);
        }
        else
        {
            Debug.LogWarning("IntroManager: No intro steps assigned in the inspector!");
        }
    }

    void Update()
    {
        if (introSteps == null || introSteps.Length == 0) return;

        // Click izquierdo, Espacio, Enter o Z
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
            || (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame
            || Keyboard.current.enterKey.wasPressedThisFrame
            || Keyboard.current.numpadEnterKey.wasPressedThisFrame
            || Keyboard.current.zKey.wasPressedThisFrame)))
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                AdvanceStep();
            }
        }
    }

    public void AdvanceStep()
    {
        currentIndex++;

        if (currentIndex < introSteps.Length)
        {
            ShowStep(currentIndex);
        }
        else
        {
            if (!string.IsNullOrEmpty(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName);
            }
        }
    }

    void ShowStep(int index)
    {
        if (introSteps == null || index < 0 || index >= introSteps.Length) return;

        if (dialogTextField != null)
        {
            StartTypewriter(introSteps[index].dialogText);
        }

        if (sceneImageField != null)
        {
            if (introSteps[index].sceneImage != null)
            {
                sceneImageField.gameObject.SetActive(true);
                sceneImageField.sprite = introSteps[index].sceneImage;
            }
            else
            {
                sceneImageField.gameObject.SetActive(false);
            }
        }

        PlayStepAudio(introSteps[index]);
    }

    // ---------- AUDIO ----------

    void PlayStepAudio(IntroStep step)
    {
        if (sfxSource != null && step.sfx != null)
        {
            sfxSource.PlayOneShot(step.sfx);
        }

        if (!sceneHasMusic) return;

        bool stepHasMusic = step.musicIntro != null || step.musicLoop != null;
        bool isDifferentTrack =
            step.musicIntro != currentMusicIntro ||
            step.musicLoop != currentMusicLoop;

        if (stepHasMusic && isDifferentTrack)
        {
            ChangeMusic(step.musicIntro, step.musicLoop);
        }
    }

    void ChangeMusic(AudioClip introClip, AudioClip loopClip)
    {
        musicIntroSource.Stop();
        musicLoopSource.Stop();

        currentMusicIntro = introClip;
        currentMusicLoop = loopClip;

        if (introClip != null)
        {
            musicIntroSource.clip = introClip;
            musicIntroSource.loop = false;
            musicIntroSource.Play();

            if (loopClip != null)
            {
                musicLoopSource.clip = loopClip;
                musicLoopSource.loop = true;

                double introEndTime =
                    AudioSettings.dspTime + introClip.length;

                musicLoopSource.PlayScheduled(introEndTime);
            }
        }
        else if (loopClip != null)
        {
            musicLoopSource.clip = loopClip;
            musicLoopSource.loop = true;
            musicLoopSource.Play();
        }
    }

    // ---------- VOLUMEN ----------

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplyVolumes();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        ApplyVolumes();
    }

    void ApplyVolumes()
    {
        if (musicIntroSource != null)
            musicIntroSource.volume = musicVolume;

        if (musicLoopSource != null)
            musicLoopSource.volume = musicVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    // ---------- TYPEWRITER ----------

    void StartTypewriter(string fullText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;

        dialogTextField.text = fullText;
        dialogTextField.maxVisibleCharacters = 0;

        int totalChars = fullText.Length;

        float delay =
            1f / Mathf.Max(charactersPerSecond, 1f);

        for (int i = 0; i <= totalChars; i++)
        {
            dialogTextField.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogTextField.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
    }
}