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

    // 
    public override void SetPathSizeOption(string pathOption)
    {
        float narrowWidth = 1.15f;
        float narrowHeight = 3.2f;

        float normalWidth = 4.3f;
        float normalHeight = 3.2f;

        float wideWidth = 20f;
        float wideHeight = 3.2f;


        if (pathOption == CustomSceneController.PATHSIZEOPTION_NARROW)
        {
            // left, right, height
            SetHallwaySize(narrowWidth / 2f, narrowWidth / 2f, narrowHeight);
        }
        else if (pathOption == CustomSceneController.PATHSIZEOPTION_NORMAL)
        {
            SetHallwaySize(normalWidth / 2f, normalWidth / 2f, normalHeight);
        }
        else if (pathOption == CustomSceneController.PATHSIZEOPTION_WIDE)
        {
            SetHallwaySize(wideWidth / 2f, wideWidth / 2f, wideHeight);
        }
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
