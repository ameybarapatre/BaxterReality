using UnityEngine;
using System.Collections;
using System;
using Windows.Kinect;
using System.Runtime.InteropServices;
using System;

public class DepthRosGeometryView : MonoBehaviour {
    private CoordinateMapper m_pCoordinateMapper;
    private MultiSourceFrameReader m_pMultiSourceFrameReader;
    private DepthSpacePoint[] m_pDepthCoordinates;

    //private WebsocketClient wsc;
    string depthTopic;
    string colorTopic;
    int framerate = 100;
    public string compression = "none"; //"png" is the other option, haven't tried it yet though
    string depthMessage;
    string colorMessage;
    private KinectManager insta;

    public Material Material;
    public static Texture2D depthTexture;
    public static Texture2D colorTexture;
  
    int width = 512;
    int height = 424;

    Matrix4x4 m;

    // Use this for initialization
    void Start() {
        // Create a texture for the depth image and color image
        depthTexture = new Texture2D(width, height, TextureFormat.R16, false);
        colorTexture = new Texture2D(2, 2);

        /*
        wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
        depthTopic = "kinect2/sd/image_depth_rect";
        colorTopic = "kinect2/sd/image_color_rect/compressed";
        wsc.Subscribe(depthTopic, "sensor_msgs/Image", compression, framerate);
        wsc.Subscribe(colorTopic, "sensor_msgs/CompressedImage", compression, framerate);*/
        InvokeRepeating("UpdateTexture", 0.1f, 0.1f);
    }

    // Update is called once per frame
    void UpdateTexture() {
        try {
            insta = KinectManager.Instance;
            ushort[] source = insta.GetRawDepthMap();
            byte[]  depthImage  = new byte[source.Length * 2]; 
            Buffer.BlockCopy(source, 0, depthImage, 0, source.Length * 2);

            depthTexture = insta.GetUsersLblTex();
            //depthTexture.LoadImage(depthImage);
            depthTexture.Apply();
            Debug.Log(depthImage.Length);

        }
        catch (Exception e) {
            Debug.Log(e.ToString());
        }

        try {
            insta = KinectManager.Instance;
            colorMessage = "hello";
            byte[] colorImage = Color32ArrayToByteArray(insta.getColor()) ;
            colorTexture = insta.GetUsersClrTex();
            colorTexture.Apply();
            Debug.Log(colorImage.Length);
        }
        catch {
            return;
        }
    }

    void OnRenderObject() {
        insta = KinectManager.Instance;
        Material.SetTexture("_MainTex", insta.GetUsersLblTex());
        //Material.SetTexture("_ColorTex", colorTexture);
        Material.SetPass(0);

           // m = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);
        //Material.SetMatrix("transformationMatrix", m);

        Graphics.DrawProcedural(MeshTopology.Points, 512 * 424, 1);
      
    }

    private static byte[] Color32ArrayToByteArray(Color32[] colors)
    {
    int length = 4 * colors.Length;
    byte[] bytes = new byte[length];
    IntPtr ptr = Marshal.AllocHGlobal(length);
    Marshal.StructureToPtr(colors, ptr, true);
    Marshal.Copy(ptr, bytes, 0, length);
    Marshal.FreeHGlobal(ptr);
    return bytes;
    }

    void onGUI(){
       
    }
    public static Texture2D sendcolor()
    { return DepthRosGeometryView.colorTexture; }
}
 