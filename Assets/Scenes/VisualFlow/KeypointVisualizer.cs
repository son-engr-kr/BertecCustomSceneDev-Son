using System.Collections.Generic;
using UnityEngine;

public class KeypointVisualizer : MonoBehaviour
{
    public const string KP_1_ID = "kp1";
    public const string KP_2_ID = "kp2";

    [Header("Visual Flow Manager")] public VisualFlow VisualFlowManager;

    // Objects
    [Header("Keypoint Visualization Objects")]
    public KeypointVisualizerObject[] KeyPointVisualizers = new KeypointVisualizerObject[2];

    [Header("Keypoint Visualization Points")]
    public Transform FirstPersonKeypointParent;

    public Transform ThirdPersonKeypointParent;

    private Bertec.KeyPointVisualizerEvents.VisualizerMode visualizerMode =
        Bertec.KeyPointVisualizerEvents.VisualizerMode.None;

    // Tracks the set values so when toggline 1st/3rd person view things snap back
    private readonly Dictionary<string, string> currentKeypointmodes = new();


    private void Awake()
    {
        // default all the keypoint objects to nothing until the protocol updates the scene correctly
        // (empty id disabled the keypoint object)
        SetKeypointVisualizer(KeypointVisualizer.KP_1_ID, "");
        SetKeypointVisualizer(KeypointVisualizer.KP_2_ID, "");

        Bertec.KeyPointVisualizerEvents.VisualizerModeChanged += SetVisualizerMode;
        Bertec.KeyPointVisualizerEvents.VisualizerChanged += SetKeypointVisualizer;
        Bertec.KeyPointVisualizerEvents.KeypointPositionUpdated += UpdateKeypointPosition;
    }

    private void OnDestroy()
    {
        Bertec.KeyPointVisualizerEvents.VisualizerModeChanged -= SetVisualizerMode;
        Bertec.KeyPointVisualizerEvents.VisualizerChanged -= SetKeypointVisualizer;
        Bertec.KeyPointVisualizerEvents.KeypointPositionUpdated -= UpdateKeypointPosition;
    }

    private void SetVisualizerMode(Bertec.KeyPointVisualizerEvents.VisualizerMode mode)
    {
        visualizerMode = mode;

        switch (visualizerMode)
        {
            // when using none or just cop mode, disable all the keypoints but also keep the current state
            case Bertec.KeyPointVisualizerEvents.VisualizerMode.None:
            case Bertec.KeyPointVisualizerEvents.VisualizerMode.CoP:
                // Disable Keypoint visualizers
                DisableKeypointVisualizers();
                break;
            case Bertec.KeyPointVisualizerEvents.VisualizerMode.FirstPerson:
                UpdateParents(FirstPersonKeypointParent);
                ReenableKeypointVisualizers();
                break;
            case Bertec.KeyPointVisualizerEvents.VisualizerMode.ThirdPerson:
                UpdateParents(ThirdPersonKeypointParent);
                ReenableKeypointVisualizers();
                break;
        }
    }

    private KeypointVisualizerObject KPOforId(string keyid)
    {
        switch (keyid)
        {
            case KeypointVisualizer.KP_1_ID:
                return KeyPointVisualizers[0];
            case KeypointVisualizer.KP_2_ID:
                return KeyPointVisualizers[1];
        }

        return null;
    }

    private void SetKeypointVisualizer(string keyid, string visualizerOption)
    {
        currentKeypointmodes[keyid] = visualizerOption;
        SetKeypointVisualizer_ND(keyid, visualizerOption);
    }

    private void SetKeypointVisualizer_ND(string keyid, string visualizerOption)
    {
        KeypointVisualizerObject go = KPOforId(keyid);
        go?.SetVisual(visualizerOption);
    }

    private void UpdateKeypointPosition(string keyid, Vector3 newPosition)
    {
        // Time to do some input sanitization here
        Vector3 temp = newPosition;
        // If the y is less than 0, which means it should go into the ground
        // it won't above the sky
        temp.y = Mathf.Clamp(temp.y, 0, 2.5f);
        // We won't let the visualizer to go left of the path or right of the path
        temp.x = Mathf.Clamp(temp.x, VisualFlowManager.minLeftCursorPos,
            VisualFlowManager.maxRightCursorPos);
        // We won't let the visualizer to go behind the camera, but it also won't go too far
        temp.z = Mathf.Clamp(temp.z, 0, 1f);

        newPosition = temp;

        if (visualizerMode == Bertec.KeyPointVisualizerEvents.VisualizerMode.FirstPerson ||
            visualizerMode == Bertec.KeyPointVisualizerEvents.VisualizerMode.ThirdPerson)
        {
            KeypointVisualizerObject kvgo = KPOforId(keyid);
            if (kvgo)
            {
                kvgo.transform.localPosition = newPosition;
            }
        }
    }

    // Disable the keypoints without changing their visual ui
    private void DisableKeypointVisualizers()
    {
        foreach (KeypointVisualizerObject keypointVisualizer in KeyPointVisualizers)
        {
            keypointVisualizer?.gameObject.SetActive(false);
        }
    }

    private void ReenableKeypointVisualizers()
    {
        DisableKeypointVisualizers();

        foreach (KeyValuePair<string, string> kvp in currentKeypointmodes)
        {
            string key = kvp.Key;
            string value = kvp.Value;

            SetKeypointVisualizer_ND(key, value);
        }
    }

    private void UpdateParents(Transform newParent)
    {
        foreach (var keypointVisualizer in KeyPointVisualizers)
        {
            if (keypointVisualizer != null)
            {
                keypointVisualizer.transform.SetParent(newParent);
                keypointVisualizer.transform.localPosition = Vector3.zero;
            }
        }
    }
}