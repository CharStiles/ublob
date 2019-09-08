using System.Collections;
using System.Collections.Generic;
using CI.HttpClient;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

public class DataControllerOld: MonoBehaviour, WebSocketUnityDelegate
{
    //string cookie = "_login=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJkYXRhIjp7ImFjY291bnRfdHlwZSI6ImVtYWlsIiwiZW5kb3dtZW50c19maW5pc2hlZCI6dHJ1ZSwiZmlyc3RfbmFtZSI6IkNoYXIiLCJsYXN0X25hbWUiOiJzdGlsZXMiLCJvYXV0aC5zcG90aWZ5Ijp7InNlcnZpY2UiOiJzcG90aWZ5IiwiaWQiOiIxMjU0NTAwODg1IiwibGluayI6Imh0dHBzOi8vYXBpLnNwb3RpZnkuY29tL3YxL3VzZXJzLzEyNTQ1MDA4ODUifSwib2F1dGhfZmluaXNoZWQiOnRydWUsIm9hdXRoX21vZGFsX3NlZW4iOnRydWUsInNoYXJlIjoiMjo6YTNhNWVhZGQ0MjJkNWJhZDgzMTg2NzUwYzg0ODQwYzUiLCJ1c2VyX2NyZWF0aW9uIjoxNTYzNTQ4ODkwLjY0MzE3OSwidXNlcm5hbWUiOiJjaGFybG90dGVmc3RpbGVzQGdtYWlsLmNvbSJ9LCJleHAiOjE1NjM2NTMzODh9.Uo3Vtb58RkbpUdqFxQIPtHkirPwn97icIvJMvcBcmC6xBueJJvJFnEnlPIaT6Ebxbx5IHq9Ywivgpayn9rCWLA";
    string cookieURL = "https://staging.projectamelia.ai/pusherman/companions/login?app=ublob&token=$2b$12$KMJbcRH8fxxCjbaKo7YB1eoEHLC2S3TaxiQNEVCiYO9boj09jpBqy";
    public Renderer blobRend;
    string cookie = "token=$2b$12$KMJbcRH8fxxCjbaKo7YB1eoEHLC2S3TaxiQNEVCiYO9boj09jpBqy";
    //string cookie = "_login=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJkYXRhIjp7ImFjY291bnRfdHlwZSI6ImVtYWlsIiwiZW5kb3dtZW50c19maW5pc2hlZCI6dHJ1ZSwiZmlyc3RfbmFtZSI6IkNoYXIiLCJsYXN0X25hbWUiOiJzdGlsZXMiLCJvYXV0aC5zcG90aWZ5Ijp7InNlcnZpY2UiOiJzcG90aWZ5IiwiaWQiOiIxMjU0NTAwODg1IiwibGluayI6Imh0dHBzOi8vYXBpLnNwb3RpZnkuY29tL3YxL3VzZXJzLzEyNTQ1MDA4ODUifSwib2F1dGhfZmluaXNoZWQiOnRydWUsIm9hdXRoX21vZGFsX3NlZW4iOnRydWUsInNoYXJlIjoiMjo6YTNhNWVhZGQ0MjJkNWJhZDgzMTg2NzUwYzg0ODQwYzUiLCJ1c2VyX2NyZWF0aW9uIjoxNTYzNTQ4ODkwLjY0MzE3OSwidXNlcm5hbWUiOiJjaGFybG90dGVmc3RpbGVzQGdtYWlsLmNvbSJ9LCJleHAiOjE1NjM2NTMzODh9.Uo3Vtb58RkbpUdqFxQIPtHkirPwn97icIvJMvcBcmC6xBueJJvJFnEnlPIaT6Ebxbx5IHq9Ywivgpayn9rCWLA";
    // Start is called before the first frame update
    string url;
    string oldUrl;
    Text name;
    GameObject nameGO;

    // boolean to manage display
	private bool sendingMessage = false;
	private bool receivedMessage = false;

	
	// Web Socket for Unity
	//    - Desktop
	//    - WebPlayer
	//	  - WebGL
	//	  With the full package (an upgrade package will be soon available if you bought the Lite version)
	//    - Android
	//    - ios (+ ios simulator)
	private WebSocketUnity webSocket;
 void Start()
 {
    nameGO = GameObject.FindWithTag("Player");
    name = nameGO.GetComponent<Text>();
    
    url = "https://scontent.cdninstagram.com/vp/40eca769e5c52d4d93f81d005e6f312f/5DCAF45F/t51.2885-19/s150x150/53745151_2080173922098515_549575747384115200_n.jpg?_nc_ht=scontent.cdninstagram.com";
    oldUrl = "https://scontent.cdninstagram.com/vp/40eca769e5c52d4d93f81d005e6f312f/5DCAF45F/t51.2885-19/s150x150/53745151_2080173922098515_549575747384115200_n.jpg?_nc_ht=scontent.cdninstagram.com";
    //StartCoroutine(getPic());
	webSocket = new WebSocketUnity("wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob", this); // <= public server
    webSocket.Open();
    //callGetPic();
//      var factory = new WebSocketClientFactory();
// WebSocket webSocket = new WebSocket("wss://staging.projectamelia.ai/pusherman/companions/websocket?app=ublob"); //await factory.ConnectAsync(new System.Uri("wss://staging.projectamelia.ai/pusherman/companions/websocket?app=ublob"));
//      webSocket.onMessage += (sender, e) =>
//             Console.WriteLine ("Laputa says: " + e.Data);
//     webSocket.Connect();
 }

 void Quit(){

    webSocket.Close();

 }
 
    // void webSocketClient_OnMessage(object sender, MessageEventArgs e)
    // {
    //     lastMessage = e.Message;
    //     Debug.Log("lastMessage = " + lastMessage);
    // }

    public void Dl()
    {
        HttpClient client = new HttpClient();
        // client.Post(new System.Uri("https://staging.projectamelia.ai/pusherman/ublob?"+cookie),HttpCompletionOption.AllResponseContent, (r) =>
        // //client.Get(new System.Uri(cookieURL), HttpCompletionOption.AllResponseContent, (r) =>
        // {
        //     Debug.Log(r.ReadAsString());
        // });


        //client.Get(new System.Uri("https://staging.projectamelia.ai/pusherman/ublob?username=charlottefstiles@gmail.com&passphrase=Pup_a_pie5"), HttpCompletionOption.AllResponseContent, (r) =>
        client.Get(new System.Uri("https://staging.projectamelia.ai/pusherman/ublob?"+cookie), HttpCompletionOption.AllResponseContent, (r) =>
        //client.Get(new System.Uri(cookieURL), HttpCompletionOption.AllResponseContent, (r) =>
        {
            Debug.Log(r.ReadAsString());
        });
    }

    IEnumerator getPic()
    {
        Debug.Log("enter getpic");

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                blobRend.material.mainTexture = DownloadHandlerTexture.GetContent(uwr);
            }
        }

        // //Start a download of the given URL
        // using (WWW www = new WWW(url))
        // {
        //     // Wait for download to complete
        //   //  WWW www = new WWW(url);
        //     Debug.Log("waitng");
        //     yield return www;
            
        //     // assign texture
        //     blobRend.material.mainTexture = www.texture;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (oldUrl != url){
            StartCoroutine(getPic());
            oldUrl = url;
        }
        if (nameGO.activeSelf == false){
            nameGO.SetActive(true);
        }
        
    }
    	// Event when the connection has been opened
	public void OnWebSocketUnityOpen(string sender){}
	
	// Event when the connection has been closed
	public void OnWebSocketUnityClose(string reason){
        StartCoroutine(sleepStart());

    }

    IEnumerator sleepStart()
    {
        print(Time.time);
        yield return new WaitForSeconds(5);
        print(Time.time);
        webSocket = new WebSocketUnity("wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob", this); // <= public server
        webSocket.Open();
        Debug.Log("closed!!");
    }

	void callGetPic(){
        //url = "https://scontent.cdninstagram.com/vp/40eca769e5c52d4d93f81d005e6f312f/5DCAF45F/t51.2885-19/s150x150/53745151_2080173922098515_549575747384115200_n.jpg?_nc_ht=scontent.cdninstagram.com";
        url = "http://pbs.twimg.com/profile_images/824493306553991172/VCY4N-tr_normal.jpg";
        StartCoroutine(getPic());
    }



	// Event when the websocket receive a message
    //public void OnWebSocketUnityReceiveMessageOnMobile(string message){}
    public void OnWebSocketUnityReceiveDataOnMobile(string message){

        Debug.Log("Received this from server : " + message);

        //string[] stringSeparators = new string[] {'\"'};
        //callGetPic();

        string[] result = message.Split('\"');
        for (int i = 0 ; i < result.Length; i ++){
            if (result[i] == "first_name"){
                Debug.Log("GOT NAME");
                name.text = "Hello" + result[i+2];
                nameGO.SetActive(false);
                
            }
            if (result[i] == "picture"){
                url = result[i+2];
                //url = "https://scontent.cdninstagram.com/vp/40eca769e5c52d4d93f81d005e6f312f/5DCAF45F/t51.2885-19/s150x150/53745151_2080173922098515_549575747384115200_n.jpg?_nc_ht=scontent.cdninstagram.com";
                //StartCoroutine(getPic());
                        //callGetPic();

                // Debug.Log("hm!");
                //break;
                // StartCoroutine("getPic");
            }
            
        }

    }

	public void OnWebSocketUnityReceiveMessage(string message){
        Debug.Log("Received this from server : " + message);

        //string[] stringSeparators = new string[] {'\"'};
        //callGetPic();

        string[] result = message.Split('\"');
        for (int i = 0 ; i < result.Length; i ++){
            if (result[i] == "first_name"){
                Debug.Log("GOT NAME");
                name.text = "Hello " + result[i+2];
            }
            if (result[i] == "picture"){
                // Debug.Log("pic url: ");
                // Debug.Log(result[i+2]);
                url = result[i+2];
                //url = "https://scontent.cdninstagram.com/vp/40eca769e5c52d4d93f81d005e6f312f/5DCAF45F/t51.2885-19/s150x150/53745151_2080173922098515_549575747384115200_n.jpg?_nc_ht=scontent.cdninstagram.com";
                //StartCoroutine(getPic());
                        //callGetPic();

                // Debug.Log("hm!");
                //break;
                // StartCoroutine("getPic");
            }
        }
		if(!sendingMessage)
			return;
		
		sendingMessage = false;
		receivedMessage = true;

    }
	
	// Event when the websocket receive data
	public void OnWebSocketUnityReceiveData(byte[] data){

    }
	
	// Event when an error occurs
	public void OnWebSocketUnityError(string error){


    }
}
