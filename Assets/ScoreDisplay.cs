using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    public Action<int> ScoreUpdated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.AssignScoreDisplay(this);
        ScoreUpdated += OnScoreUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnScoreUpdate(int score)
    {
        _scoreText.text = $"Score: {score}";
    }
}
