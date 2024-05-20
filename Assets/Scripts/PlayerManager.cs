using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerDeath))]
public class PlayerManager : TurnManager
{
	public PlayerMover playerMover;
    public PlayerInput playerInput;

    Board _board;

    public UnityEvent deathEvent;

    protected override void Awake()
    {
        base.Awake();

		playerMover = GetComponent<PlayerMover>();
        playerInput = GetComponent<PlayerInput>();

        _board = Object.FindObjectOfType<Board>().GetComponent<Board>();

		playerInput.EnableInput = true;
    }

    void Update()
    {
        if (playerMover.isMoving || _gameManager.CurrentTurn != Turn.Player)
            return;

		playerInput.GetKeyInput();

		if (playerInput.V == 0)
        {
            if (playerInput.H < 0)
                playerMover.MoveLeft();
            else if (playerInput.H > 0)
                playerMover.MoveRight();
        }
        else if (playerInput.H == 0)
        {
            if (playerInput.V < 0)
                playerMover.MoveDown();
            else if (playerInput.V > 0)
                playerMover.MoveUp();
        }
    }

    public void Die()
    {
        if (deathEvent != null)
            deathEvent.Invoke();
    }

    void CaptureEnemies()
    {
        if (_board != null)
        {
            List<EnemyManager> enemies = _board.FindEnemiesAt(_board.PlayerNode);

            if (enemies.Count != 0)
            {
                foreach (EnemyManager enemy in enemies)
                {
                    if (enemy != null)
                        enemy.Die();
                }
            }
        }
    }

    public override void FinishTurn()
    {
        CaptureEnemies();
        base.FinishTurn();
    }
}
