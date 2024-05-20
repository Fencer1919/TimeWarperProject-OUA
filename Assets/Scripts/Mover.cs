using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    public Vector3 destination;
    public bool isMoving = false;

    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
    public float moveSpeed = 1.5f;
    public float rotateTime = 0.5f;
    public float iTweenDelay = 0f;

    protected Board _board;
    protected Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } }

    public UnityEvent finishMovementEvent;

    protected virtual void Awake()
    {
        _board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    protected virtual void Start()
    {
        UpdateCurrentNode();
    }

    public void Move(Vector3 destinationPos, float delayTime = 0.25f)
    {
        if (isMoving)
            return;

        if (_board != null)
        {
            Node targetNode = _board.FindNodeAt(destinationPos);

            if (targetNode != null && _currentNode != null && _currentNode.LinkedNodes.Contains(targetNode) && !(targetNode.isPastObstacle && _board.isPast) && !(!_board.isPast && targetNode.isFutureObstacle))
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
        }
    }

    protected virtual IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;

        FaceDestination();
        yield return new WaitForSeconds(delayTime + 0.25f);

        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "speed", moveSpeed
        ));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
            yield return null;

        iTween.Stop(gameObject);
        transform.position = destinationPos;
        isMoving = false;
        UpdateCurrentNode();
    }

    public void MoveLeft()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        Move(newPosition, 0);
    }

    public void MoveRight()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        Move(newPosition, 0);
    }

    public void MoveUp()
    {
        Vector3 newPosition = transform.position + new Vector3(0f, 0f, Board.spacing);
        Move(newPosition, 0);
    }

    public void MoveDown()
    {
        Vector3 newPosition = transform.position + new Vector3(0f, 0f, -Board.spacing);
        Move(newPosition, 0);
    }

    protected void UpdateCurrentNode()
    {
        if (_board != null)
            _currentNode = _board.FindNodeAt(transform.position);
    }

    protected void FaceDestination()
    {
        Vector3 relativePosition = destination - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        float newY = newRotation.eulerAngles.y;

        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", easeType,
            "time", rotateTime
        ));
    }
}
