using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using System;
using System.Linq;

public class Linker : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetURL();

    public static int round = 1;

    [SerializeField] GameObject _startGamneButton;
    [SerializeField] GameObject _playerFound;
    [SerializeField] GameObject _playButton;
    [SerializeField] TextMeshProUGUI _roundTournamentText;

    public static string m_myTeamId = "";
    public static string m_myId = "";
    private string m_tournamentId = "";
    private string m_environment = "";
    private static string env = "https://pwpawoqa3p63hwi9un57qb2wz";

    public static int m_tournamentIdNumber = 1520;
    JSONNode node;

    //All the info from the user and tournament is gotten from the URL, this is an example and can be used as a test, in prod the url is updates when webgl is loaded from the URL the iframe is being called
    private string _url = "https://tetris.monou.gg/?userId=301&tournamentId=brick-jungle-5&ambiente=https://dev-torneos-fe.monou.gg/";//510, 276

    void Start()
    {
        Debug.Log("VERSION 35");
        //Read the url to get the user id and tournament id
        _url = GetURL();
        Debug.Log("URLLLLLLLLLLLL " + _url);

        Invoke("ReadURLInfo", 1);
    }

    void ReadURLInfo()
    {
        if (_url.Length == 0)
        {
            Debug.Log("URL not read");
            return;
        }

        List<string> infoUrl = new List<string>();
        infoUrl = _url.Split('&').ToList();

        for (int i = 0; i != infoUrl.Count; i++)
        {
            Debug.Log(infoUrl[i]);
        }

        int positionInString = infoUrl[0].IndexOf('=') + 1;
        while (infoUrl[0].Length != positionInString)
        {
            m_myId += infoUrl[0][positionInString];
            positionInString++;
        }

        positionInString = infoUrl[1].IndexOf('=') + 1;
        while (infoUrl[1].Length != positionInString)
        {
            m_tournamentId += infoUrl[1][positionInString];

            positionInString++;
        }

        if (infoUrl.Count > 2)
        {
            positionInString = infoUrl[2].IndexOf('=') + 1;
            while (infoUrl[2].Length != positionInString)
            {
                m_environment += infoUrl[2][positionInString];

                positionInString++;
            }
            Debug.Log("ID:" + m_myId + " tournament:" + m_tournamentId + " environment:" + m_environment);
            switch (m_environment)
            {
                case "https://dev-torneos-fe.monou.gg/":
                    env = "https://pwpawoqa3p63hwi9un57qb2wz";
                    break;
                case "https://stg-torneos-fe.monou.gg/":
                    env = "https://e6e6j0v1xah51y9eec0p2f12h";
                    break;
                case "https://monou.gg/":
                    env = "https://dgu2evhs9qmnap4nqu9dhmcw1";
                    break;
            }
        }
        Debug.Log(env);

        StartCoroutine(GetTournamentId());
        Debug.Log(m_myId);

    }

    void CheckStatus(string json)
    {
        node = JSON.Parse(json);
        
        for (int i = 0; i < node["data"][0][0][0]["teams"].Count; i++)
        {
            //checa si esta inscrito
            if (String.Equals(node["data"][0][0][0]["teams"][i]["creador"]["id"], m_myId))
            {
                if (node["status"] == "2")
                {    // esta inscrito pero finalizo el torneo
                    _playerFound.GetComponent<TextMeshProUGUI>().text = "El torneo ha finalizado";
                    break;
                }
                else if (node["status"] == "1")
                {    // esta inscrito y listo pa jugar
                    _playerFound.GetComponent<TextMeshProUGUI>().text = "El torneo se ha verificado puedes jugar";
                    m_myTeamId = node["data"][0][0][0]["teams"][i]["id"];
                    _playButton.SetActive(true);
                    break;
                }
            }
            _playerFound.GetComponent<TextMeshProUGUI>().text = "No estas incrito al torneo";
        }
            
    }

       

    void ParseJson(string json)
    {
        node = JSON.Parse(json);

        int maxRounds = node["rounds"].Count;

        bool foundRound = false;

        for (int i = 0; i != maxRounds; i++)
        {
            for (int j = 0; j != node["data"][i/*all rounds*/]["teams"].Count; j++)
            {
                _playButton.SetActive(true);

                if ((string)node["data"][i/*all rounds*/]["teams"][j/*players in round*/]["teams"]["team_id"] == m_myId)
                {

                    if ((string)node["data"][i/*all rounds*/]["teams"][j/*players in round*/]["teams"]["place"] == null)
                    {
                        round = i + 1;
                        foundRound = true;
                        break;
                    }
                }
            }
            if (foundRound)
                break;
        }
    }

    public static string GetEnv()
    {
        return env;
    }

    IEnumerator GetRoundsInfo() 
    {
        string api = env + ".monou.gg/api/tournament/" + m_tournamentIdNumber +"/teams/?current_page=1&per_page=5";
        UnityWebRequest www = UnityWebRequest.Get(api);
            
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("get Success");
            Debug.Log(www.downloadHandler.text);
            
            ParseJson(www.downloadHandler.text);
            CheckStatus(www.downloadHandler.text);
            //Debug.Log(www.downloadHandler.text);
            //ReadJson(www.downloadHandler.text);
            /*username = _username.text;
            SceneManager.LoadScene("CustomizationScene");*/
        }
    }

    IEnumerator GetTournamentId()
    {
        string api = env + ".monou.gg/api/tournamentBySlug/" + m_tournamentId;
        UnityWebRequest www = UnityWebRequest.Get(api);


        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("get Success");
            Debug.Log(www.downloadHandler.text);
            
            JSONNode td = JSON.Parse(www.downloadHandler.text);
            m_tournamentIdNumber = td["data"][0][0][0]["tournament"]["id"];
            StartCoroutine(GetRoundsInfo());
            

            //Debug.Log(www.downloadHandler.text);
            //ReadJson(www.downloadHandler.text);
            /*username = _username.text;
            SceneManager.LoadScene("CustomizationScene");*/
        }
    }
}