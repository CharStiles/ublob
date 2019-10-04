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
//using UnityEngine.SceneManagement;
using TMPro; // Add the TextMesh Pro namespace to access the various functions.
 
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
    public RectTransform symptomPos;
    
    // also in getpost
    public const string APIDomainWS = "wss://projectamelia.ai/pusherman/companions/login/websocket?app=ublob";
    //public const string APIDomainWS = "wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob";

     public const string APIDomain = "projectamelia.ai";
     //public const string APIDomain = "staging.projectamelia.ai";
     public const string APIUrl = "https://" + APIDomain;
    
    public TextMeshProUGUI diagnosisText;
    //Text diagnosisText;
    string diagnosisString = "\n</size=85%>\n<size=75%>If you haven't connected your social media accounts to Aura, this also could be the problem.\n\n*none of these statements have been evaluated by the SMA. This app is not intended to diagnose, treat, cure, or prevent any disease.”</size=75%>\n\n\n\n\n\n\n\n\n\n\n\n\n";
    string magenta = "\n\n</size=75%><size=85%>Magenta uBlob</size=85%>\n<size=75%>Having a magenta or pink uBlob may mean you haven\'t been consuming enough popular/verified content. This is easily solved by following a few verified accounts with more than 500k followers.</size=75%><size=75%>It also may simply mean you have been posting a lot of magenta pictures or interacting with magenta content.</size=75%><size=75%>";
    string slow = "\n\n</size=75%><size=85%>Loose, slow movements, or no movement</size=85%>\n<size=75%>Your uBlob shouldn't be still, this indicates some underlying stress that you may have towards your social media feed. This can be tough to figure out what is causing the source, try unfollowing people who make you angry or post negative content. Slow movement can be solved, but if your uBlob is completely still please mention it to a certified uBlob therapist. </size=75%><size=75%>";
    string small = "\n\n</size=75%><size=85%>Whispy-like uBlob</size=85%>\n<size=75%>You have been consuming or participating in microblogging too much, try adding some lengthier content to your social media consumption. Try spending more time reading email or reading your friends long Facebook post instead of Instagram or Twitter.</size=75%><size=75%>";
    string pale = "\n\n</size=75%><size=85%>Pale uBlob</size=85%>\n<size=75%>Watching videos or looking at images taken of real life gives uBlob its characteristic textured and colorful look, a uBlob that is light-colored (either pale, white, grey, or clay-colored) could indicate a lack of colorful images in your social media browsing, This is common for those who look at too many similar memes, text based content, or those pictures of text. </size=75%><size=75%>";
    string beginning = "<size=100%>My uBlob Diagnosis*:</size=100%><size=75%>\n";
    string token;
     Renderer blobRend;
    string usr;
     public static string SessionToken;
 
     public ClientWebSocket clientWebSocket;
     Controller controller;
     float lastTimeReceive;
     float paleToShader ;
                float yellowToShader;
    public Texture2D defaultTexture;

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
        client.Get(new System.Uri("https://projectamelia.ai/pusherman/mood_companion?token="+token), HttpCompletionOption.AllResponseContent, (r) =>
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
 
 
    IEnumerator getPic(bool tryAgain = true)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                if (tryAgain){
                    url = url + "?token="+token;
                    yield return (getPic(false));
                }
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
            blobRend.material.mainTexture = defaultTexture;
        }
        symptomPos.localPosition = new Vector3(0f,-461f,0f);

        diagnosisString = "\n</size=85%>\n<size=75%>If you haven't connected your social media accounts to Aura, this also could be the problem.\n\n*none of these statements have been evaluated by the SMA. This app is not intended to diagnose, treat, cure, or prevent any disease.”</size=75%>\n\n\n\n\n\n\n\n\n\n\n\n\n";
        string startDiagnosisString = diagnosisString;

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
                float speedToShader = ((((float)(message.Length))/3500.0f));
                if (((((float)(message.Length))/3000.0f)) < 0.1){
                    speedToShader = 0;
                }
                blobRend.material.SetFloat("_Speed", speedToShader);
                if (speedToShader < 0.4){
                    diagnosisString = slow+diagnosisString;
                    
                }
            }
            if (result[i] == "username" && gotUesrName == false && (Time.time - lastTimeReceive > 2)){
                gotUesrName = true;
                float sizeToShader = (((float)(result[i+2].Length))/13.0f) % 1.0f;
                blobRend.material.SetFloat("_Size", sizeToShader);
                
                if (sizeToShader < 0.5f){
                    diagnosisString = small+diagnosisString;
                }
                paleToShader = (((float)(result[i+2].Length))/23.111f) % 1.0f;
                yellowToShader = (((float)(result[i+2].Length))/3.1415f) % 1.0f;

               
                if (((((float)(message.Length))/3000.0f)) < 0.2){
                    yellowToShader = 0;
                    paleToShader = 1;
                }

                blobRend.material.SetFloat("_Pale", paleToShader);
                blobRend.material.SetFloat("_Yellow", yellowToShader);
                if (paleToShader > 0.5f){
                    diagnosisString = pale+diagnosisString;
                }
                if (yellowToShader < 0.5f){
                    diagnosisString = magenta+diagnosisString;
                }

                if (usr == result[i+2])
                {
                    url = "";
                    usr = "";
                    oldUrl = "";
                    token = "";
                    //SceneManager.LoadScene("rayMarched");
                    Debug.Log("loggin out");
                }
                else{
                    welcomeButton.onClick.Invoke();
                    usr = result[i+2];
                }

            }

            if (result[i] == "picture"){
            
                url = result[i+2];


            }
            if (result[i] == "token"){
            
                token = result[i+2];
                

            }


        }

    if(diagnosisString == startDiagnosisString){
        blobRend.material.SetFloat("_Speed", 0.2f);
        diagnosisString = slow+diagnosisString;
    }
    lastTimeReceive = Time.time;
    diagnosisText.SetText(beginning + diagnosisString);
    }
    
	// Event when the websocket receive data
	public void OnWebSocketUnityReceiveData(byte[] data){

    }
	
	// Event when an error occurs
	public void OnWebSocketUnityError(string error){


    }
}
