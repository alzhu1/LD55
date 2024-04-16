using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BlockPool : MonoBehaviour {
    [SerializeField] private Block blockPrefab;
    [SerializeField] private int maxBlocks = 1;

    private ObjectPool<Block> pool;
    private Dictionary<Vector3, Block> activeBlocks;

    void Awake() {
        pool = new ObjectPool<Block>(() => {
            return Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, transform);
        }, block => {
            block.gameObject.SetActive(true);
        }, block => {
            block.gameObject.SetActive(false);
        }, block => {
            Destroy(block.gameObject);
        }, true, maxBlocks + 1, maxBlocks + 1);

        activeBlocks = new Dictionary<Vector3, Block>();
    }

    void Start() {
        // Updated script execution order so that BlockPool goes after defaults
        EventBus.instance.TriggerOnBlockInit(maxBlocks);
    }

    public bool IsBlockAtPos(Vector3 pos) {
        return activeBlocks.ContainsKey(pos);
    }

    public void EnableBlockAtPos(Vector3 pos) {
        if (activeBlocks.Count < maxBlocks) {
            Block block = pool.Get();
            block.transform.position = pos;
            activeBlocks.Add(pos, block);

            EventBus.instance.TriggerOnBlockUse(true, maxBlocks - activeBlocks.Count);
        }
    }

    public void DisableBlockAtPos(Vector3 pos) {
        if (activeBlocks.ContainsKey(pos)) {
            Block block = activeBlocks[pos];
            pool.Release(block);
            activeBlocks.Remove(pos);

            EventBus.instance.TriggerOnBlockUse(false, maxBlocks - activeBlocks.Count);
        }
    }
}
