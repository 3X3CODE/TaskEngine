using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;

namespace TaskEngine.MinigameBlock;

public class Sequencer
{
    public List<IMinigameBlock> allBlocks = new();
    public MonoBehaviour behaviour;

    public Sequencer(MonoBehaviour script) => behaviour = script;

    public void AddBlock(IMinigameBlock block)
    {
        allBlocks.Add(block);
    }

    public IEnumerator RunBlocks()
    {
        foreach (IMinigameBlock block in allBlocks)
        {
            yield return behaviour.StartCoroutine(block.Execute());
        }
    }
}