using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Vector2 _coordinate;
    public Vector2 Coordinate { get { return Utils.Vector2Round(_coordinate); } }

    List<Node> _neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return _neighborNodes; } }

    List<Node> _linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return _linkedNodes; } }

    Board _board;
    bool _isInitialized = false;

    public GameObject mesh;
    public GameObject linkPrefab;
    public float scaleTime = 0.3f;
    public iTween.EaseType easeType = iTween.EaseType.easeInExpo;
    public float delay = 1f;
    public LayerMask obstacleLayer;
    public bool isLevelGoal = false;
    public bool isPastObstacle = false;
    public bool isFutureObstacle = false;

    void Awake()
    {
        _board = Object.FindObjectOfType<Board>();
        _coordinate = new Vector2(transform.position.x, transform.position.z);
    }

    void Start()
    {
        if (mesh != null)
        {
            mesh.transform.localScale = Vector3.zero;

            if (_board != null)
                _neighborNodes = FindNeighbors(_board.AllNodes);
        }
    }

    public void ShowNode()
    {
        if (mesh != null)
        {
            iTween.ScaleTo(mesh, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay", delay
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes)
    {
        List<Node> tempList = new List<Node>();

        foreach (Vector2 dir in Board.directions)
        {
            Node foundNeighbor = FindNeighborAt(nodes, dir);

            if (foundNeighbor != null && !tempList.Contains(foundNeighbor))
                tempList.Add(foundNeighbor);
        }
        return tempList;
    }

    public Node FindNeighborAt(List<Node> nodes, Vector2 dir)
    {
        return nodes.Find(n => n.Coordinate == Coordinate + dir);
    }

    public Node FindNeighborAt(Vector2 dir)
    {
        return FindNeighborAt(NeighborNodes, dir);
    }


    public void InitNode()
    {
        if (!_isInitialized)
        {
            ShowNode();
            InitNeighbors();
            _isInitialized = true;
        }
    }

    void InitNeighbors()
    {
        StartCoroutine(InitNeighborsRoutine());
    }

    IEnumerator InitNeighborsRoutine()
    {
        yield return new WaitForSeconds(delay);

        foreach (Node n in _neighborNodes)
        {
            if (!_linkedNodes.Contains(n))
            {
                Obstacle obstacle = FindObstacle(n);
                if (obstacle == null)
                {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    void LinkNode(Node targetNode)
    {
        if (linkPrefab != null)
        {
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;

            Link link = linkInstance.GetComponent<Link>();
            if (link != null)
                link.ShowLink(transform.position, targetNode.transform.position);

            if (!_linkedNodes.Contains(targetNode))
                _linkedNodes.Add(targetNode);

            if (!targetNode.LinkedNodes.Contains(this))
                targetNode.LinkedNodes.Add(this);
        }
    }

    Obstacle FindObstacle(Node targetNode)
    {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, Board.spacing + 0.1f, obstacleLayer))
            return raycastHit.collider.GetComponent<Obstacle>();

        return null;
    }
}
