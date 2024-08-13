
using Unity.Netcode;
using UnityEngine;

#if USING_RERUN
using UltimateReplay;
#endif


public class SkeletonNetworkScript : NetworkBehaviour
{
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        Debug.Log("About to destroy client irrelevant objects!");
        if (IsServer) { }
        else {
#if USING_RERUN
            GetComponentInParent<ReplayObject>().enabled = false;
            foreach (var t in GetComponentsInChildren<ReplayTransform>()) {
                t.enabled = false;
                
            }
#endif

            GetComponentInChildren<BoxCollider>().enabled = false;
        }
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();
        
    }
}
