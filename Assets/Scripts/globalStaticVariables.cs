using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalStaticVariables : MonoBehaviour
{

    public static globalStaticVariables Instance { get; private set; }

    public int Value;
    public Vector3 GlobalScale;

    [Header("Debug Mode")]
    public bool debugMode;


    [Header("Experimental")]
    public bool renderBoardAsSingleMesh = false;


    [Header("Quality Settings")]
    [Tooltip("not sure how this affects framrate in editor")]
    
    public bool vSync = false;

    [Header("Grid Size")]
    [Range(3, 100)] public int gridXSizeHalfLength = 50;
    [Range(3, 100)] public int gridZSizeHalfLength = 50;

    public void Awake()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
