using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
    public static float agent_speed = 1.5f;
    public static float animation_speed = 1.5f;
    public static int max_kp_out_of_frame = 8;
    public static string format = "jpg";
    public static int rotation_speed = 100;
    public static int min_frames = 2; 
    public static int max_frames = 5;
    public static Vector2 resolution = new Vector2(1600, 1200);

    public static string path = "";
    public static string scene_name = "scene_1";
    public static int start_from = 0;
    public static int end_at = 10;
}
