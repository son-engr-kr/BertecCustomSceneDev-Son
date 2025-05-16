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
        float defaultHeight = 0.3f;
        float defaultWidth = 0.36f;
        LeftWall.transform.localPosition = new Vector3(-left, height/2f, 0);
        LeftWall.transform.localScale = new Vector3(1f, height / 10f / defaultHeight, 1f);

        RightWall.transform.localPosition = new Vector3(right, height/2f, 0);
        RightWall.transform.localScale = new Vector3(1f, height / 10f / defaultHeight, 1f);

        Ceiling.transform.localPosition = new Vector3(0, height, 0);
        Ceiling.transform.localScale = new Vector3((right + left) / 10f / defaultWidth, 1f, 1f);

        Floor.transform.localPosition = new Vector3(0, 0, 0);
        Floor.transform.localScale = new Vector3((right + left) / 10f / defaultWidth, 1f, 1f);
    }
}
