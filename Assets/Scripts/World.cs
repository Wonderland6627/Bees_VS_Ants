using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Road
{
    public RectTransform[] points; //限制points的长度为2
}

public class World : SingleMono<World>
{
    public List<BaseCastle> castles = new List<BaseCastle>();
    public List<Road> roads = new List<Road>();

    private RectTransform roadContainer;

    public void Init()
    {
        castles.AddRange(FindObjectsOfType<BaseCastle>());
        roadContainer = GameObject.Find("Canvas/BG/RoadContainer").GetComponent<RectTransform>();
        CreateRoads();
    }
    
    public BaseUnit CreateUnit(BaseCastle spawnCastle, UnitType unitType, BaseCastle targetCastle)
    {
        var res = Resources.Load<BaseUnit>(unitType.ToString());
        var unit = Instantiate(res, FindObjectOfType<Canvas>().transform);
        unit.transform.position = spawnCastle.transform.position;
        unit.SetTarget(targetCastle);
        return unit;
    }

    public bool FindCastle(BaseCastle origin, Vector2 dir, out BaseCastle target)
    {
        target = null;
        List<BaseCastle> inAngleCastles = new List<BaseCastle>();
        foreach (var castle in castles)
        {
            if (castle == origin)
            {
                continue;
            }
            
            Vector3 dirToTarget = castle.transform.position - origin.transform.position;
            float angle = Vector3.Angle(dir, dirToTarget.normalized);
            if (angle > 20f)
            {
                continue;
            }
            
            inAngleCastles.Add(castle);
        }

        if (inAngleCastles.Count == 0)
        {
            return false;
        }
        
        target = inAngleCastles.OrderBy(castle => Vector3.Distance(castle.transform.position, origin.transform.position)).First();
        return true;
    }

    private void CreateRoads()
    {
        Image roadImgRes = Resources.Load<Image>("Road");
        for (int i = 0; i < roads.Count; i++)
        {
            Road road = roads[i];
            if (road.points.Length != 2)
            {
                continue;
            }
            Image roadImg = Instantiate(roadImgRes, roadContainer);
            RectTransform roadImgRect = roadImg.GetComponent<RectTransform>();
            Vector2 start = road.points[0].anchoredPosition;
            Vector2 end = road.points[1].anchoredPosition;
            
            float distance = Vector2.Distance(start, end);
            roadImgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, distance);
            roadImgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            
            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
            roadImgRect.rotation = Quaternion.Euler(0, 0, angle);
            
            Vector2 center = (start + end) / 2;
            roadImgRect.anchoredPosition = center;
        }
    }
}
