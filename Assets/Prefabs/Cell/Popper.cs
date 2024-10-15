using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class Popper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        hiddenPosition_ = transform.localPosition;
        popTargetPosition_ = transform.localPosition;

        randomDelayAmount_ = UnityEngine.Random.value * randomDelay;
    }

    static float JerkyInterpolation(float time)
    {
        if (time < 0.0f) return 0.0f;
        if (time > 1.0f) return 1.0f;

        float wave = (float)Math.Pow(1.1f / (1.0f + (time - 0.9f) * (time - 0.9f)), 10);

        return wave * (1 - time) + time;
    }

    void UpdatePopping()
    {
        float animTime = (Time.time - popTime_ - randomDelayAmount_) / popDuration;

        if (animTime < 0.0f) return;

        if (animTime > 1.0f)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = popTargetPosition_;

            enabled = false;
        }

        transform.localScale = Vector3.Lerp(popScale, Vector3.one, animTime);
        transform.localPosition =
            Vector3.LerpUnclamped(
                popStartPosition_,
                popTargetPosition_,
                JerkyInterpolation(animTime));
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePopping();
    }

    private void PopTo(Vector3 target)
    {
        popTime_ = Time.time;
        popTargetPosition_ = target;
        popStartPosition_ = transform.localPosition;

        enabled = true;
    }

    private float GetHeight(int balance)
    {
        float percentage = 1.0f / (1.0f + balance * balance);
        return percentage * popLow + (1.0f - percentage) * popHigh;
    }

    public void PopIn(int balance)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material material = renderer.material;
        material.SetInteger("_Balance", balance);
        textDisplay.GetComponent<TextMeshPro>().SetText(Math.Abs(balance).ToString());
        PopTo(new Vector3(0.0f, GetHeight(balance), 0.0f));
    }

    public void PopOut()
    {
        PopTo(hiddenPosition_);
    }

    public void UpdateBalance(int balance)
    {
        if (balance == 0) PopOut();
        else PopIn(balance);
    }

    public void Highlight()
    {
        Material material = GetComponent<Renderer>().material;
        material.SetInteger("_Highlight", 1);
    }

    public void Dehighlight()
    {
        Material material = GetComponent<Renderer>().material;
        material.SetInteger("_Highlight", 0);
    }

    private float popTime_ = -1024.0f;
    private Vector3 popTargetPosition_ = Vector3.zero;
    private Vector3 popStartPosition_ = Vector3.zero;
    private Vector3 hiddenPosition_ = Vector3.zero;

    public float popDuration = 0.1f;

    public Vector3 popScale = new Vector3(0.9f, 1.1f, 0.9f);

    public GameObject textDisplay;

    private float randomDelayAmount_ = 0.0f;

    public float randomDelay = 0.1f;

    public float popLow = -0.5f;
    public float popHigh = 0.3f;
}
