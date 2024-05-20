using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public enum Turn
{
    Player,
    Enemy
}
public class GameManager : MonoBehaviour
{
    Board _board;
    PlayerManager _player;
    List<EnemyManager> _enemies;

    Turn _currentTurn = Turn.Player;
    public Turn CurrentTurn { get { return _currentTurn; } }
   
    bool _hasLevelStarted = false;
    public bool HasLevelStarted { get { return _hasLevelStarted; } set { _hasLevelStarted = value; } }

    bool _isGamePlaying = false;
    public bool IsGamePlaying { get { return _isGamePlaying; } set { _isGamePlaying = value; } }

    bool _isGameOver = false;
    public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }

    bool _hasLevelFinished = false;
    public bool HasLevelFinished { get { return _hasLevelFinished; } set { _hasLevelFinished = value; } }

    public float delay = 1f;

    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;

    private bool itemCollected = false;
    void Awake()
    {
        _board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        _player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        _enemies = (Object.FindObjectsOfType<EnemyManager>() as EnemyManager[]).ToList();
    }

    void Start()
    {
        if (_player != null && _board != null)
            StartCoroutine("RunGameLoop");
    }

    IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {
        Debug.Log("Setup Level");
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }

        Debug.Log("Start Level");
        _player.playerInput.EnableInput = false;

        while (!_hasLevelStarted)
            yield return null;
        
        if (startLevelEvent != null)
            startLevelEvent.Invoke();
    }
   
    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("Play Level");
        _isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        _player.playerInput.EnableInput = true;

        if (playLevelEvent != null)
            playLevelEvent.Invoke();

        while (!_isGameOver)
        {
            yield return null;

            _isGameOver = IsWinner();
        }
    }

    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    IEnumerator LoseLevelRoutine()
    {
        _isGameOver = true;

        yield return new WaitForSeconds(1.5f);

        if (loseLevelEvent != null)
            loseLevelEvent.Invoke();

        yield return new WaitForSeconds(2f);

        Debug.Log("You have lost");
        RestartLevel();
    }

    IEnumerator EndLevelRoutine()
    {
        Debug.Log("End Level");
        _player.playerInput.EnableInput = false;
        
        if (endLevelEvent != null)
            endLevelEvent.Invoke();

        while (!_hasLevelFinished)
        {
            yield return null;
        }

        RestartLevel();
    }
    
    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel()
    {
        _hasLevelStarted = true;
    }

    bool IsWinner()
    {
         if (!itemCollected)
        {
            return false;
        }
        if (_board.PlayerNode != null)
        {
            return (_board.PlayerNode == _board.GoalNode);
        }
        
        return false;
    }

    void PlayPlayerTurn()
    {
        _currentTurn = Turn.Player;
        _player.IsTurnOver = false;

    }

    void PlayEnemyTurn()
    {
        _currentTurn = Turn.Enemy;

        foreach (EnemyManager enemy in _enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemy.IsTurnOver = false;

                enemy.PlayTurn();
            }
        }
    }

    bool IsEnemyTurnComplete()
    {
        foreach (EnemyManager enemy in _enemies)
        {
            if (enemy.IsDead)
                continue;

            if (!enemy.IsTurnOver)
                return false;
        }
        return true;
    }

    bool AreEnemiesAllDead()
    {
        foreach (EnemyManager enemy in _enemies)
        {
            if (!enemy.IsDead)
                return false;
        }
        return true;
    }

    
    public void UpdateTurn()
    {
        if (_currentTurn == Turn.Player && _player != null)
        {
            if (_player.IsTurnOver && !AreEnemiesAllDead())
            {
                PlayEnemyTurn();
            }
        }
        else if (_currentTurn == Turn.Enemy)
        {
            if (IsEnemyTurnComplete())
            {
				PlayPlayerTurn(); 
            }
        }
    }

    public void CollectItem()
    {
        itemCollected = true;
        Debug.Log("Item collected!");
    }

    public bool IsItemCollected()
    {
        return itemCollected;
    }
    
    public void FinishLevel()
    {
        if (itemCollected)
        {
            Debug.Log("Level finished!");
            SceneManager.LoadScene("NextLevel");  
        }
        else
        {
            Debug.Log("You need to collect the item before finishing the level.");
        }
    }
}
