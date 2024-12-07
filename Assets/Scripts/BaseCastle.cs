using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CastleType
{
    Nest, //巢穴
    Tower, //塔楼
}

public class BaseCastle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image castleImage;
    public Text countText;
    
    public CastleType castleType;
    public bool isOccupiedOnStart = true; //初始不为空塔
    public bool isOccupied => occupiedUnitCount > 0; //是否被任何单位占领
    public UnitType occupiedUnitType; //占领单位类型
    public int occupiedUnitCount; //占领单位数量

    public float unitSpawnInterval = 1f;

    public Vector2 startDragPos;
    public Vector2 dragDir;
    
    private Coroutine currentAttackCoroutine;

    private static Color BeeCastleColor = Color.yellow;
    private static Color AntCastleColor = Color.blue;
    private static Color FreeCastleColor = Color.gray;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        castleImage = transform.Find("Image").GetComponent<Image>();
        countText = transform.Find("CountTxt").GetComponent<Text>();
        if (!isOccupiedOnStart)
        {
            occupiedUnitCount = -10;
        }

        UpdateCountText();
        UpdateCastleColor();
        
        InvokeRepeating("SpawnUnit", 1f, unitSpawnInterval);
    }

    private void UpdateCastleColor()
    {
        if (!isOccupied)
        {
            castleImage.color = FreeCastleColor;
            return;
        }
        castleImage.color = occupiedUnitType == UnitType.Bee ? BeeCastleColor : AntCastleColor;
    }

    private void UpdateCountText()
    {
        countText.text = $"{Mathf.Abs(occupiedUnitCount)}";
    }

    private void SpawnUnit()
    {
        if (!isOccupied)
        {
            return;
        }
        if (castleType != CastleType.Nest)
        {
            return;
        }

        occupiedUnitCount++;
        countText.text = $"{occupiedUnitCount}";
    }
    
    public void OnOccupyByUnit(UnitType unitType)
    {
        if (occupiedUnitCount == 0)
        {
            occupiedUnitType = unitType;
            StopCurrentAttack();
        }
        // Debug.Log($"[{GetType().Name}] occupied by {unitType}, count = {occupiedUnitCount}");

        if (!isOccupied)
        {
            //正在尝试占领
            occupiedUnitCount++;
        }
        else
        {
            //被攻击或者获得增援
            occupiedUnitCount = unitType == occupiedUnitType ? occupiedUnitCount + 1 : occupiedUnitCount - 1;
        }
        UpdateCountText();
        UpdateCastleColor();
    }

    private bool CanDrag()
    {
        return isOccupied && occupiedUnitType == UnitType.Bee;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag())
        {
            return;
        }
        startDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag())
        {
            return;
        }
        dragDir = eventData.position - startDragPos;
        dragDir.Normalize();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag())
        {
            return;
        }
        if (!World.Instance.FindCastle(this, dragDir, out BaseCastle target))
        {
            Debug.Log($"[{GetType().Name}] no find target castle");
            return;
        }
        MoveTo(target);
    }

    public void MoveTo(BaseCastle target)
    {
        if (occupiedUnitCount <= 0)
        {
            Debug.Log($"[{GetType().Name}] unit not enough");
            return;
        }

        StopCurrentAttack();
        currentAttackCoroutine = StartCoroutine(SendUnit(occupiedUnitCount, target));
        
        // Debug.Log($"[{GetType().Name}] attack [{target.name}]");
    }

    private void StopCurrentAttack()
    {
        if (currentAttackCoroutine == null)
        {
            return;
        }
        StopCoroutine(currentAttackCoroutine);
        currentAttackCoroutine = null;
    }

    private IEnumerator SendUnit(int count, BaseCastle target)
    {
        var wait = new WaitForSeconds(0.5f);
        for (int i = 0; i < count; i++)
        {        
            yield return wait;
            World.Instance.CreateUnit(this, occupiedUnitType, target);
            occupiedUnitCount--;
            countText.text = $"{occupiedUnitCount}";
        }
    }
}
