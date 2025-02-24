﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class HSController : MonoBehaviour
{
    private string secretKey = "QuArrELSomeCorALHigHsCores"; // Edit this value and make sure it's the same as the one stored on the server
    public string addScoreURL = "http://quarrelsomecoral.x10host.com/addscore.php"; //be sure to add a ? to your url
    public string highscoreURL = "http://quarrelsomecoral.x10host.com/display.php";
    public InputField m_InputNameField;
    public Text m_HighScores;
    public Button m_SubmitScoreButton;

    void Start()
    {
        m_SubmitScoreButton.enabled = true;
    }

    // remember to use StartCoroutine when calling this function!
    public IEnumerator PostScores(string name, int score)
    {
        string hash = MD5Hasher.Md5Sum(name + score + secretKey);
        WWWForm form = new WWWForm();
        form.AddField("namePost", UnityWebRequest.EscapeURL(name));
        form.AddField("scorePost", score);
        form.AddField("hashPost", hash);

        using (var upload = UnityWebRequest.Post(addScoreURL, form))
        {
            upload.SetRequestHeader("content-type", "application/x-www-form-urlencoded");
            upload.SetRequestHeader("user-agent", "DefaultBrowser");
            upload.SetRequestHeader("cookie", string.Format("DummyCookie"));
            upload.chunkedTransfer = false;

            //Debug.Log(upload.certificateHandler);
            //upload.useHttpContinue = false;

            yield return upload.SendWebRequest();
            if (upload.isNetworkError || upload.isHttpError)
            {
                print("Upload error: " + upload.error);
            }
            else
            {
                StartCoroutine(GetScores());
            }
        }
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    public IEnumerator GetScores()
    {
        Debug.Log("Getting Scores");
        using (var download = UnityWebRequest.Get(highscoreURL))
        {
            download.SetRequestHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            download.SetRequestHeader("user-agent", "DefaultBrowser");
            download.SetRequestHeader("Access-Control-Allow-Origin", "*");
            download.SetRequestHeader("Access-Control-Allow-Methods", "HEAD, PUT, DELETE, POST, GET, OPTIONS");
            download.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type, Access-Control-Allow-Headers, text/html, Authorization, X-Requested-With, Origin, Accept,user-agent");
            download.chunkedTransfer = false;

            yield return download.SendWebRequest();

            if (download.isNetworkError || download.isHttpError)
            {
                print("Download error: " + download.error);
            }
            else
            {
                var scores = Regex.Matches(download.downloadHandler.text, ".*\n");
                m_HighScores.text = "";
                foreach (var score in scores)
                {
                    m_HighScores.text += score.ToString();
                }

                

            }
        }

    }

    public void SubmitHighScore()
    {
        StartCoroutine(PostScores(m_InputNameField.text, SubmarineManager.GetInstance().m_Score));
        m_SubmitScoreButton.enabled = false;
    }

}