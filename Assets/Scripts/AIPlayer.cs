using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Execute", 1f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Execute()
    {
        List<BaseCastle> antCastles = World.Instance.castles.FindAll(castle => castle.occupiedUnitType == UnitType.Ant);
        for (int i = 0; i < antCastles.Count; i++)
        {
            BaseCastle antCastle = antCastles[i];
            if (antCastle.IsUnityNull())
            {
                continue;
            }
            if (antCastle.occupiedUnitCount < 5)
            {
                continue;
            }

            BaseCastle targetCastle = World.Instance.castles
                .FindAll(castle => castle != antCastle && castle.occupiedUnitType == UnitType.Bee || !castle.isOccupied)
                .OrderBy(castle => Vector2.Distance(castle.transform.position, antCastle.transform.position))
                .FirstOrDefault();
            antCastle.Attack(targetCastle);
        }
    }
}
