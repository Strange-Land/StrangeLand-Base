/// https://github.com/Strange-Land/StrangeLand-Base/issues/3
/// https://github.com/Strange-Land/StrangeLand-Base/issues/5
using UnityEngine;

using Unity.Netcode;
using UnityEngine.SceneManagement;

#if USING_RERUN
using Rerun;
#endif

#if USING_RERUN
[RequireComponent(typeof(RerunPlaybackCameraManager))]
#endif

public class OberserverCameraLocalServerObject : MonoBehaviour {
#if USING_RERUN
    private RerunPlaybackCameraManager _RerunCameraManager;
#endif
    // Start is called before the first frame update
    void Start() {
#if USING_RERUN
        _RerunCameraManager = GetComponent<RerunPlaybackCameraManager>();
#endif

        Debug.Log("ObserverCamera Manger ");
        ConnectionAndSpawning.Singleton.ServerStateChange += CameraUpdateStateTracker;
    }

    private void CameraUpdateStateTracker(ActionState state) {
        switch (state) {
            case ActionState.DEFAULT:
                break;
            case ActionState.WAITINGROOM:
                LinkCameras();
               
                break;
            case ActionState.LOADINGSCENARIO:
                 DelinkCameras();
                break;
            case ActionState.LOADINGVISUALS:
                break;
            case ActionState.READY:
                LinkCameras();
                Debug.Log("Attemtpitng to LinkCameras...");
                break;
            case ActionState.DRIVE:
                break;
            case ActionState.QUESTIONS:
                break;
            case ActionState.POSTQUESTIONS:
                DelinkCameras();
                break;
            case ActionState.RERUN:
                SetupForRerun();

                break;
            default: break;
        }
    }

    void OnDisable() {
        if (ConnectionAndSpawning.Singleton != null &&
            ConnectionAndSpawning.Singleton.ServerState == ActionState.RERUN) {
            SceneManager.sceneLoaded -= LoadUnityAction;
            SceneManager.sceneUnloaded -= UnloadUnityAction;
        }
    }

    private void SetupForRerun() {
        SceneManager.sceneLoaded += LoadUnityAction;
        SceneManager.sceneUnloaded += UnloadUnityAction;
    }

    private bool initFinished = false;

    private void UnloadUnityAction(Scene arg0) {
        if (ConnectionAndSpawning.Singleton.ServerState == ActionState.RERUN) {
            DelinkCameras();
        }
    }

    private void LoadUnityAction(Scene arg0, LoadSceneMode arg1) {
        if (ConnectionAndSpawning.Singleton.ServerState == ActionState.RERUN &&
            arg0.name != ConnectionAndSpawning.WaitingRoomSceneName) {
            LinkCameras();
        }
    }

    private void DelinkCameras() {
#if USING_RERUN
        _RerunCameraManager.DeLinkCameras();
#endif

    }

    private void LinkCameras() {
#if USING_RERUN
        _RerunCameraManager.LinkCameras();
#endif
    }
}