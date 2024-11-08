using UnityEngine;

public class KeypointVisualizerObject : MonoBehaviour
{
    public const string IMAGE_INVISIBLE = "invisible";
    public const string IMAGE_KEYPOINT_BALL = "ball";
    public const string IMAGE_KEYPOINT_STAR = "star";

    public GameObject YellowStar;
    public GameObject BlueSphere;

    private string currentImageSelection = "";

    private Collider _collider;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void SetCollisionEnabled(bool state)
    {
        if (!_collider)
        {
            _collider = GetComponent<Collider>();
        }
        _collider.enabled = state;
    }
    
    public Rigidbody KpRigidbody
    {
        get
        {
            if (!_rigidbody)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            return _rigidbody;
        }
    }

    public void SetVisual(string visualID)
    {
        currentImageSelection = visualID;
        // if invisible is passed, then all of these get turned off but the collider is still left active
        BlueSphere?.SetActive(visualID == IMAGE_KEYPOINT_BALL);
        YellowStar?.SetActive(visualID == IMAGE_KEYPOINT_STAR);

        SetCollisionEnabled(visualID != "");
        gameObject.SetActive(visualID != "");
    }
}