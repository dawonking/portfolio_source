using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using UnityEngine.Video;

public class so_thread : MonoBehaviour
{
    [DllImport("SharedObject1")]
    private static extern bool matching_input(IntPtr data, int width, int height, double[] c_point, double[] mx, double[] my, double set_angle);
    [DllImport("SharedObject1")]
    private static extern bool foo(string str);

    bool passing_check = false;


    public bool RunThread = false;
    private Color32[] pixels_crop_;
    private GCHandle pixels_handle_crop_;
    private IntPtr pixels_ptr_crop_ = IntPtr.Zero;
    float angle;
    public Texture2D tex2D;

    public double[] match_Point;
    public double[] match_x_point;
    public double[] match_y_point;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Debug.Log("Load_cutfile = " + foo(Application.persistentDataPath));
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }


        tex2D = new Texture2D(240, 240, TextureFormat.RGBA32, false);

        new Thread(StartThread).Start();

    }

    void StartThread()
    {
        try
        {
            while (true)
            {
                if (RunThread)
                {
                    try
                    {
                        try
                        {
                            pixels_crop_ = tex2D.GetPixels32();
                            pixels_handle_crop_ = GCHandle.Alloc(pixels_crop_, GCHandleType.Pinned);
                            pixels_ptr_crop_ = pixels_handle_crop_.AddrOfPinnedObject();
                        }
                        catch (ThreadAbortException e)
                        {
                            Debug.Log(e.ToString());
                        }

                        //check_crop_texture , and return matching value                        
                        if (matching_input(pixels_ptr_crop_, tex2D.width, tex2D.height, match_Point, match_x_point, match_y_point, angle))
                        {
                            passing_check = true;
                            //angle = return_angle();

                        }
                        else
                        {
                            passing_check = false;
                            Debug.Log("passing_false");
                        }
                        pixels_handle_crop_.Free();
                }
                
                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

}
