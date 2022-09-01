using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class job_info
{
    public int obj_id = 0;
    public byte[] file_byte = new byte[0];
    public string recv_name = "";
}

public class rect_zone
{
    public Vector2 Left_up = new Vector2();
    public Vector2 Rigth_up = new Vector2();
}

public class json_val
{
    public int zone_count;
    public Vector2[] save_val;
}


public class Manager : MonoBehaviour
{
    public static Manager manager;

    public Camera touch_camera;

    public GameObject Center_pos;

    public GameObject[] job_obj;
    public GameObject[] add_obj;
    public Material[] job_mat;

    public Material[] job_bubble_mat;

    //cha_list
    List<GameObject> job_1 = new List<GameObject>();
    List<GameObject> job_2 = new List<GameObject>();
    List<GameObject> job_3 = new List<GameObject>();
    List<GameObject> job_4 = new List<GameObject>();
    List<GameObject> job_5 = new List<GameObject>();

    //queue_input_cha
    public Queue<job_info> job_Infos = new Queue<job_info>();

    job_info job_;


    public List<GameObject> bubble_in_game = new List<GameObject>();

    //0-right , 1-left , 2-up, 3- down
    public float[] move_range;

    public bool[] enter_obj;

    bool creat_check;
    bool last_input_check;
    public bool view_check;
    int last_play = 0;

    public UDPReceiver receiver = null;

    public AudioClip[] clips_ran;
    public AudioSource audioSource;

    public Vector2[] out_zone;

    private void OnEnable()
    {
        if(Manager.manager == null)
        {
            Manager.manager = this;
        }        

    }
    
    public float bubble_deadtime = 1800;

    int add_count = 0;

    public int zone_count;

    public rect_zone[] zone_set;

    public Vector2[] test_v;

    public GameObject[] show_panel;



    // Start is called before the first frame update
    void Start()
    {
        //Laidor
        receiver.onValueChanged += Laidor_touch;

        last_play = -1;

        // load_not_touch_zone
        Load_zone();        

        

    }

    

    // Update is called once per frame
    void Update()
    {               
        
        if(job_Infos.Count > 0 && !creat_check)
        {
            creat_check = true;
            Initiate_che_list();
        }

        run_obj();


        touch_input();      

        if (Input.GetKeyDown(KeyCode.Q))
        {
            view_check = !view_check;
        }        

    }

    

    //instanitate_cha
    void Initiate_che_list()
    {
        Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);

        job_ = job_Infos.Dequeue();


        tex.LoadImage(job_.file_byte);
        tex.Apply();

        GameObject instance_obj_1 = null;
        //GameObject pos_set_obj = null;
        //GameObject dummy = null;
        Material instance_mat = null;
        Material bubble_instance_mat = null;

        instance_obj_1 = Instantiate(job_obj[job_.obj_id]);
        instance_mat = Instantiate(job_mat[job_.obj_id]);
        instance_mat.mainTexture = tex;
        bubble_instance_mat = Instantiate(job_bubble_mat[job_.obj_id]);
        bubble_instance_mat.mainTexture = job_bubble_mat[job_.obj_id].mainTexture;
        instance_obj_1.GetComponent<cha_info>().instanse_bubble_mat = bubble_instance_mat;
        instance_obj_1.GetComponent<cha_info>().name_str = job_.recv_name;
        instance_obj_1.GetComponent<cha_info>().id_num = add_count;

        renchange_obj(instance_obj_1, instance_mat, 0);

        switch (job_.obj_id)
        {
            case 0:
                //renchange_obj(add_obj[0], instance_mat, 0);
                instance_obj_1.GetComponent<cha_info>().move_obj_mat = instance_mat;
                job_1.Add(instance_obj_1);
                break;
            case 1:
                job_2.Add(instance_obj_1);
                break;
            case 2:
                job_3.Add(instance_obj_1);
                break;
            case 3:
                job_4.Add(instance_obj_1);
                break;
            case 4:
                //renchange_obj(add_obj[1], instance_mat, 0);
                instance_obj_1.GetComponent<cha_info>().move_obj_mat = instance_mat;
                job_5.Add(instance_obj_1);
                break;
        }

        bubble_in_game.Add(instance_obj_1);

        //in game bubble_check
        if(bubble_in_game.Count > 10)
        {
            GameObject temp = bubble_in_game[0];
            bubble_in_game[0].GetComponent<cha_info>().touch_event();
            bubble_in_game.Remove(bubble_in_game[0]);
        }


        last_input_check = true;

        //StartCoroutine(delay_music());

        if (!audioSource.isPlaying)
        {
            audio_check();
        }
        else
        {
            Debug.Log("audio_playing");
        }
        creat_check = false;

        add_count++;
    }
    

    // change_object_all material
    public void renchange_obj(GameObject change_game, Material tex_mat, int num)
    {
        Renderer[] children;
        children = change_game.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag != "Particle_Obj")
            {
                children[i].material = tex_mat;
            }
            else
            {

            }
        }
    }

    void run_obj()
    {
        if(job_1.Count > 0 && !enter_obj[0])
        {
            enter_obj[0] = true;
            job_1[0].GetComponent<cha_info>().bg_.input_mat = job_1[0].GetComponent<cha_info>().move_obj_mat;
            job_1[0].SetActive(true);            
            job_1.RemoveAt(0);
        }
        if (job_2.Count > 0 && !enter_obj[1])
        {
            enter_obj[1] = true;
            job_2[0].SetActive(true);
            job_2.RemoveAt(0);
        }
        if (job_3.Count > 0 && !enter_obj[2])
        {
            enter_obj[2] = true;
            job_3[0].SetActive(true);
            job_3.RemoveAt(0);
        }

        if (job_4.Count > 0 && !enter_obj[3])
        {
            enter_obj[3] = true;
            job_4[0].SetActive(true);
            job_4.RemoveAt(0);
        }

        if (job_5.Count > 0 && !enter_obj[4])
        {
            enter_obj[4] = true;
            job_5[0].GetComponent<cha_info>().bg_.input_mat = job_5[0].GetComponent<cha_info>().move_obj_mat;
            job_5[0].SetActive(true);
            job_5.RemoveAt(0);
        }


    }

    //mouse_touch
    void touch_input()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;

            Ray ray = touch_camera.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit);

            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 5f);

            //if(ray.origin.x >= -2f && ray.origin.x <= 2f && ray.origin.y >= -2f && ray.origin.y <= -1.08f)
            //{

            //    Debug.Log(ray.origin.x + " / " + ray.origin.y + " / " + ray.origin.z);
            //}            

            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.gameObject.name);               

                if (out_zone_check(ray.origin))
                {
                    switch (hit.collider.tag)
                    {
                        case "touch_obj":
                            if (hit.collider.gameObject.GetComponent<cha_info>() != null)
                            {
                                hit.collider.gameObject.GetComponent<cha_info>().touch_event();                                
                            }
                            break;
                    }
                }                
            }
        }
    }
    
    //라이다 터치 
    void Laidor_touch(Vector3[] vecs)
    {
        for(int i =0; i < vecs.Length; i++)
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(vecs[i]);

            Physics.Raycast(ray, out hit);

            if (view_check)
            {
                show_panel[i].transform.position = new Vector3(ray.origin.x, ray.origin.y, -16.5f);
                //show_panel[i].transform.position = ray.origin;
            }

            if (hit.collider != null)
            {
                if (out_zone_check(ray.origin))
                {
                    switch (hit.collider.tag)
                    {
                        case "touch_obj":
                            if (hit.collider.gameObject.GetComponent<cha_info>() != null)
                            {
                                hit.collider.gameObject.GetComponent<cha_info>().touch_event();
                            }
                            break;
                    }
                }                
            }
        }        
    }

    bool out_zone_check(Vector2 input)
    {
        for(int i = 0; i< zone_set.Length; i++)
        {
            //if (ray.origin.x >= -2f && ray.origin.x <= 2f && ray.origin.y >= -2f && ray.origin.y <= -1.08f)
            if (input.x >=  zone_set[i].Left_up.x && input.x <= zone_set[i].Rigth_up.x && input.y >= zone_set[i].Rigth_up.y && input.y <= zone_set[i].Left_up.y)
            {                
                return false;                 
            }            
        }
        return true;

    }

    void audioFinished()
    {
        Debug.Log("audio_Finish");

        if (last_input_check)
        {
            Debug.Log("last_input");
            audio_check();
        }
        else
        {
            Debug.Log("Not_input_Music_finish");

        }

    }

    void audio_check()
    {
        last_input_check = false;
        int num;
        while (true)
        {
            num = UnityEngine.Random.Range(0, 3);
            if (num != last_play)
            {
                break;
            }
            else
            {
                num = UnityEngine.Random.Range(0, 3);
            }
        }

        last_play = num;
        audioSource.clip = clips_ran[num];
        audioSource.PlayOneShot(clips_ran[num]);
        
        Invoke("audioFinished", audioSource.clip.length);        
    }


    void Save_zone()
    {
        var setzone = new json_val();
        setzone.zone_count = zone_count;
        setzone.save_val = new Vector2[setzone.zone_count * 2];

        for(int i = 0; i<test_v.Length; i++)
        {
            setzone.save_val[i] = test_v[i];
        }

        string toJson = JsonUtility.ToJson(setzone);
        File.WriteAllText(Application.dataPath + "/zone.json", toJson);

        Debug.Log(this.gameObject.name + "_Save");

    }

    void Load_zone()
    {
        if (File.Exists(Application.dataPath + "/zone.json"))
        {
            string jsonString = File.ReadAllText(Application.dataPath + "/zone.json");
            var zone_setting = JsonUtility.FromJson<json_val>(jsonString);

            zone_count = zone_setting.zone_count;

            zone_set = new rect_zone[zone_count];

            for (int i = 0; i < zone_count; i++)
            {
                zone_set[i] = new rect_zone();
            }

            //zone_set[0].Left_up = test_v[0];
            //zone_set[0].Rigth_up = test_v[1];

            //zone_set[1].Left_up = test_v[2]; , (4,5) , (5,6)
            //zone_set[1].Rigth_up = test_v[3];

            for (int i = 0; i < zone_set.Length; i++)
            {
                zone_set[i].Left_up = zone_setting.save_val[ (i+1) * i ];
                zone_set[i].Rigth_up = zone_setting.save_val[(i + 1) * i + 1];
            }



        }
        else
        {
            Save_zone();
        }
    }

}
