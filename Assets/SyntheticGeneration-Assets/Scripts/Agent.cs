using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Agent : MonoBehaviour
{
    public Transform[] target_positions;
    public float distance_thr = 0.5f;
    public CameraCapture camera_capture;
    public string[] animation_names = {"sit"};
    public Transform object_to_rotate;
    public NavMeshAgent nav_agent;
    private Animator animator;

    int frames_min = GlobalVariables.min_frames;
    int frames_max = GlobalVariables.max_frames;
    

    private bool animation_running = false;
    private float angle_rotated = 0f;
    float rotation_speed = GlobalVariables.rotation_speed;
    private float direction;
    
    private Transform target_position;
    private int idx  = 0;
    private List<float> distance_steps;
    

    /// <summary>
    /// Sets global variables and 
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = GlobalVariables.animation_speed;
        nav_agent.speed = GlobalVariables.agent_speed;

        shuffle(target_positions);
        set_target();
    }

    /// <summary>
    /// Shuffles items in array.
    /// </summary>
    /// <param name="arr"></param>
    void shuffle(Transform[] arr)
    {
        for (int t = 0; t < arr.Length; t++ )
        {
            Transform tmp = arr[t];
            int r = Random.Range(t, arr.Length);
            arr[t] = arr[r];
            arr[r] = tmp;
        }
    }

    /// <summary>
    /// Resets animation and movement of the model. Defined for animator.
    /// </summary>
    public void event_animation_completed()
    {
        animator.SetTrigger("walk");
        nav_agent.isStopped = false;
        animation_running = false;
        angle_rotated = 0f;
    }

    /// <summary>
    /// Wrapper for screenshot capture. Defined for animator.
    /// </summary>
    public void event_animation_capture()
    {
        camera_capture.Capture(true);
    }

    /// <summary>
    /// Sets positions along the trajectory where screenshot will be taken.
    /// </summary>
    private void set_target()
    {
        if (target_positions.Length == 0)
            return;

        // set position
        target_position = target_positions[idx];
        nav_agent.destination = target_position.position;  
        idx++;

        // set positions where to take screenshot based on distance
        int frames_between_targets = Random.Range(frames_min, frames_max);
        float distance = Vector3.Distance (gameObject.transform.position, target_position.transform.position);
        float step = distance /  frames_between_targets;
        distance_steps = new List<float>();
        if(distance > distance_thr)
        {   
            for (int i = 1; i < frames_between_targets+1; i++)
            {   
                float multiplier = Random.Range(0.95f, 1.05f);
                float pos = distance - (step * i * multiplier);
                distance_steps.Add(pos);
            }
        }

        // reset idx and shuffle targets
        if (idx == target_positions.Length)
        {
            idx = 0;
            shuffle(target_positions);
        }
    }

    /// <summary>
    /// Monitors if model arrived at target position and initializes screenshot capture.
    /// </summary>
    void Update()
    {
        if (target_positions.Length == 0)
            return;

        // take screenshot at position along trajectory
        float distance = Vector3.Distance(gameObject.transform.position, target_position.transform.position);
        if ((distance_steps.Count > 0) && (distance - distance_steps[0] < distance_thr ))  
        {
            // take screenshot
            distance_steps.RemoveAt(0);
            camera_capture.Capture(false);

            string animaton_name = animation_names[Random.Range(0, animation_names.Length)];

            animator.SetTrigger(animaton_name);
            nav_agent.isStopped = true;

            animation_running = true;
            direction = Random.Range(-1, 1);
            if (direction == 0)
            {
                direction = 1;
            }
        }

        // rotate model
        if(animation_running == true && Mathf.Abs(angle_rotated) < 90f)
        {
            float multiplier = Random.Range(0.95f, 1.05f);
            float rot_angle = 1f * rotation_speed * Time.deltaTime * multiplier * direction;
            angle_rotated += rot_angle;
            object_to_rotate.transform.Rotate(0.0f, rot_angle, 0.0f, Space.Self);
        }
        
        // reached target
        if (distance < distance_thr)
        {
            set_target();
        }

    }
}
