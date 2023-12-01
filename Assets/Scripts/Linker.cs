using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class Linker : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetURL();

    public static int round;

    [SerializeField] GameObject _startGamneButton;
    [SerializeField] GameObject _playerFound;
    [SerializeField] GameObject _playButton;
    [SerializeField] TextMeshProUGUI _roundTournamentText;

    public static string m_myId = "111";
    private string m_tournamentId = "null";
    public static int m_tournamentIdNumber = 0;

    //All the info from the user and tournament is gotten from the URL, this is an example and can be used as a test, in prod the url is updates when webgl is loaded from the URL the iframe is being called
    private string _url = "https://candy.monou.gg/?userId=111&tournamentId=tewtrisrr1";//510, 276

    void Start()
    {
        Debug.Log("VERSION 35");
        //Read the url to get the user id and tournament id
        //_url = GetURL();
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

        bool isNumber = true;

        m_myId = "";
        int positionInString = _url.IndexOf('=') + 1;
        while (isNumber)
        {
            m_myId += _url[positionInString];

            positionInString++;
            if (!char.IsNumber(_url[positionInString]))
            {
                isNumber = false;
            }
        }

        positionInString = _url.LastIndexOf('=') + 1;
        m_tournamentId = "";
        while (positionInString < _url.Length)
        {
            m_tournamentId += _url[positionInString];

            positionInString++;
        }
        Debug.Log("ID:" + m_myId + " tournament:" + m_tournamentId);

        StartCoroutine(GetTournamentId());
        Debug.Log(m_myId);

    }

    void ParseJson(string json)
    {
        JSONNode node = JSON.Parse(json);

        int maxRounds = node["rounds"].Count;

        bool foundRound = false;
        for(int i = 0; i != maxRounds; i++)
        {
            for (int j = 0; j != node["data"][i/*all rounds*/]["teams"].Count; j++)
            {

                if((string)node["data"][i/*all rounds*/]["teams"][j/*players in round*/]["teams"]["team_id"] == m_myId)
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

    IEnumerator GetRoundsInfo() 
    {
        string api = "https://pwpawoqa3p63hwi9un57qb2wz.monou.gg/api/tournament/royale/rounds/" + m_tournamentIdNumber;
        //string api = "https://pwpawoqa3p63hwi9un57qb2wz.monou.gg/api/tournament/candy9/matches/";
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
            //Debug.Log(www.downloadHandler.text);
            //ReadJson(www.downloadHandler.text);
            /*username = _username.text;
            SceneManager.LoadScene("CustomizationScene");*/
        }
    }

    IEnumerator GetTournamentId()
    {
        string api = "https://pwpawoqa3p63hwi9un57qb2wz.monou.gg/api/tournamentBySlug/" + m_tournamentId;
        //string api = "https://pwpawoqa3p63hwi9un57qb2wz.monou.gg/api/tournamentBySlug/";
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