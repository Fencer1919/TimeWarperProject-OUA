using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinLogo : MonoBehaviour
{
    public float rotateSpeed = 20f;

    void Start()
    {  
        iTween.RotateBy(gameObject, iTween.Hash(
            "z", 360f,
            "looptype", iTween.LoopType.loop,
            "speed", rotateSpeed,
            "easetype", iTween.EaseType.linear
        ));
    }

}
