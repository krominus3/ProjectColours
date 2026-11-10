using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameSaver : MonoBehaviour
{
    public List<SaveObj> objsData;
    public List<SavePlayerData> playerData;

    [SerializeField] private LensInventory inventory;
    [SerializeField] private InputActionReference saveAction;
    [SerializeField] private InputActionReference loadAction;
    public InputActionReference SaveAction => saveAction;
    public InputActionReference LoadAction => loadAction;

    private void OnEnable()
    {
        saveAction.action.Enable();
        loadAction.action.Enable();
        saveAction.action.performed += SaveData;
        loadAction.action.performed += LoadData;
    }

    private void OnDisable()
    {
        saveAction.action.Disable();
        loadAction.action.Disable();
        saveAction.action.performed -= SaveData;
        loadAction.action.performed -= LoadData;
    }

    public void SaveData(InputAction.CallbackContext context)
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        var wrapper = new SaveDataList { objs = objsData , players = playerData };
        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(path, json);
        objsData = new List<SaveObj>();
    }

    public void LoadData(InputAction.CallbackContext context)
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<SaveDataList>(json);

        if (wrapper?.objs == null) return;

        foreach (var obj in wrapper.objs)
        {
            obj.id.transform.position = obj.position;
            obj.id.transform.rotation = obj.rotation;
            obj.id.transform.localScale = obj.scale;
        }

        if (wrapper?.players == null) return;

        foreach (var player in wrapper.players)
        {
            CharacterController cc = player.id.GetComponent<CharacterController>();
            cc.enabled = false;
            player.id.transform.position = player.position;
            cc.enabled = true;
            cc.Move(Vector3.zero);
            if(player.has2Jump)
                player.id.GetComponent<PlayerMovement>().AddExtraJump();
            if(player.hasDash)
                player.id.GetComponent<PlayerDash>().AddDash();
            player.id.transform.rotation = player.rotation;
            player.cameraPivot._xRotation = player.cameraX;

            inventory.ClearAll();
            foreach(LensData lens in player.lenses)
                inventory.Equip(lens);
        }
    }
}