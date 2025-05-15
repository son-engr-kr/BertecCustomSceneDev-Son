using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayManager : MonoBehaviour
{
    public float HallwayWidth = 1.0f;
    public float HallwayHeight = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        List<GroundBlockHallway> hallwayList = new List<GroundBlockHallway>(GameObject.FindObjectsOfType<GroundBlockHallway>());
        foreach (GroundBlockHallway hallway in hallwayList)
        {
            hallway.SetHallwaySize(HallwayWidth/2f, HallwayWidth/2f, HallwayHeight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
