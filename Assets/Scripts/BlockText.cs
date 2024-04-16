using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockText : MonoBehaviour {
    private TextMeshProUGUI blockText;

    void Awake() {
        blockText = GetComponent<TextMeshProUGUI>();
    }

    void Start() {
        EventBus.instance.OnBlockInit += ReceiveBlockInitEvent;
        EventBus.instance.OnBlockUse += ReceiveBlockUseEvent;
    }

    void OnDestroy() {
        EventBus.instance.OnBlockInit -= ReceiveBlockInitEvent;
        EventBus.instance.OnBlockUse -= ReceiveBlockUseEvent;
    }

    void ReceiveBlockInitEvent(int maxBlocks) {
        blockText.text = $"Blocks Left: {maxBlocks}";
    }

    void ReceiveBlockUseEvent(bool placed, int blocksLeft) {
        blockText.text = $"Blocks Left: {blocksLeft}";
    }
}
