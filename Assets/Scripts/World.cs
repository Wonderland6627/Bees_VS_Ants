using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;

public class World : SingleMono<World>
{
    public List<BaseUnit> units = new List<BaseUnit>();
    
    public List<BaseCastle> castles = new List<BaseCastle>();

    public void Init()
    {
        castles.AddRange(FindObjectsOfType<BaseCastle>());
    }
    
    public BaseUnit CreateUnit(BaseCastle spawnCastle, UnitType unitType, BaseCastle targetCastle)
    {
        var res = Resources.Load<BaseUnit>(unitType.ToString());
        var unit = Instantiate(res, FindObjectOfType<Canvas>().transform);
        unit.transform.position = spawnCastle.transform.position;
        unit.SetTarget(targetCastle);
        units.Add(unit);
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
}
