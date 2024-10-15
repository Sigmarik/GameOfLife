using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseUnpauseBtn : Button
{
    void Start()
    {
        fieldManager_ = field.GetComponent<FieldGenerator>();
        UpdateState();
    }

    override public void OnPushed()
    {
        if (fieldManager_.Paused())
        {
            fieldManager_.Unpause();
        }
        else
        {
            fieldManager_.Pause();
        }

        UpdateState();
    }

    public void UpdateState()
    {
        string text = "||";
        if (fieldManager_.Paused())
        {
            text = ">";
        }
        textDisplay.GetComponent<TextMeshPro>().SetText(text);
    }

    public GameObject field;
    private FieldGenerator fieldManager_;

    public GameObject textDisplay;
}
