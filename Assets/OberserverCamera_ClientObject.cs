
/// https://github.com/Strange-Land/StrangeLand-Base/issues/3
/// https://github.com/Strange-Land/StrangeLand-Base/issues/5
/// 
using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

#if USING_RERUN
using Rerun;
#endif

#if USING_RERUN
[RequireComponent(typeof(RerunPlaybackCameraManager))]
#endif


public class OberserverCamera_ClientObject : Client_Object {
    private ParticipantOrder m_ParticipantOrder;
    private SpawnType spawnType;
    private Interactable_Object MyInteractableObject;
    private ulong targetClient;

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
                DelinkCameras();
                break;
            case ActionState.LOADINGSCENARIO:
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
                break;
            case ActionState.RERUN:
                SetupForRerun();

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
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

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsServer) {
            gameObject.SetActive(false);
        }
    }

    public override void SetParticipantOrder(ParticipantOrder _ParticipantOrder) {
        m_ParticipantOrder = _ParticipantOrder;
    }

    public override ParticipantOrder GetParticipantOrder() {
        return m_ParticipantOrder;
    }

    public override void SetSpawnType(SpawnType _spawnType) {
        spawnType = _spawnType;
    }

    public override void AssignFollowTransform(Interactable_Object _MyInteractableObject, ulong _targetClient) {
        MyInteractableObject = _MyInteractableObject;
        targetClient = _targetClient;
    }

    public override Interactable_Object GetFollowTransform() {
        return MyInteractableObject;
    }

    public override void De_AssignFollowTransform(ulong clientID, NetworkObject netobj) {
        MyInteractableObject = null;
    }

    public override Transform GetMainCamera() {
#if USING_RERUN
        return _RerunCameraManager.GetFollowCamera();
#else
        return null;

#endif

    }

    public override void CalibrateClient(Action<bool> finishedCalibration) {
        Debug.Log($"Here we could try to find all relevant cameras again..");
        finishedCalibration.Invoke(true);
    }

    public override void StartQuestionair(QNDataStorageServer m_QNDataStorageServer) { }
    public override void GoForPostQuestion() {
        if (!IsLocalPlayer) return;
        PostQuestionServerRPC(OwnerClientId);
    }

    public override void SetNewNavigationInstruction(Dictionary<ParticipantOrder, NavigationScreen.Direction> Directions) {
        throw new NotImplementedException();
    }

    [ServerRpc]
    public void PostQuestionServerRPC(ulong clientID)
    {
        ConnectionAndSpawning.Singleton.FinishedQuestionair(clientID);
    }
}