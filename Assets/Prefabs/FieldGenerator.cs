using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldGenerator : MonoBehaviour
{
    void Update()
    {
        if (!paused_ && Time.time > fieldUpdateTime_ + fieldUpdateDelta)
        {
            SimStep();
            fieldUpdateTime_ = Time.time;
        }
    }

    TileDirector spawnCell(int idX, int idY)
    {
        float posX = idX * spacing + spacing / 2.0f;
        float posY = idY * spacing - (sizeY - 1) * spacing / 2.0f;

        GameObject tile = Instantiate(tileObject, transform);

        tile.transform.localPosition = new Vector3(posY, 0.0f, posX);
        if (idX >= 0)
        {
            tile.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        }

        TileDirector director = tile.GetComponent<TileDirector>();
        director.SetPrimaryTeam(idX >= 0 ? 1 : -1);

        return director;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int idX = -(int)sizeX; idX < sizeX; ++idX)
        {
            tiles_.Add(new List<TileDirector>());

            List<TileDirector> line = tiles_[tiles_.Count - 1];

            for (int idY = 0; idY < sizeY; ++idY)
            {
                TileDirector director = spawnCell(idX, idY);
                line.Add(director);
            }
        }

        UpdateSpeedDisplay();
        UpdateStepDisplay();

        Reset();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX * 2 * spacing, 0.0f, sizeY * spacing));
    }

    private void UpdateScores(TileDirector tile, out int positiveChange, out int negativeChange)
    {
        tile.GetTeamEvents(out int teamLost, out int teamGained);

        positiveChange = 0;
        negativeChange = 0;

        if (teamGained > 0)
        {
            positiveChange += birthReward;
        }
        if (teamGained < 0)
        {
            negativeChange += birthReward;
        }

        if (teamLost > 0)
        {
            positiveChange -= deathPenalty;
        }
        if (teamLost < 0)
        {
            negativeChange -= deathPenalty;
        }
    }

    public void SimStep()
    {
        for (int idX = 0; idX < tiles_.Count; ++idX)
        {
            for (int idY = 0; idY < tiles_[idX].Count; ++idY)
            {
                PrepareCellUpdate(idX, idY);
            }
        }

        int scoreChangePositive = 0;
        int scoreChangeNegative = 0;

        for (int idX = 0; idX < tiles_.Count; ++idX)
        {
            for (int idY = 0; idY < tiles_[idX].Count; ++idY)
            {
                TileDirector tile = tiles_[idX][idY];

                UpdateScores(tile, out int positiveChange, out int negativeChange);
                scoreChangePositive += positiveChange;
                scoreChangeNegative += negativeChange;

                tile.ApplyBalanceChange();
            }
        }

        positiveScoreDisplay.GetComponent<ScoreDisplay>().AddScore(scoreChangePositive);
        negativeScoreDisplay.GetComponent<ScoreDisplay>().AddScore(scoreChangeNegative);

        stepCount_++;

        UpdateStepDisplay();
    }

    private void PrepareCellUpdate(int idX, int idY)
    {
        uint positiveCount = 0;
        uint negativeCount = 0;

        for (int deltaX = -1; deltaX <= 1; ++deltaX)
        {
            for (int deltaY = -1; deltaY <= 1; ++deltaY)
            {
                if (deltaX == 0 && deltaY == 0) continue;

                int finalX = idX + deltaX;
                int finalY = idY + deltaY;

                if (finalX < 0 || finalX >= tiles_.Count) continue;
                if (finalY < 0 || finalY >= tiles_[finalX].Count) continue;

                TileDirector neighbor = tiles_[finalX][finalY];

                if (neighbor.GetBalance() > 0) positiveCount++;
                if (neighbor.GetBalance() < 0) negativeCount++;
            }
        }

        TileDirector cell = tiles_[idX][idY];
        cell.CaptureBalanceChange(positiveCount, negativeCount);
    }

    public void Reset()
    {
        for (int idX = 0; idX < tiles_.Count; ++idX)
        {
            for (int idY = 0; idY < tiles_[idX].Count; ++idY)
            {
                tiles_[idX][idY].Reset();
            }
        }

        stepCount_ = 0;
        UpdateStepDisplay();

        positiveScoreDisplay.GetComponent<ScoreDisplay>().ResetScore(startingScore);
        negativeScoreDisplay.GetComponent<ScoreDisplay>().ResetScore(startingScore);

        Pause();
    }

    public void Randomize()
    {
        Reset();

        for (int idX = 0; idX < tiles_.Count; ++idX)
        {
            for (int idY = 0; idY < tiles_[idX].Count; ++idY)
            {
                if (UnityEngine.Random.value < 0.5) continue;

                int choiceCenter = idX - (int)sizeX + 1;
                if (choiceCenter <= 0) choiceCenter--;
                tiles_[idX][idY].ChangeBalance(choiceCenter + (int)UnityEngine.Random.Range(-5, 6));
                tiles_[idX][idY].ApplyBalanceChange();
            }
        }
    }

    public void Pause()
    {
        paused_ = true;
        UpdatePauseBtn();
    }

    public void Unpause()
    {
        paused_ = false;
        UpdatePauseBtn();
    }

    public bool Paused()
    {
        return paused_;
    }

    public void ChangeSpeed(float delta)
    {
        fieldUpdateDelta = Math.Clamp(fieldUpdateDelta + delta,
                                      fieldUpdateDeltaLow,
                                      fieldUpdateDeltaHigh);
        UpdateSpeedDisplay();
    }

    private void UpdateSpeedDisplay()
    {
        speedDisplay.GetComponent<TextMeshPro>().SetText(Math.Round(fieldUpdateDelta * 100.0f).ToString());
    }

    private void UpdateStepDisplay()
    {
        stepCountDisplay.GetComponent<TextMeshPro>().SetText(stepCount_.ToString());
    }

    private void UpdatePauseBtn()
    {
        pauseButton.GetComponent<PauseUnpauseBtn>().UpdateState();
    }

    public uint sizeX = 5;
    public uint sizeY = 7;
    public GameObject tileObject;
    public float spacing = 1.0f;

    private List<List<TileDirector>> tiles_ = new List<List<TileDirector>>();

    private float fieldUpdateTime_ = -100.0f;

    public float fieldUpdateDelta = 0.5f;
    public float fieldUpdateDeltaLow = 0.05f;
    public float fieldUpdateDeltaHigh = 3.0f;

    public GameObject speedDisplay;
    public GameObject stepCountDisplay;
    public GameObject pauseButton;

    private uint stepCount_ = 0;

    private bool paused_ = true;

    public GameObject positiveScoreDisplay;
    public GameObject negativeScoreDisplay;
    public int startingScore = 100;

    public int birthReward = 10;
    public int deathPenalty = 10;
}
