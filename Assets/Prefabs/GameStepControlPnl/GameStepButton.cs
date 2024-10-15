using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStepButton : Button
{
    void Start()
    {
        fieldManager_ = field.GetComponent<FieldGenerator>();
    }

    override public void OnPushed()
    {
        fieldManager_.SimStep();
    }

    public GameObject field;
    private FieldGenerator fieldManager_;
}
