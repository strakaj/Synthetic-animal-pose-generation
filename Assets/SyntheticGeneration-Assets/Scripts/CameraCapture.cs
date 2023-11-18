using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CameraCapture : MonoBehaviour
{
    public int fileCounter;
    public Transform[] keypoints;
    public GameObject[] scene_variants;

    int start_from = GlobalVariables.start_from;
    int end_at = GlobalVariables.end_at;
    string scene_name = GlobalVariables.scene_name;
    string image_format = GlobalVariables.format;
    string path = GlobalVariables.path;
    int max_kp_out_of_frame = GlobalVariables.max_kp_out_of_frame;
    private Vector2 size = GlobalVariables.resolution;

    private RenderTexture render_texture;
    private Camera Camera;
    private GetMeshPosition mesh;
    private CameraJitter camera_jitter;


    /// <summary>
    /// Sets file counter and gets reference.
    /// </summary>
    private void Start()
    {
        fileCounter = start_from;
        Camera = gameObject.GetComponent<Camera>();
        mesh = gameObject.GetComponent<GetMeshPosition>();
        camera_jitter = gameObject.GetComponent<CameraJitter>();

    }

    /// <summary>
    /// Gets and transforms position of keypoints to screen view.
    /// </summary>
    /// <returns>Keypoints as string in json format.</returns>
    public string get_positions()
    {
        int keypoints_out_of_frame = 0;
        string positions = "\"keypoints\": {";
        foreach (var kp in keypoints)
        {
            Vector3 viewPos = Camera.WorldToScreenPoint(kp.transform.position);
            string name = kp.transform.name;
            
            float y =  size.y - viewPos.y;
            positions += "\"" + name + "\": [" + viewPos.x + ";" + y + "];\n";

            if( viewPos.x < 0 || viewPos.x > size.x || y < 0 || y > size.y)
                keypoints_out_of_frame++;

            if (keypoints_out_of_frame > max_kp_out_of_frame)
                return null;
        }
        positions = positions.Trim(';', '\n');
        positions += "}";
        return positions;
    }

    /// <summary>
    /// Wrapper for screen capture sequence.
    /// </summary>
    /// <param name="invoked_by_animation"></param>
    public void Capture(bool invoked_by_animation)
    {
        StartCoroutine(capture_after_wait(invoked_by_animation));
    }

    /// <summary>
    /// Saves position of model, keypoints and takes screenshot of camera view.
    /// </summary>
    /// <param name="invoked_by_animation"></param>
    public void _Capture(bool invoked_by_animation)
    {
        if (fileCounter >= end_at)
        {
            to_menu();
            return;
        }

        if (GlobalVariables.path == "")
        {
            fileCounter++;
            return;
        }
            
        string keypoint_positions_text = get_positions();
        if(keypoint_positions_text == null)
            return;

        float[] bbox = mesh.get_mesh_positions(Camera);
        if(bbox[0] < 0)
            bbox[0] = 0.0f;
        if(bbox[1] < 0)
            bbox[1] = 0.0f;
        if(bbox[2] > size.x)
            bbox[2] = size.x;
        if(bbox[3] > size.y)
            bbox[3] = size.y;

        string bbox_position_text = "\"bbox\": [";
        
        foreach (var item in bbox)
        {
            bbox_position_text += item + ";";
        }
        bbox_position_text = bbox_position_text.Trim(';');
        bbox_position_text += "]";

        string by_anim = "false";
        if (invoked_by_animation == true)
            by_anim = "true";
        string annotation_text = "{\n" + "\"in_animation\": " + by_anim + ";\n" + "\"scene_name\": \"" + scene_name + "\";\n" + keypoint_positions_text + "; \n" + bbox_position_text + "\n}";

        annotation_text = annotation_text.Replace(",", ".");
        annotation_text = annotation_text.Replace(";", ",");
        File.WriteAllText(path + fileCounter + ".json", annotation_text);

        // capture screen
        render_texture = new RenderTexture((int)size.x, (int)size.y, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Camera.targetTexture = render_texture;
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;
    
        Camera.Render();

        Texture2D image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height); 
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;

        byte[] bytes = null;
        if (image_format == "png")
        {
            bytes = image.EncodeToPNG();
        }
        else
        {
            bytes = image.EncodeToJPG();
            image_format = "jpg";
        }
        
        Destroy(image);

        File.WriteAllBytes(path + fileCounter + "." + image_format, bytes);
        fileCounter++;

        Camera.targetTexture = null;
        Destroy(render_texture);
        bytes = null;
    }

    private void to_menu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Sets randomly one of the scene variants, rotates camera and starts capture sequence. 
    /// </summary>
    /// <param name="invoked_by_animation"></param>
    /// <returns></returns>
    IEnumerator capture_after_wait(bool invoked_by_animation)
    {

        camera_jitter.applay_transform();
    
        int scene_variant_idx = Random.Range(0, scene_variants.Length);
        foreach (var scene_variant in scene_variants)
        {
            scene_variant.SetActive(false);
        }
        scene_variants[scene_variant_idx].SetActive(true);


        yield return new WaitForSeconds(1);
        _Capture(invoked_by_animation);

        camera_jitter.reset_transform();

    }
}

