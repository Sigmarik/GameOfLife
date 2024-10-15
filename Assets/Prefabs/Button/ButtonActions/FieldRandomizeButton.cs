using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldRandomizeButton : Button
{
    void Start()
    {
        fieldManager_ = field.GetComponent<FieldGenerator>();
    }

    override public void OnPushed()
    {
        fieldManager_.Randomize();
        fieldManager_.Pause();
    }

    public GameObject field;
    private FieldGenerator fieldManager_;
}
