using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/**
 *  Adds a typewriter like function to TMP_Text objects
 *  Reference https://www.youtube.com/watch?v=U85gbZY6oo8
 */

public class TeleType : MonoBehaviour
{
    [Tooltip("Reference to TMP_Text component")]
    public TMP_Text tmpText;
    [Tooltip("Reference to background")]
    public Image backgroundPanelImage;
    [Tooltip("Next button (optional)")]
    public Transform nextButton;


    [Tooltip("Typing speed")]
    public float timeBetweenCharacters = .02f;
    [Tooltip("Number characters typed")]
    public int charactersTyped = 0;

    [Tooltip("Number characters allowed to be visible")]
    public int charactersVisible = 0;
    [Tooltip("Total characters of text object")]
    public int totalCharacters = 0;

    public int waitToRead = 0;


    void OnValidate()
    {
        tmpText = gameObject.GetComponent<TMP_Text>();
        backgroundPanelImage = transform.parent.GetComponent<Image>();
    }

    private void Awake()
    {
        backgroundPanelImage.enabled = false;
    }

    /// <summary>
    /// Call this public function to add text / start teletyper
    /// </summary>
    /// <param name="newText"></param>
    public void AddText(string newText, Bot bot)
    {
        // start teletype function
        StartCoroutine(StartTyping(newText, bot));
        // find any objects needed
        nextButton = transform.Find("NextButton");
    }


    IEnumerator StartTyping(string newText, Bot bot)
    {
        // if newText is empty
        if (newText.Length <= 0) yield break;

        // if we are still writing / reading
        if (waitToRead > 0) yield break;

        waitToRead = (int)newText.Length / 25;

        // reset text object's allowed visible characters
        tmpText.maxVisibleCharacters = 0;
        charactersTyped = 0;
        charactersVisible = 0;
        // add text
        tmpText.text = newText;
        //Debug.Log("characterCount = " + tmpText.textInfo.characterCount);
        // turn on background
        backgroundPanelImage.enabled = true;

        bool addCharacters = true;

        while (addCharacters)
        {
            // update characterCount (it takes a moment before this is actually accessible)
            totalCharacters = tmpText.textInfo.characterCount;
            // # that should be visible
            //charactersVisible = charactersTyped % (totalCharacters + 1);
            charactersVisible += 1;
            // set visible
            tmpText.maxVisibleCharacters = charactersVisible;
            // m_textMeshPro.maxVisibleLines = maxLines; // could use this to start over with new lines?

            // once characterCount is accessible
            if (totalCharacters > 0)
            {
                //  && the last character has been revealed...
                if (charactersVisible >= totalCharacters)
                {
                    // stop the loop
                    addCharacters = false;
                    // do next thing (button, more text, etc.
                    if (nextButton != null) nextButton.gameObject.SetActive(true);
                }
            }
            // the first time this runs this will be zero :-(
            //Debug.Log("characterCount = " + m_textInfo.characterCount);

            // safety to prevent crash
            if (++charactersTyped > 1000)
            {
                Debug.LogWarning("Safety first!");
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        while (waitToRead > 0)
        {
            yield return new WaitForSeconds(1f);
            waitToRead--;
        }

        Debug.Log("CleanUpAfterTeleType");

        // remove text
        tmpText.text = "";
        // turn off background
        backgroundPanelImage.enabled = false;
        // turn off bot (if still available)
        if (bot != null) bot.OnEndMessage();

    }
}