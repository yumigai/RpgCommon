using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class TrophyLib {

    public static void Auth()
    {
        Social.localUser.Authenticate (ProcessAuthentication);        
    }

    private static void ProcessAuthentication (bool success)
    {
        if (success) {
            Debug.Log ("Authenticated, checking achievements");
        } else {
            Debug.Log ("Failed to authenticate");
        }
    }

    public static void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    public static void ReportScore (long score)
    {

        string leaderboardID = "score";

        Social.ReportScore (score, leaderboardID, success => {

        });
    }
}