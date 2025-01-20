using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

/* This class controls the behavior and learning process of the driver agent using the following points:
 * - The crux of the training is to teach the agent to pass through indexed check points laid on the race track in the correct order
 * - The faster an agent reaches the next checkpoint, the higher the reward, encouraging speed
 * - The agent is further encouraged to stay on the inside part of the track by passing through "Inside Track Guide" objects
 * - Touching track walls gives negative rewards, teaching the agent to avoid them
 */

public class DriverAgent : Agent
{
    [SerializeField] float turnSpeed = 100f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float maxSpeed = 10f;

    int nextTargetCheckPointIndex = 0;
    float timeSinceLastCheckPoint = 0;
    float currentSpeed = 0f;    

    public override void OnEpisodeBegin()
    {
        var behaviorParameters = GetComponent<BehaviorParameters>();
        
        if (behaviorParameters.BehaviorType == BehaviorType.InferenceOnly) // Skip start position randomization if using inference
            return;

        // Initialize training episode add randomization to the starting position
        timeSinceLastCheckPoint = Time.time;
        transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 1, Random.Range(-1.5f, 3.0f));
        transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        nextTargetCheckPointIndex = 0;
        currentSpeed = 0f;
    }

    // Set inputs based on the discrete actions the agent takes
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        switch (actions.DiscreteActions[0]) // Moving forward: 0 - No acceleration key pressed, 1 - acceleration key pressed
        {
            case 0: forwardAmount = 0; break;
            case 1: forwardAmount = 1; break;
        }

        switch (actions.DiscreteActions[1]) // Turning
        {
            case 0: turnAmount = 0; break; // No turning
            case 1: turnAmount = 1; break; // Turning right
            case 2: turnAmount = -1; break; // Turning left
        }

        SetInputs(forwardAmount, turnAmount);
    }

    // Tying manual controls to the actions
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;

        if (Input.GetKey(KeyCode.A))        
            discreteActions[1] = 2;        
        else if (Input.GetKey(KeyCode.D))        
            discreteActions[1] = 1;        
        else        
            discreteActions[1] = 0;        
    }

    // Simple tying of inputs to actual game world actions
    private void SetInputs(float forwardAmount, float turnAmount)
    {
        if (turnAmount != 0)        
            transform.Rotate(Vector3.up, turnAmount * turnSpeed * Time.deltaTime);        

        if (forwardAmount > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }
        else        
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime);        

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime); // Rigidbody usage omitted for the sake of a simple test
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Check Point"))
        {
            if (other.GetComponent<CheckPoint>().checkPointIndex == nextTargetCheckPointIndex)
            {
                timeSinceLastCheckPoint = Time.time - timeSinceLastCheckPoint;

                float baseReward = 10;
                float timeReward = Mathf.Max(10 * (3 - timeSinceLastCheckPoint), 0);                
                AddReward(baseReward + timeReward);

                Debug.Log("Correct check point passed, get reward: " + (baseReward + timeReward));

                timeSinceLastCheckPoint = Time.time;

                nextTargetCheckPointIndex++;
                nextTargetCheckPointIndex %= CheckPointManager.Instance.totalCheckPoints;
                
            }
            else
            {
                AddReward(-10f);
                Debug.Log("Wrong check point passed");
            }
        }
        else if (other.CompareTag("Inside Track Guide"))        
            AddReward(100f);        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            AddReward(-10f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            AddReward(-1f);
    }
}
