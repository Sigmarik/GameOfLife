using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour
{
    void Start()
    {
        defaultPosition_ = transform.localPosition;
    }

    void OnMouseEnter()
    {
        if (locked_)
        {
            ResetDepth();
            return;
        }

        transform.localPosition -= new Vector3(0.0f, depth / 2.0f, 0.0f);
    }

    void OnMouseExit()
    {
        if (locked_)
        {
            ResetDepth();
            return;
        }

        transform.localPosition += new Vector3(0.0f, depth / 2.0f, 0.0f);
    }

    void OnMouseDown()
    {
        if (pushed_ || locked_)
        {
            return;
        }

        transform.localPosition -= new Vector3(0.0f, depth / 2.0f, 0.0f);
        pushed_ = true;
        OnPushed();
    }

    void OnMouseUp()
    {
        if (locked_)
        {
            return;
        }

        transform.localPosition += new Vector3(0.0f, depth / 2.0f, 0.0f);
        pushed_ = false;
        OnReleased();
    }

    private void ResetDepth()
    {
        transform.localPosition = defaultPosition_;
    }

    public void Lock()
    {
        locked_ = true;

        if (pushed_)
        {
            pushed_ = false;
            OnReleased();
        }

        ResetDepth();
    }

    public void Unlock()
    {
        locked_ = false;

        ResetDepth();
    }

    public bool Locked()
    {
        return locked_;
    }

    public virtual void OnPushed() { }
    public virtual void OnReleased() { }

    public float depth = 0.25f;

    private bool locked_ = false;

    private bool pushed_ = false;
    private Vector3 defaultPosition_ = new Vector3();
}
