using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIHandler : MonoBehaviour
{
    public string userId;
    public string trackId;
    public string gameId;
    public string url;

    public GameObject humanoid;

    private void Start() => url = Application.absoluteURL;

    private void Update()
    {
        if (humanoid.GetComponent<HumanoidManager>().isGameOver())
        {
            SendRequest();
        }
    }

    public void SendRequest()
    {
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            string[] attributes = uri
                .Split('?')[1]
                .Split('&');
            userId = attributes[0].Split('=')[1];
            trackId = attributes[1].Split('=')[1];
            gameId = attributes[2].Split('=')[1];
        }
        string scoreDetails = "{" +
            "Level: " + PlayerData.level + " ," +
            "Score: " + PlayerData.score + " ," +
            "Shape: " + PlayerData.shape +
            "}";
        StartCoroutine(Login(userId, trackId, gameId, PlayerData.score + "", scoreDetails));
    }

    IEnumerator Login(string user, string track, string game, string score, string scoreDetails)
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", game);
        form.AddField("user_id", user);
        form.AddField("track_id", track);
        form.AddField("game_score", score);
        form.AddField("result_file", scoreDetails);

        string bodyData =
            "{" +
            "\"game_id\": " + game + ", " +
            "\"user_id\":" + user + ", " +
            "\"track_id\":" + track + ", " +
            "\"game_score\":" + score + ", " +
            " }";

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:8000/api/v1/savescore", bodyData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.Default.GetBytes((bodyData)));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.chunkedTransfer = false;
            yield return www.SendWebRequest();
        }
    }
}
