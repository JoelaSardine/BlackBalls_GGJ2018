#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public class AkGameObjRoomData
{
	/// Contains all active rooms sorted by priority.
	private List<AkRoom> activeRooms = new List<AkRoom>();

    public ulong GetHighestPriorityRoomID()
    {
        if (activeRooms.Count == 0)
        {
            // we're outside
            return AkRoom.INVALID_ROOM_ID;
        }

        return activeRooms.Last().GetID();
    }

    public void AddAkRoom(AkRoom room)
    {
        int index = activeRooms.BinarySearch(room, AkRoom.s_compareByPriority);
        if (index < 0)
        {
            activeRooms.Insert(~index, room);
        }
    }

    public void RemoveAkRoom(AkRoom room)
    {
        activeRooms.Remove(room);
    }
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.