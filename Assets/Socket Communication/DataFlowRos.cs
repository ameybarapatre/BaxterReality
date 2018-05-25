using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;


public class DataFlowRos : MonoBehaviour {
    System.Net.Sockets.TcpClient clientSocket;
    NetworkStream serverStream;
    GameObject camerarig , left , right;
    public GameObject TRACKER1 , TRACKER2 , HEAD , LEFT, RIGHT;
    string s;
    // Use this for initialization
    void Start () {

        camerarig = GameObject.Find("[CameraRig]");
    




    }
	
	// Update is called once per frame
	void Update () {
        //child 0  - left
        //child 1 -  right
        //child 3 - head
        NewBehaviourScript l = camerarig.transform.GetChild(0).GetComponent<NewBehaviourScript>();
        NewBehaviourScript r = camerarig.transform.GetChild(1).GetComponent<NewBehaviourScript>();
        if (camerarig != null)
        {
            s = LEFT.transform.position.ToString("F4") + "," + LEFT.transform.rotation.ToString("F4") + ":" +
                RIGHT.transform.position.ToString("F4") + "," + RIGHT.transform.rotation.ToString("F4") + ":" +
                HEAD.transform.position.ToString("F4") + "," + HEAD.transform.rotation.ToString("F4") + ":" + (l.getbuttonpress() ? 1 : 0).ToString() +":" + (r.getbuttonpress() ? 1 : 0).ToString() + ":" +
                TRACKER1.transform.position.ToString("F4") + "," + TRACKER1.transform.rotation.ToString("F4") + ":" +
                TRACKER2.transform.position.ToString("F4") + "," + TRACKER2.transform.rotation.ToString("F4");
        clientSocket = new System.Net.Sockets.TcpClient();
        clientSocket.Connect("10.8.48.217", 8080);
        serverStream = clientSocket.GetStream();

        byte[] outStream = System.Text.Encoding.ASCII.GetBytes(s);
        serverStream.Write(outStream, 0, outStream.Length);
   
        serverStream.Flush();

        }
        else { Debug.Log("NULL! Check if devices are connected!"); }

    }
}
