using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Stat,
    Walker,
}

public class EnemyMover : Mover
{
    public Vector3 directionToMove = new Vector3(0f, 0f, Board.spacing);
    public MovementType movementType = MovementType.Stat;
    public float statTime = 1f;

    protected override void Awake()
    {
        base.Awake(); 
    }

    protected override void Start()
    {
        base.Start();
    }

    public void MoveOneTurn()
    {
        switch (movementType)
        {
            case MovementType.Walker:
                Walker();
                break;
            case MovementType.Stat:
				Stat();
                break;
        }
    }

    void Walker()
    {
        StartCoroutine(WalkerRoutine());
    }

    IEnumerator WalkerRoutine()
    {
        Vector3 startPos = new Vector3(_currentNode.Coordinate.x, 0f, _currentNode.Coordinate.y);
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);

        Move(newDest, 0f);

        while (isMoving)
			yield return null; 

        if (_board != null)
        {
            Node newDestNode = _board.FindNodeAt(newDest);
            Node nextDestNode = _board.FindNodeAt(nextDest);
            if (nextDestNode == null || !newDestNode.LinkedNodes.Contains(nextDestNode))
            {
                destination = startPos;
                FaceDestination();

                yield return new WaitForSeconds(rotateTime);
            }
        }
		base.finishMovementEvent.Invoke();
    }

    void Stat()
    {
        StartCoroutine(StatRoutine());
    }

    IEnumerator StatRoutine()
    {
       
        yield return new WaitForSeconds(statTime);
        base.finishMovementEvent.Invoke();
    }
}
