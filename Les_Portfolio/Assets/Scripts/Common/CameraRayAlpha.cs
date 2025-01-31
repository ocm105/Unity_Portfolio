using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SurfaceType
{
    Opaque,
    Transparent
}
public class CameraRayAlpha : MonoBehaviour
{
    //1.카메라부터 캐릭터까지 Raycast를 쏜다.
    [SerializeField] Transform player;
    [SerializeField] float opacity;
    [SerializeField] LayerMask layer;

    private float distance;
    private Vector3 direction;

    private List<RaycastHit> hits = new List<RaycastHit>();
    private List<GameObject> obstacleObjects = new List<GameObject>();

    private void FixedUpdate()
    {
        if (hits != PlayerRaycast())
        {
            hits.Clear();

            for (int i = obstacleObjects.Count - 1; i >= 0; i--)
            {
                OpaqueSet(obstacleObjects[i]);
            }
            obstacleObjects.Clear();

            hits = PlayerRaycast();
            for (int i = 0; i < hits.Count; i++)
            {
                obstacleObjects.Add(hits[i].collider.gameObject);
                TransparentSet(obstacleObjects[i]);
            }

            // => Ray가 더이상 맞지 않거나, 다른물체에 맞았을경우 Renderer 교체 처리

            // => Obstacle들의 Renderer의 렌더모드는 Transparent여야함.

            // => Custom Shader에서는 별도 처리가 필요할수 있음.
        }
    }

    // 카메라와 플레이어 사이 RayCast
    private List<RaycastHit> PlayerRaycast()
    {
        distance = Vector3.Distance(transform.position, player.position);
        direction = (player.position - transform.position).normalized;

        return Physics.RaycastAll(transform.position, direction, distance - 1f, layer).ToList();
    }

    public void SetInit()
    {
        hits.Clear();

        for (int i = obstacleObjects.Count - 1; i >= 0; i--)
        {
            OpaqueSet(obstacleObjects[i]);
        }
        obstacleObjects.Clear();
    }

    #region Event
    // 불투명 처리
    private void OpaqueSet(GameObject obj)
    {
        Material[] mats = obj.GetComponent<MeshRenderer>() ? obj.GetComponent<MeshRenderer>().materials : obj.GetComponent<SkinnedMeshRenderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetFloat("_Opacity", 1);
        }
    }
    // 투명 처리
    private void TransparentSet(GameObject obj)
    {
        Material[] mats = obj.GetComponent<MeshRenderer>() ? obj.GetComponent<MeshRenderer>().materials : obj.GetComponent<SkinnedMeshRenderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetFloat("_Opacity", opacity);
        }
    }
    #endregion
}
