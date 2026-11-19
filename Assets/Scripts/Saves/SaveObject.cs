using UnityEngine;
using UnityEngine.InputSystem;

public class SaveObject : MonoBehaviour
{
    private GameObject gameSaverObj;
    private GameSaver gameSaver;


    private void OnEnable()
    {
        if (gameSaver == null)
        {
            gameSaverObj = transform.parent.gameObject;
            while(gameSaverObj.name != "GameSaver")
            {
                if (gameSaverObj.transform.parent == null) break;
                gameSaverObj = gameSaverObj.transform.parent.gameObject;
            }
            if (gameSaverObj != null) gameSaver = gameSaverObj.GetComponent<GameSaver>();
            else return;
        }
        gameSaver.SaveAction.action.started += save;
    }

    private void OnDisable()
    {
        if (gameSaver == null) return;
        gameSaver.SaveAction.action.started -= save;
    }

    private void save(InputAction.CallbackContext context)
    {
        gameSaver.objsData.Add(
            new SaveObj
            { 
                id = gameObject, 
                position = transform.position, 
                rotation = transform.rotation, 
                scale = transform.localScale
            }
        );
    }
}
