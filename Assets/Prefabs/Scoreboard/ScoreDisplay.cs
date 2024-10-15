using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    void Start()
    {
        textComponent_ = GetComponent<TextMeshPro>();
    }

    public void SetScore(int score)
    {
        score_ = score;

        string scoreString = score_.ToString();

        if (score_ < 0 || hadNegativeScore_)
        {
            scoreString = "<color=grey>" + score_.ToString() + "</color>";
            hadNegativeScore_ = true;
        }

        textComponent_.SetText(scoreString);
    }

    public void AddScore(int change)
    {
        SetScore(GetScore() + change);
    }

    public int GetScore()
    {
        return score_;
    }

    public void ResetScore(int score)
    {
        hadNegativeScore_ = false;
        SetScore(score);
    }

    private TextMeshPro textComponent_;
    private int score_ = 0;
    bool hadNegativeScore_ = false;
}
