﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public void GotKey()
    {
        Destroy(gameObject);
    }
}
