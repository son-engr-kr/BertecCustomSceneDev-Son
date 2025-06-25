using UnityEngine;

/// <summary>
/// Each ground block prefab controls the path shape, path size, and peripheral objects.
/// The peripheral are at a fixed location for each ground block and do not change as the scene moves.
/// </summary>

public class GroundBlock : MonoBehaviour
{
    [Header("Peripheral Related")]
    public GameObject[] Pyramids;
    public GameObject[] Rectangles;

    [Header("Path Related")]
    public GameObject[] Path_Narrow;
    public GameObject[] Path_Normal;
    public GameObject[] Path_Wide;

    public void SetPeripheralOption(string peripheralOption)
    {
        foreach (GameObject pyramid in Pyramids)
        {
            pyramid?.SetActive(peripheralOption == VisualFlow.PERIPHERALOPTION_PYRAMIDS);
        }

        foreach (GameObject rectangle in Rectangles)
        {
            rectangle?.SetActive(peripheralOption == VisualFlow.PERIPHERALOPTION_RECTANGLES);
        }
    }

    public void SetPathTypeOption(string pathOption, Material pathMaterial)
    {
        foreach (GameObject path in Path_Narrow)
        {
            path.GetComponent<MeshRenderer>().material = pathMaterial;
        }

        foreach (GameObject path in Path_Normal)
        {
            path.GetComponent<MeshRenderer>().material = pathMaterial;
        }

        foreach (GameObject path in Path_Wide)
        {
            path.GetComponent<MeshRenderer>().material = pathMaterial;
        }
    }

    public virtual void SetPathSizeOption(string pathOption)
    {
        foreach (GameObject path in Path_Narrow)
        {
            path.SetActive(pathOption == VisualFlow.PATHSIZEOPTION_NARROW);
        }

        foreach (GameObject path in Path_Normal)
        {
            path.SetActive(pathOption == VisualFlow.PATHSIZEOPTION_NORMAL);
        }

        foreach (GameObject path in Path_Wide)
        {
            path.SetActive(pathOption == VisualFlow.PATHSIZEOPTION_WIDE);
        }
    }
}
