using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Static Scene is a black or blue background with a number of 'bubbles' on it. Unlike Visual Flow, Static Scenes are not designed
/// to move the user 'through' the scene. This does not mean that the scene itself cannot move - you can certainly implement some logic
/// so that the bubbles float around for example.
/// </summary>

public class StaticScene : MonoBehaviour
{
    // Using variable names for the option keys helps avoid any copy-paste mistakes that can happen when using raw strings.
    public const string BACKGROUNDOPTION = "background";
	public const string BACKGROUNDOPTION_BLACK = "black";
	public const string BACKGROUNDOPTION_BLUE = "blue";
	public const string BUBBLESOPTION = "bubbles";
	public const string BUBBLESOPTION_OFF = "off";  // the rest of the bubbles option are numbers, so no need to list here
    public const string VISUALAIDOPTION = "visualaid"; // this is a new option for the visual aid

    // This declares the scene info that will be used by the Bertec system to display the scene in the UI.
    // The required Key value is used as a unique identifier within the project for these scene; it is not shown to the user.
    // The required Name value is what is shown to the user on the UI and should be descriptive but short.
    // Optionally, you can also set the Description text with a longer description of the scene; the UI will show this in a tooltip or popup message.
    // The required Scene value is used to identify the Unity scene file to load. Scene names starting with the @ symbol indicate
    // the scene file should be located by the filename (ex: @StaticScene will look for StaticScene.unity in the Assets folder).
    // If the @ symbol is NOT used, the Scene value must be the full path to the scene file - in this case, Assets\Scenes\StaticScene\StaticScene.unity)
    [Bertec.SceneInfo(Key = "staticsceneexample", Name = "Static Scene Example", Scene = "@StaticScene")]
	public class StaticSceneInfo : Bertec.SceneInfo
	{
		public StaticSceneInfo()
		{
			Features = new Bertec.SceneFeatures
			{
				HasPostProcessing = true, // will turn on Grain and Vignette options in the UI
                HasCameraPosition = true, // will turn on Camera Position options in the UI
                Cognitive = Cognitive.MakeCognitive(), // will turn on the Cognitive options in the UI
				CameraRotation = Bertec.CameraRotations.BothSinusoidalAndRotate // will turn on various Rotation options in the UI (sinusoidal, etc)
			};
            const string GroupSection = "Environment";
            
            // Add in the background color option. This shows one way of adding options by adding a list of choices and then each choice
            Bertec.SceneOption backgroundOption = AddList(BACKGROUNDOPTION, "Background Color");
            backgroundOption.AddChoice(BACKGROUNDOPTION_BLACK, "Black", true);   // this will be the default choice
            backgroundOption.AddChoice(BACKGROUNDOPTION_BLUE, "Blue");
            backgroundOption.Group = GroupSection;

            // Add in the # of bubbles options. This shows another way of adding options; in this pattern, you add the scene option object
            // directly instead of calling functions. This can be more concise but may be harder to read.
            AddOption(new Bertec.SceneOption()
            {
                Type = Bertec.SceneOption.OptionType.Choicelist,
                Key = BUBBLESOPTION,
                Group = GroupSection,
                Name = "Object Density",   // what will be displayed in the Kinamoto UI
                Choices = new Bertec.SceneChoices(new List<Bertec.SceneChoiceItem>()
                {
                    { BUBBLESOPTION_OFF, "Off" },
                    { 5 },	// the key value will be the same as what is shown on the screen, so no need to do it twice
                    { 10 },
                    { 20 },
                    { 40 },
                    { 80 },
                    { 160 },
                    { 320 },
                    { 640 },	// higher is easily possible, but then the scene becomes so crowded it's not usable
                }, BUBBLESOPTION_OFF)   // the default is no bubbles
            });
            
            // Visual aid option
            Bertec.SceneOption visualAidOption = AddCheckbox(VISUALAIDOPTION, "Visual Aid");
            visualAidOption.Group = GroupSection;
            visualAidOption.SetDefault(false);
		}
	}

	public BubblePrefab bubblePrefab;
    public Cognitive cognitiveObject;
    public GameObject visualAidObject;
    public Transform bubbleParent;

    private List<BubblePrefab> bubbles = new List<BubblePrefab>();

    // Default value is off, so it must be 0.
	private int bubblesOptionValue = 0;

	void Awake()
	{
        Bertec.OptionChangedContainer.Connect(OptionChanged);

        if (bubblePrefab == null)
			Debug.LogError("Bubble prefab is not set; please check the StaticScene in the Unity editor!");
	}


	void Start()
    {
		CreateBubbles();
	}

    void OnDestroy()
    {
    }

    void Update()
	{
	}

	// This returns a value from the min to the max, with a step of 0.5 (ex: -2 to +2 will give -2.0, -1.5, -1.0 etc)
	// We do it this way because the Unity random range float version tends to cluster the results together and we want to try and keep
	// the positioning of the bubbles a little more spaced out.
	float RandomByHalves(int minValue, int maxValue)
	{
		int r = Random.Range(0, ((maxValue - minValue) * 2) + 1);   // the int version is max EXCLUSIVE, while the float is INCLUSIVE.
		return ((float)r / 2.0f) + (float)minValue;
	}

	// This is not perfect; ideally, this should create the bubbles in a viewable cone within the field of view. But it gets the job done.
	void CreateBubbles()
	{
		foreach (BubblePrefab b in bubbles)
			GameObject.Destroy(b.gameObject);

		bubbles.Clear();

		if (bubblePrefab != null)
		{
			Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
			for (int i = 0; i < bubblesOptionValue; i++)
			{
				BubblePrefab bubble = Instantiate(bubblePrefab, bubbleParent);
				Vector3 pos = new Vector3(RandomByHalves(-6, 6), RandomByHalves(-4, 4), RandomByHalves(3, 17));
				bubble.transform.position = cameraRotation * pos;  // this puts the bubble in front of the camera no matter the Y rotation
				bubble.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

				Quaternion qrot = bubble.transform.localRotation;
				Vector3 rot = qrot.eulerAngles;
				rot.z = Random.Range(0, 360 / 8) * 8;  // give it a random spin so they don't all look the same
				qrot.eulerAngles = rot;
				bubble.transform.localRotation = qrot;
				bubbles.Add(bubble);
			}
		}
	}

	Vector3 GetRandomPositionInFieldOfView()
	{
		Camera mainCamera = Camera.main;
		float fieldOfView = mainCamera.fieldOfView;
		float aspectRatio = mainCamera.aspect;
		float cameraRotation = mainCamera.transform.rotation.eulerAngles.y;

		float halfScreenWidth = Mathf.Tan(Mathf.Deg2Rad * fieldOfView);
		float halfScreenHeight = halfScreenWidth / aspectRatio;

		float randomX = Random.Range(-halfScreenWidth, halfScreenWidth);
		float randomY = Random.Range(-halfScreenHeight, halfScreenHeight);

		Vector3 randomPosition = new Vector3(randomX, randomY, Random.Range(3.0f, 11.0f));
		randomPosition = Quaternion.Euler(0f, cameraRotation, 0f) * randomPosition;

		return randomPosition;
	}

	// This will be called whenever the user changes the option in the PC side and when the scene is loaded with the initial protocol options.
	void OptionChanged(string key, object val)
	{
		if (key == BACKGROUNDOPTION)
		{
			switch (val.ToString())
			{
				case BACKGROUNDOPTION_BLACK:
					Camera.main.backgroundColor = Color.black;
					break;
				case BACKGROUNDOPTION_BLUE:
					Camera.main.backgroundColor = Color.blue;
					break;
			}
		}
		else if (key == BUBBLESOPTION)
		{
			if (val.ToString() == BUBBLESOPTION_OFF)
			{
				// turn the bubbles off
				bubblesOptionValue = 0;
				CreateBubbles();
			}
			else
			{
				int density = Bertec.Utils.toInt(val);
				if (density > 0)
				{
					// turn the bubbles on
					bubblesOptionValue = density;
					CreateBubbles();
				}
			}
		}
        else if (key == VISUALAIDOPTION)
        {
            visualAidObject.SetActive(Bertec.Utils.toBool(val));
        }
	}
}
