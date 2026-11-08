using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavePlayerData
{
    public GameObject id;
    public Vector3 position;
    public Quaternion rotation;
    public bool has2Jump;
    public bool hasDash;
    public PlayerCameraControl cameraPivot;
    public float cameraX;
}

[Serializable]
public class SavePlayerList
{
}

[Serializable]
public class SaveObj
{
    public bool isPlayer;
    public GameObject id;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[Serializable]
public class SaveDataList
{
    public List<SaveObj> objs = new List<SaveObj>();
    public List<SavePlayerData> players = new List<SavePlayerData>();
}