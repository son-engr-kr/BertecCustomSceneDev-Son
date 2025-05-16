using Bertec;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomSceneController : MonoBehaviour
{
    // Using variable names for the option keys helps avoid any copy-paste mistakes that can happen when using raw strings.
    // It also makes the code easier to read when differentiating between say PERIPHERALOPTION_NONE and OBSTACLEOPTION_NONE
    public const string LIGHTINGOPTION = "lighting";
    public const string LIGHTINGOPTION_LIGHT = "light";
    public const string LIGHTINGOPTION_DARK = "dark";

    public const string PERIPHERALOPTION = "periphery";
    public const string PERIPHERALOPTION_PYRAMIDS = "pyramids";
    public const string PERIPHERALOPTION_RECTANGLES = "rectangles";
    public const string PERIPHERALOPTION_NONE = "none";

    public const string PATHTYPEOPTION = "pathtype";
    public const string PATHTYPEOPTION_SPOTTED = "spotted";
    public const string PATHTYPEOPTION_SOLID = "solid";

    public const string PATHSIZEOPTION = "pathsize";
    public const string PATHSIZEOPTION_NARROW = "narrow";
    public const string PATHSIZEOPTION_NORMAL = "normal";
    public const string PATHSIZEOPTION_WIDE = "wide";

    public const string OBSTACLEOPTION = "obstacle";
    public const string OBSTACLEOPTION_NONE = "none";
    public const string OBSTACLEOPTION_EASY = "easy";
    public const string OBSTACLEOPTION_MEDIUM = "medium";
    public const string OBSTACLEOPTION_HARD = "hard";
    
    public const string OBSTACLEOPTION_AUDIO_FEEDBACK = "obstacle_audio_feedback";

    public const string KEYPOINTVIZOPTION_INVISIBLE = "none";
    public const string KEYPOINTVIZOPTION_BALL = "ball";
    public const string KEYPOINTVIZOPTION_STAR = "star";

    public const string AUDIOAMBIANCEOPTION = "audioambiance";
    public const string VISUALDISTRACTIONSOPTION = "visualdistractions";


    // This defines the scene info used by the Bertec system to display the scene in the UI.
    // The required Key value is used as a unique identifier within the project for these scene; it is not shown to the user.
    // The required Name value is what is shown to the user on the UI and should be descriptive but short.
    // Optionally, you can also set the Description text with a longer description of the scene; the UI will show this in a tooltip or popup message.
    // The required Scene value is used to identify the Unity scene file to load. Scene names starting with the @ symbol indicate
    // the scene file should be located by the filename (ex: @VisualFlow will look for VisualFlow.unity in the Assets folder).
    // If the @ symbol is NOT used, the Scene value must be the full path to the scene file - in this case, Assets\Scenes\VisualFlow\VisualFlow.unity)
    [SceneInfo(Key = "customhallwayscene", Name = "Custom Hallway Scene", Scene = "@CustomHallwayScene")]
    public class CustomSceneInfo : SceneInfo
    {
        public CustomSceneInfo()
        {
            Features = new SceneFeatures
            {
                VisualFlow = true, // will turn on the Visual Flow option in the UI
                HasPostProcessing = true, // will turn on Grain and Vignette options in the UI
                HasObstacles = true, // implies a UI option set
                CameraRotation =
                    CameraRotations
                        .BothSinusoidalAndRotate, // will turn on various Rotation options in the UI (sinusoidal, etc)

                Cognitive = Cognitive.MakeCognitive(),

                // defines the Keypoint Visualizer Options that the UI will present. Without this there will be no keypoints or cop handled
                // Setting the CoPKeypointVisualizers value enables the force plate COP keypoint tracking.
                // The KeyPointVisualizerEvents object will trigger VisualizerModeChanged, VisualizerChanged, and KeypointPositionUpdated as needed
                CoPKeypointVisualizers = new SceneChoices(new()
                {
                    {KEYPOINTVIZOPTION_INVISIBLE, "Invisible"},
                    {KEYPOINTVIZOPTION_BALL, "Blue Sphere"},
                    {KEYPOINTVIZOPTION_STAR, "Yellow Star"},
                    // the scene will allow the user to switch between the blue ball and the star.
                    // Cop's are tied into the force plate COP value
                }, KEYPOINTVIZOPTION_INVISIBLE),
            };

            // Setting the KeypointVisualizers enables the motion capture keypoint tracking. This is separate from COP.
            // The KeyPointVisualizerEvents object will trigger VisualizerModeChanged, VisualizerChanged, and KeypointPositionUpdated as needed

            // This keypoint is either on or off; since there is only one choice, the ui will not give you a seleciton but only let to assign
            // or un-assign a mocap data source
            Features.AddKeypointVisualizer(KeypointVisualizer.KP_1_ID, "Keypoint 1",
                new SceneChoices(new()
                {
                    // you can add more visuals here, and you don't have the use the same yellow star as COP.
                    {
                        KEYPOINTVIZOPTION_STAR, "Yellow Star"
                    },
                }, KEYPOINTVIZOPTION_STAR)
            );

            // This shows how to have a second keypoint visual with a different set of options.
            // You can have as few or as many keypoint visuals as you want, and each one can have 1 or more choice options.
            // The ui will show you a drop list of the two visual choices to have here. You can add more if you want - for example,
            // you could add in KEYPOINTVIZOPTION_INVISIBLE and support a keypoint that tracks mocap data but doesn't show on the screen.
            Features.AddKeypointVisualizer(KeypointVisualizer.KP_2_ID, "Keypoint 2",
                new SceneChoices(
                    new()
                    {
                        {
                            KEYPOINTVIZOPTION_STAR, "Yellow Star"
                        },
                        {
                            KEYPOINTVIZOPTION_BALL, "Blue Sphere"
                        },
                    },
                    KEYPOINTVIZOPTION_BALL)
            );

            // Add in the various scene options.

            const string GroupSection = "Environment";
            
            // Scene lighting options
            SceneOption lighting = AddList(LIGHTINGOPTION, "Lighting");
            lighting.Group = GroupSection;
            lighting.AddChoice(LIGHTINGOPTION_LIGHT, "Light", true); // this will be the default choice
            lighting.AddChoice(LIGHTINGOPTION_DARK, "Dark");

            const string SubGroupPath = "Path";
            
            SceneOption pathSize = AddList(PATHSIZEOPTION, "Size");
            pathSize.Group = GroupSection;
            pathSize.Subgroup = SubGroupPath;
            pathSize.AddChoice(PATHSIZEOPTION_NARROW, "Narrow");
            pathSize.AddChoice(PATHSIZEOPTION_NORMAL, "Normal", true); // this will be the default choice
            pathSize.AddChoice(PATHSIZEOPTION_WIDE, "Wide");
            
            SceneOption pathType = AddList(PATHTYPEOPTION, "Texture");
            pathType.Group = GroupSection;
            pathType.Subgroup = SubGroupPath;
            pathType.AddChoice(PATHTYPEOPTION_SPOTTED, "Spotted Tile");
            pathType.AddChoice(PATHTYPEOPTION_SOLID, "Solid Colored Tile", true); // this will be the default choice

            // Peripheral options
            SceneOption peripheral = AddList(PERIPHERALOPTION, "Periphery");
            peripheral.Group = GroupSection;
            peripheral.AddChoice(PERIPHERALOPTION_NONE, "None", true); // this will be the default choice
            peripheral.AddChoice(PERIPHERALOPTION_PYRAMIDS, "Pyramids");
            peripheral.AddChoice(PERIPHERALOPTION_RECTANGLES, "Cubes");

            const string SubGroupObstacles = "Obstacles";

            // This shows an alternative way to define an option instead of the AddList/AddChoice calls.
            // With this pattern, you add the scene option object directly instead of calling functions.
            // This can be more concise but may be harder to read.
            SceneOption obstacleOptions = AddOption(new SceneOption()
            {
                Type = SceneOption.OptionType.Choicelist,
                Key = OBSTACLEOPTION,
                Name = "Mode",
                Choices = new SceneChoices(
                    new List<SceneChoiceItem>()
                    {
                        {OBSTACLEOPTION_NONE, "None"},
                        {OBSTACLEOPTION_EASY, "Easy"},
                        {OBSTACLEOPTION_MEDIUM, "Medium"},
                        {OBSTACLEOPTION_HARD, "Hard"}
                    }, OBSTACLEOPTION_NONE)
            });
            obstacleOptions.Group = GroupSection;
            obstacleOptions.Subgroup = SubGroupObstacles;

            SceneOption obstacleAudioFeedback = AddCheckbox(OBSTACLEOPTION_AUDIO_FEEDBACK, "Audio Feedback");
            obstacleAudioFeedback.Group = GroupSection; // defaults to unchecked
            obstacleAudioFeedback.Subgroup = SubGroupObstacles;

            // Another example of adding an option, this time instead of using AddCheckbox
            AddOption(new SceneOption()
            {
                Type = SceneOption.OptionType.Checkbox,
                Key = VISUALDISTRACTIONSOPTION,
                Name = "Bubbles",
                Group = GroupSection,
                Subgroup = "Distractions"
            });

            // Scene audio
            AddCheckbox(AUDIOAMBIANCEOPTION, "Scene Audio").Group = GroupSection; // defaults to unchecked
        }
    }

    // The Unity scene properties

    public bool
        TestRunning =
            false; // exposed as a public value so it can be toggled on and off in the editor. Typically handled by the UI

    public float
        VisualFlowSpeed =
            0.0f; // exposed as a public value so it can be adjusted in the editor; otherwise is mapped to the system belt speed

    // UpdateVisualFlowSpeed and MoveGroundBlocks
    public float
        FlowSpeedScaler =
            0.2f; // this is used to scale the belt speed to the visual flow speed. The belt speed is in meters per second

    // while the visual flow speed is in units per second. This value is used to convert the belt speed to the visual flow speed.
    // This value is set in the editor and should be tuned to match the visual flow speed to the belt speed.
    public float
        BlockRepostionOffset =
            27f; // used to determine how far back from the camera position the ground block should be
    // before it triggers a reposition to in front. The value should be tuned to match the overall
    // size of the block and the scene to avoid a jump effect in front of the player.

    [Header("Ground Blocks")]
    public GroundBlock[]
        GroundBlocks; // the way that the visual flow scene is designed, blocks of 'terrain' are moved to give the appearance of forward motion

    [Header("Scene Objects")] public Light DirectionalLight;

    [Header("Light Related")] public Material DayTimeSkybox;
    public Material NightTimeSkybox;

    [Header("Obstacle Related")] public Obstacle ObstacleObject;
    public float Path_Narrow_Right_Border;
    public float Path_Narrow_Left_Border;
    public float Path_Normal_Right_Border;
    public float Path_Normal_Left_Border;
    public float Path_Wide_Right_Border;
    public float Path_Wide_Left_Border;

    [Header("Distractions")] public GameObject[] Distraction_Bubbles;

    [Header("Cognitive")] public Cognitive CognitiveObject; // this handles the Cognitive stuff

    [Header("Audio")] public AudioSource AmbianceSound;

    [Header("Keypoint Visualizers")] public GameObject COP_Container;
    public GameObject COP_BlueSphere;
    public GameObject COP_YellowStar;

    /*
    public GameObject MocapKeypoint_KP1;
    public GameObject MocapKeypoint_KP1_YellowStar;
    public GameObject MocapKeypoint_KP2;
    public GameObject MocapKeypoint_KP2_BlueSphere;
    public GameObject MocapKeypoint_KP2_YellowStar;
    */

    [Header("Materials")] public Material Path_SpottedRock;
    public Material Path_SolidColorTile;


    public readonly float
        CursorPosGain =
            10.0f; // used to scale the force plate input to something Unity can handle; your code may need something else

    [NonSerialized]
    public float minLeftCursorPos, maxRightCursorPos; // set from the Path_... values

    // current visualizer mode
    [HideInInspector]
    public KeyPointVisualizerEvents.VisualizerMode visualizerMode = KeyPointVisualizerEvents.VisualizerMode.None;
    private string currentCopKeypointImageSelection = "";

    void Awake()
    {
        TestRunning = false;
        VisualFlowSpeed = 0;

        // Reset all the options to their defaults so the scene starts in a known state. The options handler will call OptionChanged
        // during the start phase with any options set in the UI.
        
        // UI shows default, NONE, so no CoP or Keypoint Should be assigned.
        //SetKeypointVisualizer(KeyPointVisualizerEvents.FORCEPLATEKEYPOINTTAG, KEYPOINTVIZOPTION_BALL);
        //SetKeypointVisualizer(KEYPOINTVIZOPTION_STAR, "");  // turn off the mocap star until the user selects one in the ui
        
        SetLightingOption(LIGHTINGOPTION_LIGHT);
        SetObstacleOption(OBSTACLEOPTION_NONE);
        SetPathSizeOption(PATHSIZEOPTION_NORMAL);
        SetPathTypeOption(PATHTYPEOPTION_SOLID);
        SetPeripheralOption(PERIPHERALOPTION_NONE);
        SetAudioAmbiance(false);
        SetObstacleAudioFeedbackOption(false);
        SetVisualDistraction(false);

        OptionChangedContainer.Connect(OptionChanged);

        // This updates the scene movement
        VisualFlowMovementSpeed.SpeedUpdated += UpdateVisualFlowSpeed;

        // Handle the key points/cop position
        KeyPointVisualizerEvents.VisualizerModeChanged += SetVisualizerMode;
        KeyPointVisualizerEvents.VisualizerChanged += SetKeypointVisualizer;
        KeyPointVisualizerEvents.KeypointPositionUpdated += KeypointPositionUpdated; // both cop and motion capture keypoints come through here

        ObstacleEvents.OnResetHitMissCounts += ResetHitMissCounts;

        if (ObstacleObject!=null)
        {
            Bounds kpBounds = Utils.GetBoundingForGameObject(COP_Container);
            Bounds ooBounds = Utils.GetBoundingForGameObject(ObstacleObject.gameObject);
            ObstacleObject.ObstacleMissZLimit = COP_Container.transform.position.z -
                (kpBounds.extents.z + ooBounds.extents.z); 
            // missing will be just behind the trailing edge of the keypoint and just before the leading edge of the obstacle.
            // You can adjust this a bit forwards (ex: * 0.9f) or back (*1.1f) depending on how you like it to 'feel'
        }
    }

    private void OnDestroy()
    {
        VisualFlowMovementSpeed.SpeedUpdated -= UpdateVisualFlowSpeed;
        KeyPointVisualizerEvents.VisualizerModeChanged -= SetVisualizerMode;
        KeyPointVisualizerEvents.VisualizerChanged -= SetKeypointVisualizer;
        KeyPointVisualizerEvents.KeypointPositionUpdated -= KeypointPositionUpdated;
        ObstacleEvents.OnResetHitMissCounts -= ResetHitMissCounts;
    }

    private void SetVisualizerMode(KeyPointVisualizerEvents.VisualizerMode mode)
    {
        visualizerMode = mode;
        // Here you could handle the various modes and turn certain things on and off to handle first or third person positioning.
        visualizerMode = mode;
        switch (visualizerMode)
        {
            case KeyPointVisualizerEvents.VisualizerMode.CoP:
                SetKeypointVisualizer(KeyPointVisualizerEvents.FORCEPLATEKEYPOINTTAG,
                    currentCopKeypointImageSelection);
                break;
            case KeyPointVisualizerEvents.VisualizerMode.None:
            case KeyPointVisualizerEvents.VisualizerMode.FirstPerson:
            case KeyPointVisualizerEvents.VisualizerMode.ThirdPerson:
                DisableCoPVisualizers();
                break;
        }
    }

    private void DisableCoPVisualizers()
    {
        COP_BlueSphere?.SetActive(false);
        COP_YellowStar?.SetActive(false);
        COP_Container?.SetActive(false);
    }

    // When the UI changes the set keypoint visualizer, it will call this function with a UI-specific key id and one of the scene-specific
    // visualization option (in this case, none, ball, or star).
    private void SetKeypointVisualizer(string keyid, string visualizerOption)
    {
        currentCopKeypointImageSelection = visualizerOption;
        if (keyid == KeyPointVisualizerEvents.FORCEPLATEKEYPOINTTAG)    // this is used for the COP tracking
        {
            COP_Container?.SetActive(visualizerOption != "");
            COP_BlueSphere?.SetActive(visualizerOption == KEYPOINTVIZOPTION_BALL &&
                                      visualizerMode == KeyPointVisualizerEvents.VisualizerMode.CoP);
            COP_YellowStar?.SetActive(visualizerOption == KEYPOINTVIZOPTION_STAR &&
                                      visualizerMode == KeyPointVisualizerEvents.VisualizerMode.CoP);
        }
    }


    // This just updates the values; the position of the ball/star will be updated in Unity's Update function
    // The key id will either be the fixed cop id string, or one of the selected keypoint visualizers from the KeypointVisualizers list.
    // For this example, there is only one item - the star - so we don't have to check for multiple.
    void KeypointPositionUpdated(string keyid, Vector3 newpos)
    {
        if (keyid == KeyPointVisualizerEvents
                .FORCEPLATEKEYPOINTTAG) // the forceplate keypoint key id is a fixed well-known id that the PC side uses; it is seperate from the motion capture keypoints
        {
            // Update the ball/star position from the COG "x" position; this moves the visual left and right. For Visual Flow scenes,
            // typically you do not move forwards/back on the Y axis, only the X.
            // You could also handle the Fz value and implement some effect when there is no load (Fz < 20 newtons) such as hiding the visualizer
            // For this scene, we ignore the Fz value and the Y value
            if (COP_Container != null)
            {
                Vector3 v = COP_Container.transform.localPosition;
                if (!float.IsNaN(newpos.x)) // safety check to ensure Unity doesn't crash
                {
                    v.x = Mathf.Clamp(newpos.x * CursorPosGain, minLeftCursorPos,
                        maxRightCursorPos); // keep the cop cursor on the path like the moving obstacle
                }
                COP_Container.transform.localPosition = v;
            }
        }
    }

    // Reset the counts; called when the protocol is (re)started in the UI
    void ResetHitMissCounts()
    {
        Obstacle.ResetHitMissCounts();
    }

    private void Update()
    {
        if (VisualFlowSpeed > 0)
        {
            MoveGroundBlocks();
            ObstacleObject?.MoveObstacle(Time.deltaTime * VisualFlowSpeed * FlowSpeedScaler);
        }

#if UNITY_EDITOR
        TestOptionsWithKeyboard();
#endif
    }

#if UNITY_EDITOR
    // When testing the scene inside of Unity, it can be helpful to set up some keyboard controls. Useless in the headset,
    // which is why this is if-def'd out.
    private void TestOptionsWithKeyboard()
    {
        // test path size
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            SetPathSizeOption(PATHSIZEOPTION_NARROW);
        }
        else if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            SetPathSizeOption(PATHSIZEOPTION_NORMAL);
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            SetPathSizeOption(PATHSIZEOPTION_WIDE);
        }

        // test path type
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SetPathTypeOption(PATHTYPEOPTION_SOLID);
        }
        else if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            SetPathTypeOption(PATHTYPEOPTION_SPOTTED);
        }

        // test lighting
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            SetLightingOption(LIGHTINGOPTION_LIGHT);
        }
        else if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            SetLightingOption(LIGHTINGOPTION_DARK);
        }

        // test peripheral
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            SetPeripheralOption(PERIPHERALOPTION_PYRAMIDS);
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            SetPeripheralOption(PERIPHERALOPTION_RECTANGLES);
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            SetPeripheralOption(PERIPHERALOPTION_NONE);
        }

        // test keypoint visualizer
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            SetKeypointVisualizer(KeyPointVisualizerEvents.FORCEPLATEKEYPOINTTAG, KEYPOINTVIZOPTION_INVISIBLE);
        }
        else if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            SetKeypointVisualizer(KeyPointVisualizerEvents.FORCEPLATEKEYPOINTTAG, KEYPOINTVIZOPTION_BALL);
        }
        else if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            SetKeypointVisualizer(KeyPointVisualizerEvents.FORCEPLATEKEYPOINTTAG, KEYPOINTVIZOPTION_STAR);
        }

        // test obstacle
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            SetObstacleOption(OBSTACLEOPTION_EASY);
        }
        else if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            SetObstacleOption(OBSTACLEOPTION_MEDIUM);
        }
        else if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            SetObstacleOption(OBSTACLEOPTION_HARD);
        }
        else if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            SetObstacleOption(OBSTACLEOPTION_NONE);
        }

        // test ambiance sound
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            SetAudioAmbiance(true);
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SetAudioAmbiance(false);
        }

        // test visual distraction
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            SetVisualDistraction(true);
        }
        else if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            SetVisualDistraction(true);
        }

        // test start/stop
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            TestRunning = true;
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TestRunning = false;
        }

        // test flow speed
        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            VisualFlowSpeed = Mathf.Max(0, VisualFlowSpeed - 1.0f); // don't go backwards
        }
        else if (Keyboard.current.rightBracketKey.wasPressedThisFrame)
        {
            VisualFlowSpeed = Mathf.Min(20, VisualFlowSpeed + 1.0f); // up to 20 
        }
    }
#endif

    // This will be called whenever the user changes the option in the PC side and when the scene is loaded with the initial protocol options.

    void OptionChanged(string key, object val)
    {
        switch (key)
        {
            case LIGHTINGOPTION:
                SetLightingOption(Utils.toString(val));
                break;
            case PERIPHERALOPTION:
                SetPeripheralOption(Utils.toString(val));
                break;
            case PATHTYPEOPTION:
                SetPathTypeOption(Utils.toString(val));
                break;
            case PATHSIZEOPTION:
                SetPathSizeOption(Utils.toString(val));
                break;
            case OBSTACLEOPTION:
                SetObstacleOption(Utils.toString(val));
                break;
            case OBSTACLEOPTION_AUDIO_FEEDBACK:
                SetObstacleAudioFeedbackOption(Utils.toBool(val));
                break;
            case AUDIOAMBIANCEOPTION:
                SetAudioAmbiance(Utils.toBool(val));
                break;
            case VISUALDISTRACTIONSOPTION:
                SetVisualDistraction(Utils.toBool(val));
                break;
        }
    }

    private void SetLightingOption(string lightingOption)
    {
        switch (lightingOption)
        {
            case LIGHTINGOPTION_LIGHT:
                RenderSettings.skybox = DayTimeSkybox;
                ColorUtility.TryParseHtmlString("#FFF4D6", out Color dayTimeColor);
                DirectionalLight.color = dayTimeColor;
                break;
            case LIGHTINGOPTION_DARK:
                RenderSettings.skybox = NightTimeSkybox;
                ColorUtility.TryParseHtmlString("#1F1B5E", out Color nightTimeColor);
                DirectionalLight.color = nightTimeColor;
                break;
        }
    }

    private void SetPeripheralOption(string peripheralOption)
    {
        foreach (GroundBlock block in GroundBlocks)
        {
            block?.SetPeripheralOption(peripheralOption);
        }
    }

    private void SetPathTypeOption(string pathOption)
    {
        foreach (GroundBlock block in GroundBlocks)
        {
            block?.SetPathTypeOption(pathOption,
                pathOption == PATHTYPEOPTION_SPOTTED ? Path_SpottedRock : Path_SolidColorTile);
        }
    }

    private void SetPathSizeOption(string pathOption)
    {
        foreach (GroundBlock block in GroundBlocks)
        {
            block?.SetPathSizeOption(pathOption);
        }

        switch (pathOption)
        {
            case PATHSIZEOPTION_NARROW:
                SetBordersForObstacles(Path_Narrow_Left_Border, Path_Narrow_Right_Border);
                break;
            case PATHSIZEOPTION_NORMAL:
                SetBordersForObstacles(Path_Normal_Left_Border, Path_Normal_Right_Border);
                break;
            case PATHSIZEOPTION_WIDE:
                SetBordersForObstacles(Path_Wide_Left_Border, Path_Wide_Right_Border);
                break;
        }
    }

    private void SetBordersForObstacles(float left, float right)
    {
        minLeftCursorPos = left;
        maxRightCursorPos = right;
        ObstacleObject?.SetBorders(left, right);
    }

    private void SetObstacleOption(string obstacleOption)
    {
        switch (obstacleOption)
        {
            case OBSTACLEOPTION_NONE:
                ObstacleObject?.DisableObstacle();
                break;
            case OBSTACLEOPTION_EASY:
                ObstacleObject?.SetObstacleEasy();
                break;
            case OBSTACLEOPTION_MEDIUM:
                ObstacleObject?.SetObstacleMedium();
                break;
            case OBSTACLEOPTION_HARD:
                ObstacleObject?.SetObstacleHard();
                break;
        }
    }
    
    private void SetObstacleAudioFeedbackOption(bool state)
    {
        ObstacleObject?.SetAudioFeedback(state);
    }

    private void SetVisualDistraction(bool state)
    {
        foreach (GameObject distractionBubble in Distraction_Bubbles)
        {
            distractionBubble?.SetActive(state);
        }
    }

    // The audio ambiance is a looping music file that plays in the background. This function will start or stop the music.
    private void SetAudioAmbiance(bool state)
    {
        if (AmbianceSound != null)
        {
            if (state)
            {
                if (!AmbianceSound.isPlaying) // avoid restarting the sound loop
                    AmbianceSound.Play();
            }
            else
                AmbianceSound.Stop();
        }
    }


    // This is called whenever there is a change in either the fixed speed value or the treadmill belt speed.
    // For some applicaitons, using the seperate Left/Right belt speeds may be of use; for this example, we just use
    // the combined Speed value which is the max of the two.
    private void UpdateVisualFlowSpeed(FlowMovementSpeedData newSpeed)
    {
        VisualFlowSpeed = MathF.Max(0, newSpeed.Speed); // the treadmill can run in reverse but our scene cannot
    }

    // Move the blocks forward in time with the set speed. If the block is behind the camera, move it to the front.
    // This only works because the camera is at a fixed xyz position and we only rotate the view.
    private void MoveGroundBlocks()
    {
        // move the blocks first then reposition
        foreach (GroundBlock block in GroundBlocks)
        {
            block.transform.position += Vector3.back * (Time.deltaTime * VisualFlowSpeed * FlowSpeedScaler);
        }

        // Check if the block behind the player needs to come forwards
        for (int i = 0; i < GroundBlocks.Length; ++i)
        {
            if (GroundBlocks[i].transform.position.z < -BlockRepostionOffset)
            {
                // The block has moved far enough away that we need to reposition it to the front of the player. The prior
                // block in the list will be the one furthest out front, so use the Bounds of that block to determine the
                // z position.

                int otherindex = ((i - 1) + GroundBlocks.Length) % GroundBlocks.Length;
                GroundBlock srcBlock = GroundBlocks[otherindex];
                Bounds srcBounds =
                    Utils.GetBoundingForGameObject(srcBlock.gameObject); // this is always in world coords
                float newZ = srcBounds.center.z + srcBounds.size.z; // move the new center so that the edges line up
                Vector3 pos = GroundBlocks[i].transform.position;
                pos.z = newZ;
                GroundBlocks[i].transform.position = pos; // resets the world space position
            }
        }
    }
}