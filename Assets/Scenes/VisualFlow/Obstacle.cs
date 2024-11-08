using System.Collections;
using UnityEngine;

/// <summary>
/// Obstacle controls creating the obstacle object in front of the player and moving them left and right.
/// Depending on the difficultly level, how close the obstacles are to the player and often they will reappear changes.
/// </summary>

public class Obstacle : MonoBehaviour
{
    public AudioSource ObstacleHitSound;
    public AudioSource ObstacleMissSound;
    public Material ObstacleHitMaterial;
    private bool isObstacleAudioFeedbackEnabled = false;

    public float ObstacleMissZLimit = 3.5f;    // set to match the keypoint visualizer's z pos in the scene start minus the size
    public float ObstacleRespawnLimit = 0; // when the obstacle goes to here, move it and schedule respawn

    public static int HitCount = 0, MissCount = 0;

    public enum Status
    {
        NotChecking = -1,       // set when disabled
        Armed = 0,              // set when the object is reset and is waiting to respawn
        WaitingForHitMiss,      // waiting for hit or miss; set to this state when enabled and respawned
        HitPlayer,              // hit the player object in trigger. Will go into Armed once reaches respawn trigger point
        MissedPlayer            // position moved behind the scene's ObstacleMissZLimit value. Will go into Armed once reaches respawn trigger point
    };

    public Status hittingStatus = Status.NotChecking;   // reset when item is renabled and put back into to play; set to hit when collided or miss when behind camera

    private bool movingLeft = false;

    // The limits to the obstacle's movement path. Changes based on the path size
    private float leftSideBorder = -1, rightSideBorder = 1;

    private Vector3 startingPosition;
    private float routineTime = -1f;
    private int routineId = -1;

    private const float movementSpeed = 1f; // how fast the ball rolls left and right

    private MeshRenderer[] _meshRenderers;
    private Material[] _originalMaterials;

    private void Awake()
    {
        GetOriginalMaterials();
    }

    private void Start()
    {
        movingLeft = Random.value < 0.5f;
    }

    private void OnDisable()
    {
        Bertec.Utils.Coroutines.Stop(routineId);
        routineId = -1;
    }


    /// <summary>
    /// Get and cache the original materials on the original renderers.
    /// This is important because the obstacle is re-used and if it is hit, its materials will change to hit material.
    /// Upon re-enabling the obstacle, the original materials need to be restored.
    /// </summary>
    private void GetOriginalMaterials()
    {
        if (_meshRenderers == null || _meshRenderers.Length == 0)
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        }

        if (_originalMaterials == null || _originalMaterials.Length == 0)
        {
            _originalMaterials = new Material[_meshRenderers.Length];
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _originalMaterials[i] = _meshRenderers[i].material;
            }
        }
    }

    // Called when the protocol itself triggers a restart or explicitly resets the hit/miss counts
    public static void ResetHitMissCounts()
    {
        HitCount = MissCount = 0;
    }

    void Update()
    {
        switch (hittingStatus)
        {
            case Status.NotChecking:
                break;
            case Status.Armed:
                break;
            case Status.WaitingForHitMiss:
                // If the Obstacle is behind the player, count as a miss and move it out of the way, then schedule the retrigger
                if (transform.position.z < ObstacleMissZLimit)
                {
                    hittingStatus = Status.MissedPlayer;
                    ++MissCount;
                    // Tell the pc side the user missed hitting the Obstacle, which can be logged and/or displayed on the screen
                    Bertec.ObstacleEvents.UpdateHitMiss(HitCount, MissCount);
                    if (isObstacleAudioFeedbackEnabled)
                    {
                        ObstacleMissSound.Play();
                    }
                }
                break;
            case Status.HitPlayer:
            case Status.MissedPlayer:
                if (transform.position.z < ObstacleRespawnLimit)
                {
                    Bertec.Utils.Coroutines.Stop(routineId);
                    routineId = Bertec.Utils.Coroutines.Start(DelayReInstateObstacle());    // goes into armed state
                }
                break;
        }


        // move the Obstacle it until it reaches the border, then switch the direction
        if (movingLeft)
        {
            transform.Rotate(0, 0, Time.deltaTime * movementSpeed * 200f, Space.Self); // the 200f value is match the apparent rotation speed against the movement speed
            transform.Translate(Vector3.left * (Time.deltaTime * movementSpeed), Space.World);
            if (transform.position.x < leftSideBorder)
                movingLeft = false;
        }
        else
        {
            transform.Rotate(0, 0, -Time.deltaTime * movementSpeed * 200f, Space.Self);
            transform.Translate(Vector3.right * (Time.deltaTime * movementSpeed), Space.World);
            if (transform.position.x > rightSideBorder)
                movingLeft = true;
        }
    }

    /// <summary>
    /// Moves the obstacle towards the player.
    /// </summary>
    /// <param name="speed">The speed at which the obstacle should move.</param>
    public void MoveObstacle(float speed)
    {
        if (isActiveAndEnabled)
        {
            transform.Translate(Vector3.back * speed, Space.World);
        }
    }

    // This the player has hit the Obstacle; update the hit count and inform the UI
    private void OnTriggerEnter(Collider other)
    {
        if (isActiveAndEnabled && (hittingStatus == Status.WaitingForHitMiss))
        {
            hittingStatus = Status.HitPlayer;  // so that the miss doesn't count
            ++HitCount;
            Bertec.ObstacleEvents.UpdateHitMiss(HitCount, MissCount);
            // If the audio feedback is enabled, play the hit sound so there is actually an audio feedback to user
            if (isObstacleAudioFeedbackEnabled)
            {
                ObstacleHitSound.Play();
            }
            // Update the obstacle with the hit material so there is actually a visual feedback to user
            UpdateHitMaterial();
        }
    }

    /// <summary>
    /// Sets the left and right borders of the obstacle's movement path.
    /// </summary>
    public void SetBorders(float left, float right)
    {
        leftSideBorder = left;
        rightSideBorder = right;
    }

    /// <summary>
    /// Disables the obstacle and informs the PC side it is now hidden
    /// </summary>
    public void DisableObstacle()
    {
        Bertec.Utils.Coroutines.Stop(routineId);
        routineId = -1;
        hittingStatus = Status.NotChecking;
        ResetObstacleMaterial();
        gameObject.SetActive(false);
        Bertec.ObstacleEvents.ObstacleHidden();
    }

    /// <summary>
    /// Sets the obstacle to the easy difficulty level.
    /// </summary>
    public void SetObstacleEasy()
    {
        SetObstacle(20f, 14f);
    }

    /// <summary>
    /// Sets the obstacle to the medium difficulty level.
    /// </summary>
    public void SetObstacleMedium()
    {
        SetObstacle(20f, 11.5f);
    }

    /// <summary>
    /// Sets the obstacle to the hard difficulty level.
    /// </summary>
    public void SetObstacleHard()
    {
        SetObstacle(15f, 9f);
    }

    /// <summary>
    /// Sets the obstacle position and reinstate time values; informs the PC side the Obstacle is now visible
    /// </summary>
    private void SetObstacle(float rTime, float zPos)
    {
        routineTime = rTime;
        startingPosition.z = zPos;
        ReInstateObstacle();
    }

    /// <summary>
    /// Coroutine to reinstate the obstacle after a certain time.
    /// </summary>
    private IEnumerator DelayReInstateObstacle()
    {
        hittingStatus = Status.Armed;
        transform.position = new Vector3(0, 0, 50000f); // far enough out to not be visible
        yield return Bertec.Utils.Coroutines.WaitForSeconds(routineTime);
        ReInstateObstacle();
    }

    private void ReInstateObstacle()
    {
        Bertec.Utils.Coroutines.Stop(routineId);
        routineId = -1;
        ResetObstacleMaterial();
        movingLeft = Random.value < 0.5f;
        startingPosition = new Vector3(Random.Range(leftSideBorder * .8f, rightSideBorder * .8f), 0.24f, startingPosition.z);  // always needs to be above ground a bit
        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        hittingStatus = Status.WaitingForHitMiss;
        Bertec.ObstacleEvents.ObstacleDisplayed();
    }


    /// <summary>
    /// Sets the obstacle audio feedback state.
    /// </summary>
    /// <param name="state">new state, true to enable obstacle audio feedback, false to disable</param>
    public void SetAudioFeedback(bool state)
    {
        isObstacleAudioFeedbackEnabled = state;
    }

    /// <summary>
    /// Updates the obstacle mesh with the hit material, so the visibility of the obstacle is updated
    /// and a visual feedback is given to the user.
    /// </summary>
    private void UpdateHitMaterial()
    {
        // If the materials and/or renderers are not cached, get them
        GetOriginalMaterials();
        // Assign hit material to the obstacle
        foreach (MeshRenderer t in _meshRenderers)
        {
            t.material = ObstacleHitMaterial;
        }
    }

    /// <summary>
    /// Reset the obstacle materials so the obstacle is visible with its default materials again,
    /// Indicating that it's ready for the next hit or miss.
    /// </summary>
    private void ResetObstacleMaterial()
    {
        // If the materials and/or renderers are not cached, get them
        GetOriginalMaterials();
        // Assign original materials back
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].material = _originalMaterials[i];
        }
    }
}
