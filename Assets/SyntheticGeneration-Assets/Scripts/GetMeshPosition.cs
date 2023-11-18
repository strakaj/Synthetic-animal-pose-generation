using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMeshPosition : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    List<float> x;
    List<float> y;
    Vector2 resolution = GlobalVariables.resolution;
    public SkinnedMeshRenderer model_mesh;

    /// <summary>
    /// Generates bounding box from mesh. Script should be placed on object with SkinnedMeshRenderer component.
    /// </summary>
    /// <param name="camera"></param>
    /// <returns>Bounding box in camera view cordinates.</returns>
    public float[] get_mesh_positions(Camera camera)
    {
        x = new List<float>();
        y = new List<float>();

        Mesh baked = new Mesh();
        model_mesh.BakeMesh(baked);

        Vector3 scale = model_mesh.transform.localScale;

        Vector3 local_cordinates_scaled;
        Vector3 world_cordinates;
        Vector3 view_pos;
        foreach (var item in baked.vertices)
        {   
            local_cordinates_scaled = new Vector3(item.x / scale.x, item.y / scale.y, item.z / scale.z);
            world_cordinates = model_mesh.transform.TransformPoint(local_cordinates_scaled);
            view_pos = camera.WorldToScreenPoint(world_cordinates);

            x.Add(view_pos.x);
            y.Add(resolution.y - view_pos.y);
        }
        float[] bbox = { Mathf.Min(x.ToArray()), Mathf.Min(y.ToArray()), Mathf.Max(x.ToArray()), Mathf.Max(y.ToArray())  };

        baked = null;

        return bbox;
    }
}
