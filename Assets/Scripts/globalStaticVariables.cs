using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class globalStaticVariables : MonoBehaviour
{

    public static globalStaticVariables Instance { get; private set; }

    public int Value;
    public Vector3 GlobalScale;

    public List<string> debugLog = new List<string>();


    [Header("Debug Mode")]
    public bool debugMode;


    [Header("Experimental")]
    public bool renderBoardAsSingleMesh = false;
    public bool obstacleGenerationComplete = false;

    [Header("Quality Settings")]
    [Tooltip("not sure how this affects framrate in editor")]
    
    public bool vSync = false;

    [Header("Grid Size")]
    [Range(3, 100)] public int gridXSizeHalfLength = 50;
    [Range(3, 100)] public int gridZSizeHalfLength = 50;

    public void Awake()
    {
        string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
        File.Delete(fileName);
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void callBuild()
    {
        
    }
    // Start is called before the first frame update


    void Start()
    {
      if(vSync)
        {
            QualitySettings.vSyncCount = 4;
        }
        debugLog.Add("TEST STRING");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnApplicationQuit()
    {
        Debug.Log("Quitting");
        string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
        //FileStream fs = new FileStream(fileName,FileMode.Create);
        //fs.Write(debugLog.ToString());
        StreamWriter writer = new StreamWriter(fileName, true);
        foreach(string str in debugLog)
        {
            writer.WriteLine(str);
        }
        writer.Close();
    }
}
