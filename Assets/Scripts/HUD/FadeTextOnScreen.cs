//Code by Eele Roet
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;

/// <summary>
/// <para> Shows text on screen that fades over time</para>
/// <para> Changes the text, starts fading the text after specified amount of time</para> 
/// </summary>
public class FadeTextOnScreen : MonoBehaviour
{
    [SerializeField] TMP_Text TextToFade;//the text element that gets faded
    private bool displaying = false;

    //public method that gets called when text needs to be displayed
    public void DisplayText(string newText, float seconds)
    {
        displaying = true;
        TextToFade.text = newText;//sets the new text
        StartCoroutine(FadeTextAfterSeconds(seconds));//fade the text after 'seconds' amount of seconds
    }

    private IEnumerator FadeTextAfterSeconds(float seconds)
    {
        TextToFade.enabled = true;//show the text
        yield return new WaitForSeconds(seconds);//wait for some time
        Color saveTextColorForLater = TextToFade.color;//saves the starting color
        Color colorToChange = saveTextColorForLater;//the color that slowly starts to fade out.
        while(TextToFade.color.a > 0)
        {
            colorToChange.a -= 0.016f;//decrease the alpha so the fadeout takes about a second
            TextToFade.color = colorToChange;//apply the new alpha
            yield return null;
        }
        //when the fade is done
        TextToFade.color = saveTextColorForLater;//reset the color
        TextToFade.enabled = false;//turn off the text
        displaying = false;
    }

    //public bool used to check if new text can be displayed
    public bool IsReadyForNewText()
    {
        return !displaying;
    }
}
