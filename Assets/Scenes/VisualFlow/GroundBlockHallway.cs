using UnityEngine;

/// <summary>
/// Each ground block prefab controls the path shape, path size, and peripheral objects.
/// The peripheral are at a fixed location for each ground block and do not change as the scene moves.
/// </summary>

public class GroundBlockHallway : GroundBlock
{
    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject Ceiling;
    public GameObject Floor;

    void Start()
    {
    }


    public void SetPathTypeOption(string pathOption, Material pathMaterial)
    {
        //Do nothing
    }

    public void SetPathSizeOption(string pathOption)
    {
        //Do nothing
    }
    public void SetHallwaySize(float left, float right, float height)
    {
        LeftWall.transform.localPosition = new Vector3(-left, 5f, 0);
        RightWall.transform.localPosition = new Vector3(right, 5f, 0);
        Ceiling.transform.localPosition = new Vector3(0, height, 0);
        Floor.transform.localPosition = new Vector3(0, 0, 0);
    }
}
