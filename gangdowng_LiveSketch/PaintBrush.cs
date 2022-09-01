using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    public static PaintBrush paintBrush;

    public int resolution = 512;
    public int resolution_w = 1920;
    public int resolution_h = 1080;
    Texture2D whiteMap;
    public float brushSize;
    public Texture2D brushTexture;
    private Texture2D CopiedBrushTexture;
    public Color brushColor = Color.white;
    public Color[] color_list;
    Vector2 stored;    

    public static Dictionary<Collider, RenderTexture> paintTextures = new Dictionary<Collider, RenderTexture>();

    public Vector2[] spacing_arr;
    Vector2 pixelUV;

    public Collider set;

    [Range(1, 300)]
    public int spacing = 100;

    private void OnEnable()
    {
        if(paintBrush != null)
        {
            paintBrush = this;
        }

        spacing_arr = new Vector2[spacing];

        CreateClearTexture();
    }

    private void OnDisable()
    {
        CreateClearTexture();
    }

    void Start()
    {        
        CopyBrushTexture();
    }

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {                

                Collider coll = hit.collider;
                if (coll != null)
                {
                    if (!paintTextures.ContainsKey(coll)) // if there is already paint on the material, add to that
                    {
                        Renderer rend = hit.transform.GetComponent<Renderer>();
                        paintTextures.Add(coll, getWhiteRT());
                        rend.material.SetTexture("_PaintTex", paintTextures[coll]);
                    }
                    if (stored != hit.lightmapCoord) // stop drawing on the same point
                    {
                        if (stored != Vector2.zero)
                        {
                            // 터치 인풋이나 빠르게 움직일시 끊겨서 보이는 현상이 있음
                            // 프레임별로 마지막 터치지점에서 현재 터치지점까지 길이를 일정간격으로 나눈뒤
                            // 색칠해주게 되면 자연스럽게 보인다.
                            spacing_arr[0] = stored;
                            for (int i = 1; i < spacing_arr.Length; i++)
                            {
                                float x = (hit.lightmapCoord.x - stored.x) / spacing_arr.Length * i;
                                float y = (stored.y - hit.lightmapCoord.y) / spacing_arr.Length * i;
                                spacing_arr[i].x = spacing_arr[0].x + x;
                                spacing_arr[i].y = spacing_arr[0].y - y;
                            }
                            for (int i = 0; i < spacing_arr.Length; i++)
                            {
                                pixelUV = spacing_arr[i];
                                pixelUV.y *= resolution_h;
                                pixelUV.x *= resolution_w;

                                DrawTexture(paintTextures[coll], pixelUV.x, pixelUV.y);
                            }
                        }
                        else
                        {
                            //Debug.Log("start_reset");
                        }
                        
                        stored = hit.lightmapCoord;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            stored = Vector2.zero;
        }
        

    }

    void DrawTexture(RenderTexture rt, float posX, float posY)
    {

        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, resolution_w, resolution_h, 0);        
        Graphics.DrawTexture(new Rect
            (
                posX - CopiedBrushTexture.width / brushSize, 
                (rt.height - posY) - CopiedBrushTexture.height / brushSize, 
                CopiedBrushTexture.width / (brushSize * 0.5f), 
                CopiedBrushTexture.height / (brushSize * 0.5f)), 
                CopiedBrushTexture
            );
        GL.PopMatrix();
        RenderTexture.active = null;
        
    }

    RenderTexture getWhiteRT()
    {
        RenderTexture rt = new RenderTexture(resolution_w, resolution_h, 32);
        Graphics.Blit(whiteMap, rt);
        return rt;
    }

    void CreateClearTexture()
    {
        whiteMap = new Texture2D(1920, 1080);

        // * 간혹 키오스크 경우에따라 에디터에서는 정상이지만
        // 빌드시 뒷배경이 이상하게 보이니 초기화해주자
        for (int y = 0; y < whiteMap.height; y++)
        {
            for (int x = 0; x < whiteMap.width; x++)
            {
                whiteMap.SetPixel(x, y, Color.white);
            }
        }

        whiteMap.Apply();
    }

    private void CopyBrushTexture()
    {
        if (brushTexture == null) return;
        
        DestroyImmediate(CopiedBrushTexture);
        {
            CopiedBrushTexture = new Texture2D(brushTexture.width, brushTexture.height,TextureFormat.ARGB32, false);
            CopiedBrushTexture.filterMode = FilterMode.Point;
            
        }

        int height = brushTexture.height;
        int width = brushTexture.width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = color_list[7];
                c.a *= brushTexture.GetPixel(x, y).a;

                CopiedBrushTexture.SetPixel(x, y, c);
            }
        }

        CopiedBrushTexture.Apply();

        Debug.Log("Copy Brush Texture");
    }

    public void change_color(int num)
    {
        if (brushTexture == null) return;
        
        DestroyImmediate(CopiedBrushTexture);

        {
            CopiedBrushTexture = new Texture2D(brushTexture.width, brushTexture.height, TextureFormat.ARGB32, false);
            CopiedBrushTexture.filterMode = FilterMode.Point;
        }

        int height = brushTexture.height;
        int width = brushTexture.width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = color_list[num];
                c.a *= brushTexture.GetPixel(x, y).a;                
                CopiedBrushTexture.SetPixel(x, y, c);
            }
        }

        CopiedBrushTexture.Apply();
    }

    public void Reset_draw_page()
    {
        paintTextures.Clear();
        this.gameObject.GetComponent<MeshRenderer>().material.SetTexture("_PaintTex", null);
    }

}
