using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZoneManager : MonoBehaviour
{
    public List<EnemyZone> zones = new List<EnemyZone>();

    [System.Serializable]
    public class EnemyZone
    {
        public string name;
        public Vector2 center;
        public Vector2 size;

        [HideInInspector] public bool isOccupied = false;
        [HideInInspector] public GameObject zoneObject;
        [HideInInspector] public List<GameObject> activeEnemies = new List<GameObject>();

        public Vector3 Position => zoneObject != null ? zoneObject.transform.position : center;

        public float Width => size.x;
        public float Height => size.y;

        public bool Contains(Vector3 pos)
        {
            Vector2 topLeft = center - size / 2f;
            Vector2 bottomRight = center + size / 2f;
            return pos.x >= topLeft.x && pos.x <= bottomRight.x &&
                pos.y >= topLeft.y && pos.y <= bottomRight.y;
        }
    }

    public EnemyZone GetAvailableZone()
    {
        foreach (var zone in zones)
        {
            if (!zone.isOccupied)
                return zone;
        }
        return null;
    }

    public void DrawGizmos()
    {
        foreach (var zone in zones)
        {
            Gizmos.color = zone.isOccupied ? Color.red : Color.green;
            Gizmos.DrawWireCube(zone.center, zone.size);
        }
    }

    void OnDrawGizmos()
    {
        DrawGizmos();
    }  
}
