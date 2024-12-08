using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum UnitType
{
    Bee,
    Ant,
}

public class BaseUnit : MonoBehaviour
{
    public UnitType unitType;
    
    public float moveDuration = 15f;
    public float moveSpeed = 1.0f;
    public Vector2 moveDir;
    public float arrivalThreshold = 0.1f;
    
    public BaseCastle target;

    void Start()
    {
        arrivalThreshold = 1f;
    }

    void Update()
    {
        Execute();
    }

    private void Execute()
    {
        if (target.IsUnityNull())
        {
            return;
        }
        
        float distancePerFrame = Screen.width / moveDuration * Time.deltaTime * moveSpeed;
        transform.Translate(moveDir * distancePerFrame);
        
        if (Vector2.Distance(transform.position, target.transform.position) < arrivalThreshold)
        {
            OnReachTarget();
        }
    }

    public void SetTarget(BaseCastle target)
    {
        this.target = target;
        moveDir = target.transform.position - transform.position;
        moveDir = moveDir.normalized;
    }

    private void OnReachTarget()
    {
        if (target.IsUnityNull())
        {
            return;
        }
        
        target.OnOccupyByUnit(unitType);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseUnit otherUnit = other.GetComponent<BaseUnit>();
        if (otherUnit.IsUnityNull())
        {
            return;
        }
        if (otherUnit.unitType == unitType)
        {
            return;
        }
        
        Destroy(otherUnit.gameObject);
    }
}
