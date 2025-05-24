using UnityEngine;
using System;
using System.Collections;

public class GamePhaseTimer : MonoBehaviour
{
    [Serializable]
    public class GamePhase
    {
        public string phaseName;
        public float duration;
    }

    [SerializeField] private GamePhase[] gamePhases;
    [SerializeField] private bool autoStart = true;
    
    private int currentPhaseIndex = -1;
    private float currentPhaseTime;
    private bool isRunning = false;

    public event Action<string> OnPhaseChanged;
    public event Action OnAllPhasesCompleted;

    private void Start()
    {
        if (autoStart)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            currentPhaseIndex = -1;
            StartNextPhase();
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    public void ResetTimer()
    {
        StopTimer();
        currentPhaseIndex = -1;
        currentPhaseTime = 0f;
    }

    private void StartNextPhase()
    {
        if (!isRunning) return;

        currentPhaseIndex++;
        if (currentPhaseIndex < gamePhases.Length)
        {
            currentPhaseTime = 0f;
            OnPhaseChanged?.Invoke(gamePhases[currentPhaseIndex].phaseName);
            StartCoroutine(PhaseTimer());
        }
        else
        {
            isRunning = false;
            OnAllPhasesCompleted?.Invoke();
        }
    }

    private IEnumerator PhaseTimer()
    {
        while (currentPhaseTime < gamePhases[currentPhaseIndex].duration)
        {
            currentPhaseTime += Time.deltaTime;
            yield return null;
        }
        StartNextPhase();
    }

    public string GetCurrentPhaseName()
    {
        if (currentPhaseIndex >= 0 && currentPhaseIndex < gamePhases.Length)
        {
            return gamePhases[currentPhaseIndex].phaseName;
        }
        return "No Phase";
    }

    public float GetCurrentPhaseProgress()
    {
        if (currentPhaseIndex >= 0 && currentPhaseIndex < gamePhases.Length)
        {
            return currentPhaseTime / gamePhases[currentPhaseIndex].duration;
        }
        return 0f;
    }

    public float GetRemainingTime()
    {
        if (currentPhaseIndex >= 0 && currentPhaseIndex < gamePhases.Length)
        {
            return gamePhases[currentPhaseIndex].duration - currentPhaseTime;
        }
        return 0f;
    }
} 