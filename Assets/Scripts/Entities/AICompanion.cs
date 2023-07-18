using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class AICompanion : Agent
{
    public bool isTraining;
    public RoomGenerator roomGenerator;
    public int numRaycasts;
    public float raycastMaxLength = 5;
    public LayerMask obstacleLayer;
    public Transform target;

    private Vector3[] raycastDirections;
    private Player _player;

    public static AICompanion instance;
    public Player player { get { return _player; } }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        // Calculate the angles for raycast directions
        raycastDirections = new Vector3[numRaycasts];
        float angleIncrement = 360f / numRaycasts;
        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = i * angleIncrement;
            raycastDirections[i] = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        }

        // Verify if the agent should be trained or play
        if (isTraining)
        {
            // If training, call the StartTraining() method
            StartTraining();
        }
    }

    private void Update()
    {
        if (!isTraining)
            RequestDecision();
    }

    private void StartTraining()
    {
        // Begin the training loop
        StartCoroutine(TrainingLoop());
    }

    private IEnumerator TrainingLoop()
    {
        while (PlayerController.Instance != null)
        {
            RequestDecision();

            // Wait for the decisions to be made
            yield return new WaitForSeconds(0.5f);

            bool shouldEndEpisode = CheckEndEpisodeCondition();

            if (shouldEndEpisode)
            {
                EndEpisode();
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        if (isTraining)
        {
            roomGenerator.Regenerate();
            PlayerController.Instance.transform.position = roomGenerator.RandomPosition();
            transform.position = roomGenerator.RandomPosition();
        }
        target = PlayerController.Instance.transform;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 targetDirection = target.position - transform.position;
        // Direction to target (2 observations)
        sensor.AddObservation(targetDirection.normalized);
        // Distance to target (1 observation)
        sensor.AddObservation(Mathf.Min(targetDirection.magnitude / 20f, 1f));

        // Raycasts around the AI (numRaycast observations)
        foreach (Vector2 direction in raycastDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction, raycastMaxLength, obstacleLayer);
            sensor.AddObservation(((hit.collider != null) ? hit.distance : raycastMaxLength) / raycastMaxLength);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Map the maxActionIndex to movement
        Vector2 movementDirection;

        if (!actionBuffers.DiscreteActions.IsEmpty())
        {
            movementDirection = Vector2.zero;
            switch (actionBuffers.DiscreteActions.Array[0])
            {
                case 1:
                    movementDirection.x = 1;
                    break;
                case 2:
                    movementDirection.x = -1;
                    break;
            }
            switch (actionBuffers.DiscreteActions.Array[1])
            {
                case 1:
                    movementDirection.y = 1;
                    break;
                case 2:
                    movementDirection.y = -1;
                    break;
            }
        }
        else
            movementDirection = new Vector2(actionBuffers.ContinuousActions.Array[0], actionBuffers.ContinuousActions.Array[1]);

        // Move the AI companion
        player.moveDirection = movementDirection;

        float directionReward = Vector2.Dot((target.position - transform.position).normalized, movementDirection);
        bool movingTowardObstacle = Physics2D.Raycast((Vector2)transform.position, movementDirection, 0.5f, obstacleLayer);

        // Penalize the AI for moving into obstacles
        if (movingTowardObstacle)
            AddReward(-1);
        // Reward the agent for moving toward the target except if it is moving toward a wall
        if (!movingTowardObstacle || directionReward < 0)
            // Dot product is 1 when the movement direction is exactly towards the target, -1 when exactly opposite
            AddReward(directionReward);
        // Penalize the AI for taking time to reach the target
        AddReward(-0.01f);
    }

    private bool CheckEndEpisodeCondition()
    {
        bool reachedTarget = (target.position - transform.position).sqrMagnitude < 1;

        // Reward the AI for reaching the target
        if (reachedTarget)
            AddReward(100);
        return reachedTarget;
    }
}
