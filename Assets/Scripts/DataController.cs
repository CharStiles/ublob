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
    public GameObject blob;
    public const string APIDomainWS = "wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob";
     public const string APIDomain = "staging.projectamelia.ai";
     public const string APIUrl = "https://" + APIDomain;
    string token;
     Renderer blobRend;
    public string usr;
     public static string SessionToken;
 
     public ClientWebSocket clientWebSocket;
     Controller controller;
     float lastTimeReceive;

    IEnumerator bePatientSetupClient(){

        yield return new WaitForSeconds(0.5f);
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
             while (clientWebSocket.State == WebSocketState.Open)
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
                  //GetPosts();
                 //Debug.Log(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
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

     public void GetPosts()
     {
        HttpClient client = new HttpClient();
        // client.Post(new System.Uri("https://staging.projectamelia.ai/pusherman/ublob?"+cookie),HttpCompletionOption.AllResponseContent, (r) =>
        // //client.Get(new System.Uri(cookieURL), HttpCompletionOption.AllResponseContent, (r) =>
        // {
        //     Debug.Log(r.ReadAsString());
        // });


        //client.Get(new System.Uri("https://staging.projectamelia.ai/pusherman/ublob?username=charlottefstiles@gmail.com&passphrase=Pup_a_pie5"), HttpCompletionOption.AllResponseContent, (r) =>
        client.Get(new System.Uri("https://staging.projectamelia.ai/pusherman/mood_companion?token="+token), HttpCompletionOption.AllResponseContent, (r) =>
        //client.Get(new System.Uri(cookieURL), HttpCompletionOption.AllResponseContent, (r) =>
        {
            Debug.Log(r.ReadAsString());
        });
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
    token = "";
    lastTimeReceive = 0f;
    blobRend = blob.GetComponent<Renderer>(); 
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
        if (Mathf.Abs(Time.time - controller.lastNext) > controller.timeToLogOut){ // left the screen for 1 min
            usr = "";
            controller.LogOut();

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



	public void ReceiveMessage(string message){
        
        Debug.Log("Received this from server : " + message);
        if (Time.time - lastTimeReceive > 1){
            controller.LogOut();
        }
        
        controller.lastNext = Time.time;
        string[] result = message.Split('\"');
        bool gotName = false;
        bool gotUesrName = false;

        for (int i = 0 ; i < result.Length; i ++){
            if (result[i] == "first_name" && gotName == false){
                Debug.Log("GOT NAME");
                gotName = true;
                name.text = "Hello " + result[i+2];
                // GameObject b = blobs[ (result[i+2].Length ) % blobs.Length ];
                // b.SetActive(true);
                //blobRend = b.GetComponent<Renderer>();  
                blobRend.material.SetFloat("_Speed", ((((float)(result[i+2].Length))/10.0f) % 1.0f) + 0.001f);

            }
            if (result[i] == "username" && gotUesrName == false && (Time.time - lastTimeReceive > 2)){
                gotUesrName = true;
                blobRend.material.SetFloat("_Size", (((float)(result[i+2].Length))/12.0f) % 1.0f);

                if (usr == result[i+2])
                {
                    url = "";
                    usr = "";
                    oldUrl = "";
                    token = "";
                    Debug.Log("loggin out");
                }
                else{
                    welcomeButton.onClick.Invoke();
                    usr = result[i+2];
                }

                
            }

            if (result[i] == "picture"){
            
                url = result[i+2];
                blobRend.material.SetFloat("_Pale", (((float)(result[i+2].Length))/24.0f) % 1.0f);
                blobRend.material.SetFloat("_Yellow", (((float)(result[i+2].Length))/33.0f) % 1.0f);

            }
            if (result[i] == "token"){
            
                token = result[i+2];

           
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
