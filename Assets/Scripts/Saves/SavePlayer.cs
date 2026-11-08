using UnityEngine;
using UnityEngine.InputSystem;

public class SavePlayer : MonoBehaviour
{
    [SerializeField] private PlayerCameraControl cameraPivot;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerDash playerDash;
    [SerializeField] private GameSaver gameSaver;
    private GameObject gameSaverObj;


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
        gameSaver.playerData.Add(
            new SavePlayerData
            { 
                id = gameObject, 
                position = transform.position,
                rotation = transform.rotation,
                has2Jump = playerMovement._ExtraJump,
                hasDash = playerDash._CanDash,
                cameraPivot = cameraPivot,
                cameraX = cameraPivot._xRotation
            }
        );
    }
}
