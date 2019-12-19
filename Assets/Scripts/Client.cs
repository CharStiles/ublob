 using System;
 using System.Collections;
 using System.Text;
 using System.Threading;
 
 using System.Net;
 using System.Net.WebSockets;
 using System.Security.Cryptography.X509Certificates;
 
 using UnityEngine;
 using UnityEngine.Networking;
 
 public class Client : MonoBehaviour
 {
     public const string APIDomainWS = "wss://staging.projectamelia.ai/pusherman/companions/login/websocket?app=ublob";
     public const string APIDomain = "staging.projectamelia.ai";
     public const string APIUrl = "https://" + APIDomain;
 
     public static string SessionToken;
 
     public ClientWebSocket clientWebSocket;
     
     async void Start()
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
                 Debug.Log("Input message ('exit' to exit): ");
                 
                 ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                     Encoding.UTF8.GetBytes("hello fury from unity")
                 );
                 await clientWebSocket.SendAsync(
                     bytesToSend, 
                     WebSocketMessageType.Text, 
                     true, 
                     CancellationToken.None
                 );
                 ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                 WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(
                     bytesReceived, 
                     CancellationToken.None
                 );
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
     }
 }
