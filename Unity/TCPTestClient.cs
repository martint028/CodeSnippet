using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;

public class TCPTestClient {  	
	#region private members 	
	public TcpClient socketConnection; 	
	private Thread clientReceiveThread;
    public PlayerStatus[] sharedPlayers;
	#endregion
    private static TCPTestClient _tcpClient;
    public static TCPTestClient TcpClient
    {
        get
        {
            if(_tcpClient == null)
            {
                _tcpClient = new TCPTestClient();
            }
            return _tcpClient;
        }
    }


    // Use this for initialization 	
    public void Start () {
        ipStr = LoadOutherTxtFile();
        //"127.0.0.1";
        ConnectToTcpServer();
    }
    
    public void Disconnect()
    {
        if(socketConnection != null)
        {
            socketConnection.Close();
        }
        
    }
    // Update is called once per frame
    void Update () {         
		if (Input.GetKeyDown(KeyCode.Space)) {             
			//SendMessage();         
		}     
	}  	
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}

    private string ipStr;

    private string LoadOutherTxtFile()
    {
        string ipstr;
        StreamReader reader = new StreamReader(Application.dataPath + "/OuterIP.txt");
        ipstr = reader.ReadToEnd();
        reader.Close();

        return ipstr;
    }
    public string getServerMessage = null;
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try
        {           //192.168.1.151
            socketConnection = new TcpClient(ipStr, 8777);  			
			Byte[] bytes = new Byte[1024];             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
						var incommingData = new byte[length]; 						
						Array.Copy(bytes, 0, incommingData, 0, length); 						
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
                       getServerMessage =  serverMessage;
						Debug.Log("server message received as: " + serverMessage); 					
					} 				
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public void SendMessage(string clientMessage) {         
		if (socketConnection == null) {             
			return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				//string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	} 
}