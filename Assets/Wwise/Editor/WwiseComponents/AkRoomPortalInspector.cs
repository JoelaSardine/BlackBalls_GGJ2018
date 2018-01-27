#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System;


[CustomEditor(typeof(AkRoomPortal))]
public class AkRoomPortalInspector : Editor
{
    AkUnityEventHandlerInspector m_OpenPortalEventHandlerInspector = new AkUnityEventHandlerInspector();
    AkUnityEventHandlerInspector m_ClosePortalEventHandlerInspector = new AkUnityEventHandlerInspector();

    [UnityEditor.MenuItem("GameObject/Wwise/Room Portal", false, 1)]
    public static void CreatePortal()
    {
        GameObject portal = new GameObject("RoomPortal");

        Undo.AddComponent<AkRoomPortal>(portal);
        portal.GetComponent<Collider>().isTrigger = true;

        Selection.objects = new UnityEngine.Object[] { portal };
    }

    AkRoomPortal m_roomPortal;
    int[] m_selectedIndex = new int[2];

    void OnEnable()
    {
        m_OpenPortalEventHandlerInspector.Init(serializedObject, "triggerList", "Open On: ", false);
        m_ClosePortalEventHandlerInspector.Init(serializedObject, "closePortalTriggerList", "Close On: ", false);

        m_roomPortal = target as AkRoomPortal;

        FindOverlappingRooms();
        for (int i = 0; i < 2; i++)
        {
            int index = m_roomPortal.roomList[i].list.IndexOf(m_roomPortal.rooms[i]);
            m_selectedIndex[i] = index == -1 ? 0 : index;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        m_OpenPortalEventHandlerInspector.OnGUI();
        m_ClosePortalEventHandlerInspector.OnGUI();

        GUILayout.BeginVertical("Box");
        {
            string[] labels = new String[2];
            labels[0] = "Back";
            labels[1] = "Front";

            for (int i = 0; i < 2; i++)
            {
                string[] roomLabels = new String[m_roomPortal.roomList[i].list.Count];

                for (int j = 0; j < roomLabels.Length; j++)
                {
                    if (m_roomPortal.roomList[i].list[j] != null)
                    {
                        roomLabels[j] = j + 1 + ". " + m_roomPortal.roomList[i].list[j].name;
                    }
                    else
                    {
                        m_roomPortal.roomList[i].list.RemoveAt(j);
                    }
                }

                m_selectedIndex[i] = EditorGUILayout.Popup(labels[i] + " Room", m_selectedIndex[i], roomLabels);

                m_roomPortal.rooms[i] = (m_selectedIndex[i] < 0 || m_selectedIndex[i] >= m_roomPortal.roomList[i].list.Count) ? null : m_roomPortal.roomList[i].list[m_selectedIndex[i]];
            }
        }
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    public void FindOverlappingRooms()
    {
        BoxCollider portalCollider = m_roomPortal.gameObject.GetComponent<BoxCollider>();
        if (portalCollider == null)
            return;

        // compute halfExtents and divide the local z extent by 2
        Vector3 halfExtents = new Vector3(
            portalCollider.size.x * m_roomPortal.transform.localScale.x / 2,
            portalCollider.size.y * m_roomPortal.transform.localScale.y / 2,
            portalCollider.size.z * m_roomPortal.transform.localScale.z / 4);

        // move the center backward
        FillRoomList(Vector3.forward * -0.25f, halfExtents, 0);

        // move the center forward
        FillRoomList(Vector3.forward * 0.25f, halfExtents, 1);
    }

    void FillRoomList(Vector3 center, Vector3 halfExtents, int roomListIndex)
    {
        m_roomPortal.roomList[roomListIndex].list.Clear();

        center = m_roomPortal.transform.TransformPoint(center);

        Collider[] colliders = Physics.OverlapBox(center, halfExtents, m_roomPortal.transform.rotation);

        foreach (var collider in colliders)
        {
            var room = collider.gameObject.GetComponent<AkRoom>();
            if (room != null && !m_roomPortal.roomList[roomListIndex].list.Contains(room))
                m_roomPortal.roomList[roomListIndex].list.Add(room);
        }
    }
}
#endif