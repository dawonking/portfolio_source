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

public class udp_client : MonoBehaviour
{
    UdpClient cli;
    public byte id_b = 0;
    public byte draw_num = 0;

    public byte[] datagram = new byte[3];

    public List<byte> data_list = new List<byte>();

    public result_page result_;

    byte[] temp;

    // Start is called before the first frame update
    void Start()
    {
        cli = new UdpClient();

    }

    // Update is called once per frame
    void Update()
    {
        //test_btn
        if (Input.GetKeyDown(KeyCode.A))
        {            
            temp = Encoding.UTF8.GetBytes(Manager.manager.name_val);
            data_list.Clear();
            for(int i =0; i<temp.Length; i++)
            {
                data_list.Add(temp[i]);
            }
            
            data_list.Add(id_b);
            data_list.Add(Convert.ToByte(Manager.manager.select_job));
            data_list.Add(Convert.ToByte(result_.send_count));

            temp = new byte[data_list.Count];

            for(int i =0; i<temp.Length; i++)
            {
                temp[i] = data_list[i];
            }

            cli.Send(temp, temp.Length, "192.168.0.101", 6231);

            string str = Encoding.Default.GetString(temp);
            Debug.Log(str);

        }
    }


    public void send_result()
    {
        //add_user_text
        temp = Encoding.UTF8.GetBytes(Manager.manager.name_val);
        data_list.Clear();
        for (int i = 0; i < temp.Length; i++)
        {
            data_list.Add(temp[i]);
        }

        //pc_num
        data_list.Add(id_b);
        //user_select_job
        data_list.Add(Convert.ToByte(Manager.manager.select_job));
        //add_img_num
        data_list.Add(Convert.ToByte(result_.send_count));

        temp = new byte[data_list.Count];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = data_list[i];
        }

        cli.Send(temp, temp.Length, "192.168.0.101", 6231);
        result_.send_count++;
    }

}
