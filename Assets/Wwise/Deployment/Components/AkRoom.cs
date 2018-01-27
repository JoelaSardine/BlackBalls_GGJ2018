#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
﻿using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Wwise/AkRoom")]
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
/// @brief An AkRoom is an enclosed environment that can only communicate to the outside/other rooms with AkRoomPortals
/// @details 
public class AkRoom : MonoBehaviour
{
    public class AkRoom_CompareByPriority : IComparer<AkRoom>
    {
        public virtual int Compare(AkRoom a, AkRoom b)
        {
            int result = a.priority.CompareTo(b.priority);

            if (result == 0 && a != b)
                return 1;
            else
                return result;
        }
    }

    static public AkRoom_CompareByPriority s_compareByPriority = new AkRoom_CompareByPriority();

    static public ulong INVALID_ROOM_ID = unchecked((ulong)-1.0f);

    /// The reverb auxiliary bus.
    public AK.Wwise.AuxBus reverbAuxBus;

    [Range(0, 1)]
    /// The reverb control value for the send to the reverb aux bus.
    public float reverbLevel = 1;

    [Range(0, 1)]
    /// Occlusion level modeling transmission through walls.
    public float wallOcclusion = 1;

    [Tooltip("Higher number has a higher priority")]
    /// In cases where a game object is in an area with two rooms, the higher priority room will be chosen for AK::SpatialAudio::SetGameObjectInRoom()
    /// The higher the priority number, the higher the priority of a room.
    public int priority = 0;

    /// Access the room's ID
    public ulong GetID() { return (ulong)GetInstanceID(); }

    private void OnEnable()
    {
        AkRoomParams roomParams = new AkRoomParams();

        roomParams.Up.X = transform.up.x;
        roomParams.Up.Y = transform.up.y;
        roomParams.Up.Z = transform.up.z;

        roomParams.Front.X = transform.forward.x;
        roomParams.Front.Y = transform.forward.y;
        roomParams.Front.Z = transform.forward.z;

        roomParams.ReverbAuxBus = (uint)reverbAuxBus.ID;
        roomParams.ReverbLevel = reverbLevel;
        roomParams.WallOcclusion = wallOcclusion;

        RoomCount++;
        AkSoundEngine.SetRoom(GetID(), roomParams, name);
    }

    private void OnDisable()
    {
        RoomCount--;
        AkSoundEngine.RemoveRoom(GetID());
    }

    void OnTriggerEnter(Collider in_other)
    {
        var spatialAudioObjects = in_other.GetComponentsInChildren<AkSpatialAudioBase>();
        for(int i = 0; i < spatialAudioObjects.Length; i++)
        {
            if (spatialAudioObjects[i].enabled)
                spatialAudioObjects[i].EnteredRoom(this);
        }
    }

    void OnTriggerExit(Collider in_other)
    {
        var spatialAudioObjects = in_other.GetComponentsInChildren<AkSpatialAudioBase>();
        for (int i = 0; i < spatialAudioObjects.Length; i++)
        {
            if (spatialAudioObjects[i].enabled)
                spatialAudioObjects[i].ExitedRoom(this);
        }
    }

    static int RoomCount = 0;

    public static bool IsSpatialAudioEnabled
    {
        get { return RoomCount > 0; }
    }
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.