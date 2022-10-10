using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareScore : MonoBehaviour
{
    /*Twitter Sharing*/
    string TWITTER_ADDRESS = "https://twitter.com/intent/tweet";
    
    string TWEET_LUNGUAGE = "en";



    public void shareGlobalScore()
    {
        if (LevelLoader.Instance.placement == "!")
        {
            
        }
        else
        {
            string textToDisplay = "I ranked #" + LevelLoader.Instance.placement +
                                   " 🏆 \nAre you smarter than me? \nProve it now @CyberBloks \n#dixtra";
            Debug.Log(textToDisplay);
            Application.OpenURL(TWITTER_ADDRESS + "?text=" + WWW.EscapeURL(textToDisplay) + "&amp:language=" +
                                WWW.EscapeURL(TWEET_LUNGUAGE));
        }
    }
    public void shareLocalScore()
    {
        if (LevelLoader.Instance.placement == "!")
        {
            
        }
        else
        {
            string textToDisplay = "I ranked #" + LevelLoader.Instance.placement + " in Level-" +
                                   LevelLoader.Instance.currentIndex +
                                   " 🏆 \nAre you smarter than me? \nProve it now @CyberBloks \n#dixtra";
            Debug.Log(textToDisplay);
            Application.OpenURL(TWITTER_ADDRESS + "?text=" + WWW.EscapeURL(textToDisplay) + "&amp:language=" +
                                WWW.EscapeURL(TWEET_LUNGUAGE));
        }
    }

}
