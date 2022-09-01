using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class job_info
{
    public int obj_id = 0;
    public byte[] file_byte = new byte[0];
    public string recv_name = "";
}

public class udp_server : MonoBehaviour
{

    UdpClient udpserver;
    IPAddress iPAddress;
    IPEndPoint point;
    Thread udp_thread;
    bool connet = false;
    byte[] bytes;
    byte[] temp;

    public string[] path_;
    public byte[] file_byte;


    public string name_input;
    public int pc_id;
    public int job_id;
    public int png_count;

    bool job_doing = false;

    job_info job_info_udp;

    public Queue<byte[]> wait_b = new Queue<byte[]>();

    private void Awake()
    {
        for (int i = 0; i < path_.Length; i++)
        {
            string[] files = Directory.GetFiles(path_[i]);
            foreach (string file in files)
            {
                Debug.Log(file);
                File.Delete(file);
            }
        }


        job_info_udp = new job_info();
        //server_ip
        iPAddress = IPAddress.Parse("192.168.0.6");
        point = new IPEndPoint(iPAddress, 6231);
        try
        {
            udpserver = new UdpClient(point);
        }
        catch (Exception e)
        {
            Debug.Log("udp_new_error = " + e);
        }

        connet = true;
        udp_thread = new Thread(Receive_byte);
        udp_thread.Start();
    }


    void Receive_byte()
    {
        bytes = new byte[10];

        while (connet)
        {
            try
            {

                try
                {
                    bytes = udpserver.Receive(ref point);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                if (bytes.Length != 0)
                {
                    // not working
                    if (!job_doing)
                    {
                        job_doing = true;
                        temp = new byte[bytes.Length - 3];

                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp[i] = bytes[i];
                        }
                        name_input = Encoding.Default.GetString(temp);
                        // id , job_select, png_name
                        png_count = bytes[bytes.Length - 1];
                        job_id = bytes[bytes.Length - 2];
                        pc_id = bytes[bytes.Length - 3];

                        file_byte = File.ReadAllBytes(path_[pc_id] + png_count + ".png");
                        job_info_udp.file_byte = file_byte;
                        job_info_udp.obj_id = job_id;
                        job_info_udp.recv_name = name_input;
                        Manager.manager.job_Infos.Enqueue(job_info_udp);

                        if (wait_b.Count > 0)
                        {
                            Debug.Log("wait_b_count = " + wait_b.Count);
                            try
                            {
                                for (int i = 0; i < wait_b.Count; i++)
                                {
                                    creat_obj(wait_b.Dequeue());
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                            }
                        }

                        job_doing = false;
                    }
                    //working
                    else
                    {
                        Debug.Log("job_doing");
                        wait_b.Enqueue(bytes);
                    }

                }
                else
                {

                }


            }
            catch (Exception e)
            {
                Debug.Log(e);
            }


        }
    }


    void creat_obj(byte[] in_byte)
    {
        job_doing = true;
        temp = new byte[in_byte.Length - 3];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = in_byte[i];
        }
        name_input = Encoding.Default.GetString(temp);
        // id , job_select, png_name
        png_count = in_byte[in_byte.Length - 1];
        job_id = in_byte[in_byte.Length - 2];
        pc_id = in_byte[in_byte.Length - 3];

        file_byte = File.ReadAllBytes(path_[pc_id] + png_count + ".png");
        job_info_udp.file_byte = file_byte;
        job_info_udp.obj_id = job_id;
        job_info_udp.recv_name = name_input;
        Manager.manager.job_Infos.Enqueue(job_info_udp);
    }


    private void OnApplicationQuit()
    {
        connet = false;
        udp_thread.Abort();
        udpserver.Close();
    }

}
