using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class WebRequest : MonoBehaviour
{
    List<LongwallData> lw;
    public Terrain TerrainMain;
    void Start()
    {
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://103.1.184.204:84/Longwall.aspx?AuthKey=Longwall123");
        yield return www.Send();

        // Create new List of Longwall data
        lw = new List<LongwallData>();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //show results as text
            Debug.Log("#1");
            //Debug.Log(www.downloadHandler.text);
            // Read the text from the URL
            string b = www.downloadHandler.text;
            // split the rows into lines
            string[] lines = b.Split(';');

            // for every line
            foreach (string l in lines)
            {
                if (l != "")
                {
                    // split the values
                    string[] values = l.Split('|');

                    // create and populate the new data point
                    var r = new LongwallData();
                    r.RowLabel = values[0];
                    r.XAxis = int.Parse(values[1]);
                    r.YAxis = int.Parse(values[2]);
                    r.FloorHeight = float.Parse(values[3]);
                    r.PointColor = values[4];

                    // populate the new data point
                    lw.Add(r);
                }
            }
        }
        string RowColor;
        int xRes = TerrainMain.terrainData.heightmapWidth;
        int yRes = TerrainMain.terrainData.heightmapHeight;
        Vector3 s = TerrainMain.terrainData.heightmapScale;
        Vector3 sc = new Vector3(1, 10000, 1);
        float[,] heights = TerrainMain.terrainData.GetHeights(0, 0, xRes, yRes);
        float[,,] alphas = TerrainMain.terrainData.GetAlphamaps(0, 0, TerrainMain.terrainData.alphamapWidth, TerrainMain.terrainData.alphamapHeight);

        for (int x = 0; x < TerrainMain.terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < TerrainMain.terrainData.alphamapHeight; y++)
            {
                alphas[x, y, 0] = 0;
                alphas[x, y, 1] = 0;
                alphas[x, y, 2] = 0;
                alphas[x, y, 3] = 0;
            }
        }


        foreach (LongwallData lwd in lw)
        {
            heights[lwd.XAxis, lwd.YAxis] = lwd.FloorHeight;
            RowColor = lwd.PointColor.Trim();



            switch (RowColor)
            {
                case "Red":
                    alphas[lwd.XAxis, lwd.YAxis, 0] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 1] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 2] = 1;
                    alphas[lwd.XAxis, lwd.YAxis, 3] = 0;
                    break;
                case "Yellow":
                    alphas[lwd.XAxis, lwd.YAxis, 0] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 1] = 1;
                    alphas[lwd.XAxis, lwd.YAxis, 2] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 3] = 0;
                    break;
                case "Green":
                    alphas[lwd.XAxis, lwd.YAxis, 0] = 1;
                    alphas[lwd.XAxis, lwd.YAxis, 1] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 2] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 3] = 0;
                    break;
                case "Grey":
                    alphas[lwd.XAxis, lwd.YAxis, 0] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 1] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 2] = 0;
                    alphas[lwd.XAxis, lwd.YAxis, 3] = 1;
                    break;

            }

        }

        
        if (Input.GetKeyDown("r"))
        {
            GetData();
        }

        TerrainMain.terrainData.SetHeights(0, 0, heights);
        TerrainMain.terrainData.SetAlphamaps(0, 0, alphas);
        


    }

    public GUIStyle ToolbarTexture;
    public GUIStyle ButtonTexture;
    void OnGUI()
    {
        //Toolbar
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height/10), "     Cut Coal Technology - 3D Mine Floor Model Application", ToolbarTexture);
        //Refresh Button
        if (GUI.Button(new Rect((Screen.width) / 10000 + 3, (Screen.height) / 1000 + 20, 35, Screen.height / 10), "Refresh", ButtonTexture))
        {
            GetData();
        }

    }



}









   