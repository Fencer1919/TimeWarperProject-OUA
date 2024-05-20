using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWarp : MonoBehaviour
{
    public GameObject past1;
    public GameObject future1;
    private Board _board;

    private void Awake()
    {
        _board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _board.ChangeTime();
            if (past1 != null && future1!= null)
            {
                if (past1.activeSelf)
                {
                    past1.SetActive(false);
                    future1.SetActive(true);
                   
                }
                else
                {
                    past1.SetActive(true);
                    future1.SetActive(false);
                   
                }
            }
        }
    }
}
