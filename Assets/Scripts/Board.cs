using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static float spacing = 2f;

    public static readonly Vector2[] directions =
    {
        new Vector2(spacing, 0f),
        new Vector2(-spacing, 0f),
        new Vector2(0f, spacing),
        new Vector2(0f, -spacing)
    };

    List<Node> _allNodes = new List<Node>();
    public List<Node> AllNodes { get { return _allNodes; } }

    Node _playerNode;
    public Node PlayerNode { get { return _playerNode; } }

    Node _goalNode;
    public Node GoalNode { get { return _goalNode; } }
    public GameObject goalPrefab;
    public float drawGoalTime = 2f;
    public float drawGoalDelay = 2f;
    public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;

    PlayerMover _player;

    public List<Transform> capturePositions;
    int _currentCapturePosition = 0;
	public int CurrentCapturePosition { get { return _currentCapturePosition; } set { _currentCapturePosition = value; } }
    public float capturePositionIconSize = 0.4f;

    public bool isPast = false;
    public void ChangeTime()
    {
        if (isPast) { isPast = false; }
        else { isPast = true; }
    }
    void Awake()
    {
        _player = Object.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
        GetNodeList();

        _goalNode = FindGoalNode();
    }

    public void GetNodeList()
    {
        Node[] nList = Object.FindObjectsOfType<Node>();
        _allNodes = new List<Node>(nList);
    }

    public Node FindNodeAt(Vector3 pos)
    {
        Vector2 boardCoord = Utils.Vector2Round(new Vector2(pos.x, pos.z));
        return _allNodes.Find(n => n.Coordinate == boardCoord);
    }

    Node FindGoalNode()
    {
        return _allNodes.Find(n => n.isLevelGoal);
    }
    
    public Node FindPlayerNode()
    {
        if (_player != null && !_player.isMoving)
        {
            return FindNodeAt(_player.transform.position);
        }
        return null;
    }

    public List<EnemyManager> FindEnemiesAt(Node node)
    {
        List<EnemyManager> foundEnemies = new List<EnemyManager>();
        EnemyManager[] enemies = Object.FindObjectsOfType<EnemyManager>() as EnemyManager[];

        foreach (EnemyManager enemy in enemies)
        {
            EnemyMover mover = enemy.GetComponent<EnemyMover>();

            if (mover.CurrentNode == node)
            {
                foundEnemies.Add(enemy);
            }
        }
        return foundEnemies;
    }

    public void UpdatePlayerNode()
    {
        _playerNode = FindPlayerNode();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        if (_playerNode != null)
            Gizmos.DrawSphere(_playerNode.transform.position, 0.2f);
       
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);

        foreach (Transform capturePos in capturePositions)
            Gizmos.DrawCube(capturePos.position, Vector3.one * capturePositionIconSize);
    }

    public void DrawGoal()
    {
        if (goalPrefab != null && _goalNode != null)
        {
            GameObject goalInstance = Instantiate(goalPrefab, _goalNode.transform.position, Quaternion.identity);
            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType
            ));
        }
    }

    public void InitBoard()
    {
        if (_playerNode != null)
        {
            _playerNode.InitNode();
        }
    }


}
