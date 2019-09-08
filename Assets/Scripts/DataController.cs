 using System;
 using System.Collections;
 using System.Collections.Generic;
 using System.Text;
 using System.Threading;
 
 using System.Net;
 using System.Security.Cryptography.X509Certificates;
 
using CI.HttpClient;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks; 

//part of this code is from https://answers.unity.com/questions/1475622/systemnetwebsocketsclientwebsocket-fails-at-connec.html
// https://fogbugz.unity3d.com/default.asp?1082184_b1jgq2gplqp9fnja

public class DataController: MonoBehaviour
{

    string url;
    string oldUrl;
    Text name;
    GameObject nameGO;
    public Button welcomeButton;
    public GameObject[] blobs;
    public const string APIDomainWS = "wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob";
     public const string APIDomain = "staging.projectamelia.ai";
     public const string APIUrl = "https://" + APIDomain;
     Renderer blobRend;
    string usr;
     public static string SessionToken;
 
     public ClientWebSocket clientWebSocket;
     Controller controller;
     float lastTimeReceive;

    IEnumerator bePatientSetupClient(){

        yield return new WaitForSeconds(1);
        SetupClient();
    }

     async void SetupClient()
     {
         DontDestroyOnLoad(gameObject);
         
         clientWebSocket = new ClientWebSocket();
 
         clientWebSocket.Options.AddSubProtocol("Tls");
         
         Debug.Log("[WS]:Attempting connection.");
         try
         {
             Uri uri = new Uri(APIDomainWS);
             await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
             if (clientWebSocket.State == WebSocketState.Open)
             {
                //  Debug.Log("Input message ('exit' to exit): ");
                 
                //  ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                //      Encoding.UTF8.GetBytes("hello fury from unity")
                //  );
                //  await clientWebSocket.SendAsync(
                //      bytesToSend, 
                //      WebSocketMessageType.Text, 
                //      true, 
                //      CancellationToken.None
                //  );
                 ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                 WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(
                     bytesReceived, 
                     CancellationToken.None
                 );
                 Debug.Log("ABOUT TO SEND TO RECIEVE");
                 ReceiveMessage(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
                 Debug.Log(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
             }
             Debug.Log("[WS][connect]:" + "Connected");
         }
         catch (Exception e)
         {
             Debug.Log("[WS][exception]:" + e.Message);
             if (e.InnerException != null)
             {
                 Debug.Log("[WS][inner exception]:" + e.InnerException.Message);
             }
         }
         
         Debug.Log("End");

        StartCoroutine(bePatientSetupClient());
     }
 void Start()
 {
     GameObject go = GameObject.FindWithTag("GameController");

    controller = go.GetComponent<Controller>();
    nameGO = GameObject.FindWithTag("Player");
    name = nameGO.GetComponent<Text>();
    SetupClient();
    url = "";
    usr = "";
    oldUrl = "";
    lastTimeReceive = 0f;
 }

 void Quit(){

    //webSocket.Close();

 }
 
 
    IEnumerator getPic()
    {
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
	void OnWebSocketUnityOpen(string sender){}
	
	// Event when the connection has been closed
	// void OnWebSocketUnityClose(string reason){
    //     StartCoroutine(sleepStart());

    // }

    // IEnumerator sleepStart()
    // {
    //     print(Time.time);
    //     yield return new WaitForSeconds(5);
    //     print(Time.time);
    //     webSocket = new WebSocketUnity("wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob", this); // <= public server
    //     webSocket.Open();
    //     Debug.Log("closed!!");
    // }

	void callGetPic(){
        //url = "https://scontent.cdninstagram.com/vp/40eca769e5c52d4d93f81d005e6f312f/5DCAF45F/t51.2885-19/s150x150/53745151_2080173922098515_549575747384115200_n.jpg?_nc_ht=scontent.cdninstagram.com";
        url = "http://pbs.twimg.com/profile_images/824493306553991172/VCY4N-tr_normal.jpg";
        StartCoroutine(getPic());
    }


	public void ReceiveMessage(string message){
        
        Debug.Log("Received this from server : " + message);
        welcomeButton.onClick.Invoke();
        //string[] stringSeparators = new string[] {'\"'};
        //callGetPic();
        controller.lastNext = Time.time;
        string[] result = message.Split('\"');
        bool gotName = false;
        bool gotUesrName = false;
        for (int i = 0 ; i < blobs.Length ; i++){

                blobs[i].SetActive(false);
            
        }
        for (int i = 0 ; i < result.Length; i ++){
            if (result[i] == "first_name" && gotName == false){
                Debug.Log("GOT NAME");
                gotName = true;
                name.text = "Hello " + result[i+2];
                GameObject b = blobs[ (result[i+2].Length ) % blobs.Length ];
                b.SetActive(true);
                blobRend = b.GetComponent<Renderer>();  
            }
            if (result[i] == "username" && gotUesrName == false && (Time.time - lastTimeReceive > 5)){
                gotUesrName = true;
 
                if (usr == result[i+2])
                {
                    controller.LogOut();
                    Debug.Log("loggin out");
                }

                usr = result[i+2];
            }

            if (result[i] == "picture"){
            
                url = result[i+2];
           
            }
        }
    lastTimeReceive = Time.time;

    }
	
	// Event when the websocket receive data
	public void OnWebSocketUnityReceiveData(byte[] data){

    }
	
	// Event when an error occurs
	public void OnWebSocketUnityError(string error){


    }
}
