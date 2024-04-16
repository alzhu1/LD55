using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour {
    public static EventBus instance = null;

    public event Action OnLevelComplete = delegate {};
    public event Action OnLevelRestart = delegate {};

    public event Action<int> OnBlockInit = delegate {};
    public event Action<bool, int> OnBlockUse = delegate {};

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    public void TriggerOnLevelComplete() {
        OnLevelComplete?.Invoke();
    }

    public void TriggerOnLevelRestart() {
        OnLevelRestart?.Invoke();
    }

    public void TriggerOnBlockInit(int maxBlocks) {
        OnBlockInit?.Invoke(maxBlocks);
    }

    public void TriggerOnBlockUse(bool placed, int blocksLeft) {
        OnBlockUse?.Invoke(placed, blocksLeft);
    }
}
