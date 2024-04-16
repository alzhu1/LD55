using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour {
    [SerializeField] private float fadeMultiplier = 1;

    private Image fadeImage;

    void Awake() {
        fadeImage = GetComponent<Image>();
        fadeImage.color = Color.black;
    }

    void Start() {
        StartCoroutine(FadeImage(true, -1));

        EventBus.instance.OnLevelComplete += ReceiveLevelCompleteEvent;
        EventBus.instance.OnLevelRestart += ReceiveLevelRestartEvent;
    }

    void OnDestroy() {
        EventBus.instance.OnLevelComplete -= ReceiveLevelCompleteEvent;
        EventBus.instance.OnLevelRestart -= ReceiveLevelRestartEvent;
    }

    void ReceiveLevelCompleteEvent() {
        int currIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(FadeImage(false, (currIndex + 1) % SceneManager.sceneCountInBuildSettings));
    }

    void ReceiveLevelRestartEvent() {
        StartCoroutine(FadeImage(false, SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator FadeImage(bool start, int nextLevel) {
        Color startColor = start ? Color.black : Color.clear;
        Color endColor = start ? Color.clear : Color.black;

        float t = 0;
        while (t < 1) {
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
            t += Time.deltaTime * fadeMultiplier;
        }

        fadeImage.color = endColor;

        if (nextLevel >= 0) {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
