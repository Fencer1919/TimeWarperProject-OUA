using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float rotateSpeed = 20f;

    void Start()
    {
       transform.position = new Vector3(transform.position.x, transform.position.y - 0.60f, transform.position.z);
       
        iTween.RotateBy(gameObject, iTween.Hash(
            "y", 360f,
            "looptype", iTween.LoopType.loop,
            "speed", rotateSpeed,
            "easetype", iTween.EaseType.linear
        ));
    }

}
