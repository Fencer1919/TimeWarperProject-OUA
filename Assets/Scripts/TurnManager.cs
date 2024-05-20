using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    protected GameManager _gameManager;

    protected bool _isTurnOver = false;
    public bool IsTurnOver { get { return _isTurnOver; } set { _isTurnOver = value; }}

    protected virtual void Awake()
    {
        _gameManager = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    public virtual void FinishTurn()
    {
        _isTurnOver = true;

        if (_gameManager != null)
            _gameManager.UpdateTurn();
    }
}
