using UnityEngine;
using System.Collections;
using System;
using Windows.Kinect;
using System.Runtime.InteropServices;
using System;


public class DepthView : MonoBehaviour
{
 
    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;
    private int[] _Indices;
    private Color[] _Colors;

    // Only works at 4 right now
    private const int _DownsampleSize = 4 ;
    private const double _DepthScale = 0.01f;
    private const int _Speed = 50;
    private KinectManager insta;
    public Boolean ShowTrueColors = true;
    void Start()
    {
        // Downsample to lower resolution
            CreateMesh(KinectWrapper.GetDepthWidth() / _DownsampleSize, KinectWrapper.GetDepthHeight() / _DownsampleSize);

       
    }

    void CreateMesh(int width, int height)
    {
        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh;
        _Colors = new Color[width * height];
        _Vertices = new Vector3[width * height];
        _UV = new Vector2[width * height];
        _Triangles = new int[6 * ((width - 1) * (height - 1))];
        _Indices = new int[width * height];
        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;
                _Indices[index] = index;
                _Vertices[index] = new Vector3(-x, -y, 0);
                _UV[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));
                _Colors[index] = new Color(255f, 255f, 255f, 0.0f);
                // Skip the last row/col
                if (x != (width - 1) && y != (height - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + width;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        //_Mesh.triangles = _Triangles;
       // _Mesh.RecalculateNormals();

        _Mesh.SetIndices(_Indices, MeshTopology.Points, 0);
        _Mesh.colors = _Colors;
    }

   

    void Update()
    {
            insta = KinectManager.Instance;
            //gameObject.GetComponent<Renderer>().material.mainTexture = insta.GetUsersClrTex();
            RefreshData(insta.GetRawDepthMap(),insta.colorsize()[0], insta.colorsize()[1]);
       
         
    }

    private void RefreshData(ushort[] depthData, int colorWidth, int colorHeight)
    {
        insta = KinectManager.Instance;

        
        for (int y = 0; y < KinectWrapper.Constants.DepthImageHeight; y += _DownsampleSize)
        {
            for (int x = 0; x < KinectWrapper.Constants.DepthImageWidth; x += _DownsampleSize)
            {
                int indexX = x / _DownsampleSize;
                int indexY = y / _DownsampleSize;
                int smallIndex = (indexY * (KinectWrapper.GetDepthWidth() / _DownsampleSize)) + indexX;

                double avg = GetAvg(depthData, x, y, KinectWrapper.GetDepthWidth(), KinectWrapper.GetDepthHeight());

                avg = avg * _DepthScale;
                
                _Vertices[smallIndex].z = (float)avg;

                // Update UV mapping with CDRP
                Vector2 colorSpacePoint = insta.GetColorMapPosForDepthPos(new Vector2(x,y));
                Color32[] a = insta.getColor();
                
                if ((float)avg < 45.0f)
                {
                    
                    _Colors[smallIndex] = new Color(0f, 0f, 0f, 0f);
                }
                else if (!ShowTrueColors)
                {
                  

                    _Colors[smallIndex] = new Color(255f, 255f, 255f, 1.0f);

                }
                else if (ShowTrueColors)
                {
                    _Colors[smallIndex] = a[(int)(colorSpacePoint.y * KinectWrapper.Constants.ColorImageWidth + colorSpacePoint.x)];

                }

                _UV[smallIndex] = new Vector2(colorSpacePoint.x / colorWidth, colorSpacePoint.y / colorHeight);
            }
        }

       _Mesh.vertices = _Vertices;
       _Mesh.uv = _UV;
       _Mesh.colors = _Colors;
      
        //_Mesh.triangles = _Triangles;
        //_Mesh.RecalculateNormals();
    }

    private double GetAvg(ushort[] depthData, int x, int y, int width, int height)
    {
        double sum = 0.0;

        for (int y1 = y; y1 < y + 4; y1++)
        {
            for (int x1 = x; x1 < x + 4; x1++)
            {
                int fullIndex = (y1 * width) + x1;

                if (depthData[fullIndex] == 0)
                    sum += 4500;
                else
                    sum += depthData[fullIndex];

            }
        }

        return sum / 16;
    }

    void OnApplicationQuit()
    {
    }
}
