using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDirector : MonoBehaviour
{
    public void CaptureBalanceChange(uint positiveNeighbors, uint negativeNeighbors)
    {
        balanceChange_ =
            GetAbsoluteBalanceChange(positiveNeighbors, negativeNeighbors);
    }

    public void ApplyBalanceChange()
    {
        balance_ += balanceChange_;

        if (balanceChange_ != 0)
        {
            GameObject subject = isTower_ ? tower : popper;
            Popper subjectPopper = subject.GetComponent<Popper>();
            subjectPopper.UpdateBalance(balance_);

            if (balance_ == 0 && isTower_)
            {
                KillTower();
            }
        }

        balanceChange_ = 0;
    }

    private bool ShouldWin(uint alpha, uint beta)
    {
        return SpawnableFrom(alpha) && (!SpawnableFrom(beta) || alpha > beta);
    }

    public int GetBalance() { return balance_; }

    private int GetAbsoluteBalanceChange(uint positiveNeighbors, uint negativeNeighbors)
    {
        if (balance_ == 0)
        {
            if (ShouldWin(positiveNeighbors, negativeNeighbors))
            {
                return 1;
            }

            if (ShouldWin(negativeNeighbors, positiveNeighbors))
            {
                return -1;
            }

            return 0;
        }
        else if (balance_ > 0)
        {
            return GetRelativeBalanceChange(positiveNeighbors, negativeNeighbors);
        }
        else if (balance_ < 0)
        {
            return -GetRelativeBalanceChange(negativeNeighbors, positiveNeighbors);
        }

        return 0;
    }

    private bool SpawnableFrom(uint count)
    {
        return count >= spawnThreshold && LivableAt(count);
    }

    private bool LivableAt(uint count)
    {
        return count >= underpopulationThreshold && count < overpopulationThreshold;
    }

    private int GetRelativeBalanceChange(uint teammates, uint enemies)
    {
        if (ShouldWin(enemies, teammates))
        {
            return -1;
        }

        if (!isTower_ && !LivableAt(teammates))
        {
            return -1;
        }

        if (!isTower_ && ShouldWin(teammates, enemies))
        {
            return 1;
        }

        return 0;
    }

    public void ChangeBalance(int change)
    {
        balanceChange_ = change;
        ApplyBalanceChange();
    }

    public void ConvertToTower(int team)
    {
        Popper towerPopper = tower.GetComponent<Popper>();
        Popper commonPopper = popper.GetComponent<Popper>();

        balance_ = startingTowerHealth * team;

        towerPopper.PopIn(balance_);
        commonPopper.PopOut();

        isTower_ = true;
    }

    public void KillTower()
    {
        Popper towerPopper = tower.GetComponent<Popper>();
        Popper commonPopper = popper.GetComponent<Popper>();

        towerPopper.PopOut();
        if (balance_ != 0)
        {
            commonPopper.PopIn(balance_);
        }

        isTower_ = false;
    }

    public void Highlight()
    {
        Popper towerPopper = tower.GetComponent<Popper>();
        Popper commonPopper = popper.GetComponent<Popper>();

        towerPopper.Highlight();
        commonPopper.Highlight();

        Material material = GetComponent<Renderer>().material;
        material.SetInteger("_Highlight", 1);
    }

    public void Dehighlight()
    {
        Popper towerPopper = tower.GetComponent<Popper>();
        Popper commonPopper = popper.GetComponent<Popper>();

        towerPopper.Dehighlight();
        commonPopper.Dehighlight();

        Material material = GetComponent<Renderer>().material;
        material.SetInteger("_Highlight", 0);
    }

    public void SetPrimaryTeam(int team)
    {
        Material material = GetComponent<Renderer>().material;
        material.SetInteger("_Balance", team);
        team_ = team;
    }

    public int GetPrimaryTeam()
    {
        return team_;
    }

    public bool IsTower()
    {
        return isTower_;
    }

    public void Reset()
    {
        balanceChange_ = -balance_;
        ApplyBalanceChange();
    }

    public void GetTeamEvents(out int teamLost, out int teamGained)
    {
        int newBalance = balance_ + balanceChange_;

        teamLost = 0;
        teamGained = 0;

        if (newBalance * balance_ > 0) return;

        teamLost = Math.Sign(balance_);
        teamGained = Math.Sign(newBalance);
    }

    public GameObject tower;
    public GameObject popper;

    public int startingTowerHealth = 40;
    public int underpopulationThreshold = 2;
    public uint spawnThreshold = 3;
    public uint overpopulationThreshold = 4;

    private int balance_ = 0;
    private int balanceChange_ = 0;
    private bool isTower_ = false;

    private int team_;
}
