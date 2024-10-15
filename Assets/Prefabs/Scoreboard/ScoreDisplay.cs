using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public void SetScore(int score)
    {
        score_ = score;

        string scoreString = score_.ToString();

        if (score_ < 0 || hadNegativeScore_)
        {
            scoreString = "<color=grey>" + score_.ToString() + "</color>";
            hadNegativeScore_ = true;
        }

        GetComponent<TextMeshPro>().SetText(scoreString);
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

    private int score_ = 0;
    bool hadNegativeScore_ = false;
}
