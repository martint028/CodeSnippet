using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;
using System.IO;

public class TCPTestServer {
    
    private static TCPTestServer _tcpServer;
    public static TCPTestServer TCPServer
    {
        get
        {
            if(_tcpServer == null)
            {
                _tcpServer = new TCPTestServer();
            }
            return _tcpServer;
        }
    } 
    /*
    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }       */

    public string netDataString;
   // public TextAsset ipstring;
    private string ipstr;
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	public TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient; 	
	#endregion 	
		
	// Use this for initialization
	public void Start () {
        // Start TcpServer background thread

        ipstr = LoadOutherTxtFile();
        //"127.0.0.1";

        Debug.Log(ipstr);
            //"192.168.1.151";
        //ipstring.ToString() != null ? ipstring.ToString() : "127.0.0.1";
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
	}  	

    private string LoadOutherTxtFile()
    {
        string ipstr;
        StreamReader reader = new StreamReader(Application.dataPath+"/OuterIP.txt");
            //("Assets/OuterIP.txt");
        ipstr = reader.ReadToEnd();
        reader.Close();
       
        return ipstr;
    }
	
	// Update is called once per frame
	 	
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse(ipstr), 8777); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[1024];  			
			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData);

                            netDataString = clientMessage;
							Debug.Log("client message received as: " + clientMessage); 						
						} 					
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}
    /// <summary> 	
    /// Send message to client using socket connection. 	
    /// </summary> 	
    public void SendMessage(string serverMessage)
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                
                 serverMessage = serverMessage != null ? serverMessage: "This is a message from your server.";
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }                      
         
	
}