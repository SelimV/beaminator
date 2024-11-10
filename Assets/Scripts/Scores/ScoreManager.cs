using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;

    [SerializeField] private int connectionPoints = 0;
    [SerializeField] private UnityEvent<string> updateScore = new UnityEvent<string>();

    public void AddConnectionPoint()
    { connectionPoints++; updateScore.Invoke($"Connection points: {connectionPoints}"); }

    public void ResetConnectionPointScore()
    { connectionPoints = 0; updateScore.Invoke($"Connection points: {connectionPoints}"); }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }



}