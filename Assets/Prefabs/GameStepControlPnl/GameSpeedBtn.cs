using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedBtn : Button
{
    void Start()
    {
        fieldManager_ = field.GetComponent<FieldGenerator>();
    }

    override public void OnPushed()
    {
        fieldManager_.ChangeSpeed(sign * amount);
    }

    public GameObject field;
    private FieldGenerator fieldManager_;

    public int sign = 0;
    public float amount = 0.1f;
}
