#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class AkSpatialAudioBase : MonoBehaviour
{
    private AkGameObjRoomData m_roomData = new AkGameObjRoomData();

    protected void SetGameObjectInHighestPriorityRoom()
    {
        ulong highestPriorityRoomID = m_roomData.GetHighestPriorityRoomID();
        AkSoundEngine.SetGameObjectInRoom(gameObject, highestPriorityRoomID);
    }

    public void EnteredRoom(AkRoom room)
    {
        m_roomData.AddAkRoom(room);
        SetGameObjectInHighestPriorityRoom();
    }

    public void ExitedRoom(AkRoom room)
    {
        m_roomData.RemoveAkRoom(room);
        SetGameObjectInHighestPriorityRoom();
    }

    public void SetGameObjectInRoom()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.0f);
        foreach (var collider in colliders)
        {
            var room = collider.gameObject.GetComponent<AkRoom>();
            if (room != null)
            {
                m_roomData.AddAkRoom(room);
            }
        }
        SetGameObjectInHighestPriorityRoom();
    }
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.