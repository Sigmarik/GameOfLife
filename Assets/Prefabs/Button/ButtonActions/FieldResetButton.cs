using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldResetButton : Button
{
    void Start()
    {
        fieldManager_ = field.GetComponent<FieldGenerator>();
    }

    override public void OnPushed()
    {
        fieldManager_.Reset();
    }

    public GameObject field;
    private FieldGenerator fieldManager_;
}
