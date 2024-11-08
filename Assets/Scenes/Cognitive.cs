using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Cognitive shows and hides a word + color on the screen. The show and hide triggers are sent from the PC to the scene, where the scene controls what is show and where.
/// The scene can declare different types of cognitive choices, but for the purposes of this example code only Stroop is provided.
/// You can expand this to have other choices - for example, Pattern, Math, etc.
/// </summary>


public class Cognitive : MonoBehaviour
{
    public Canvas Panel;    // this will be this class itself, typically
    public TextMeshProUGUI Text;

    static readonly string COGNITIVECHOICE_STROOP = "stroop";
    static readonly string STROOP_CONGRUENT = "congruent";     // the text will match the color (ex: "Red" is red)
    static readonly string STROOP_INCONGRUENT = "incongruent"; // the text will NOT match the color (ex: "Red" is blue)
    static readonly string STROOP_RANDOM = "random";           // randomly picks either congruent or incongruent

    public static Bertec.CognitiveChoices MakeCognitive()
    {
        // defines how the cognitive options are presented in the UI. What these actually "are" is up to the scene to control.
        // CognitiveEvents.ShowCognitiveDisplay and CognitiveEvents.HideCognitiveDisplay are used to show/hide the cognitive display
        return new Bertec.CognitiveChoices(new Bertec.CognitiveChoiceItem
            {
                Key = COGNITIVECHOICE_STROOP,
                Name = "Stroop",
                SubOptionName = "Congruency",
                SubOptions =
                    new Bertec.SceneChoices(
                        new List<Bertec.SceneChoiceItem>()
                        {
                            {STROOP_CONGRUENT, "Congruent"},
                            {STROOP_INCONGRUENT, "Incongruent"},
                            {STROOP_RANDOM, "Random"}
                        }, STROOP_CONGRUENT) // defaults to Congruent
            }
        );
    }

    void Awake()
    {
        // The CognitiveEvents class will trigger show/hide and your code should handle presenting and reporting back to the ui
        Bertec.CognitiveEvents.ShowCognitiveDisplay += ShowCognitiveDisplay;
        Bertec.CognitiveEvents.HideCognitiveDisplay += HideCognitiveDisplay;
        HideCognitiveDisplay();
    }

    private void OnDestroy()
    {
        Bertec.CognitiveEvents.ShowCognitiveDisplay -= ShowCognitiveDisplay;
        Bertec.CognitiveEvents.HideCognitiveDisplay -= HideCognitiveDisplay;
    }

    // When the PC side triggers a Show Cognitive Display, the Unity side should generate the appropriate display and report back
    // to the PC side what it did. Here, we generate a simple bit of text on the screen with five different colors and words.
    private void ShowCognitiveDisplay(Bertec.ShowCognitiveData cogData)
    {
        if (cogData.ID == COGNITIVECHOICE_STROOP)
        {
            (string text, Color color)[] stroopTestColors =
            {
                    ("RED", Color.red),
                    ("BLUE", Color.blue),
                    ("GREEN", Color.green),
                    ("YELLOW", Color.yellow),
                    ("WHITE", Color.white),
                };

            int colorForText = Random.Range(0, stroopTestColors.Length);
            int nameForText = colorForText; // default to congruent
            bool doincongruent = (cogData.SubOption == STROOP_INCONGRUENT);
            if (cogData.SubOption == STROOP_RANDOM)
            {
                doincongruent = (Random.Range(0, 2) == 0);
            }

            if (doincongruent)
            {
                // pick another random value that is not the same as the colorForText so the text != color
                do
                {
                    nameForText = Random.Range(0, stroopTestColors.Length);
                } while (nameForText == colorForText);
            }

            Color color = stroopTestColors[colorForText].color;
            Text.text = stroopTestColors[nameForText].text;
            Text.color = stroopTestColors[colorForText].color;
            Text.faceColor = stroopTestColors[colorForText].color;
            Text.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor, stroopTestColors[colorForText].color);
            Panel.enabled = true;

            string textIs = stroopTestColors[nameForText].text;
            string colorIs = stroopTestColors[colorForText].text;

            // Report back to the PC what the text is, the color is, the correct response, and what should be verbally prompted to the user
            Bertec.CognitiveEvents.CognitiveDisplayed(textIs, colorIs, "What is the color of the word?", colorIs);
        }
        else
        {
            HideCognitiveDisplay();
        }
    }

    // Called when the PC side wants to hide the display. Simple disables it and reports back this is done. This will complete the
    // loop and trigger the next display based on the PC settings (typically a delay or condition requirement).
    private void HideCognitiveDisplay()
    {
        if (Panel.enabled)
        {
            Panel.enabled = false;
            Bertec.CognitiveEvents.CognitiveHidden();
        }
    }
}
