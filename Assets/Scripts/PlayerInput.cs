using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    float _h;
    public float H { get { return _h; } }

    float _v;
    public float V { get { return _v; } }

    bool _enableInput = false;
    public bool EnableInput { get { return _enableInput; } set { _enableInput = value; } }

    public void GetKeyInput()
    {
        if (_enableInput)
        {
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
        }
        else
        {
            _h = 0f;
            _v = 0f;
        }
    }
}
