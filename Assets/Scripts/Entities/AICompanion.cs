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
    private Room room;
    private Vector2 roomSize;
    public int numRaycasts;
    public float raycastMaxLength = 5;
    public LayerMask obstacleLayer;
    public Transform target;

    private Vector3[] raycastDirections;
    private MovingObject moveComponent;

    public static AICompanion instance;
    private Entity _entity;
    public Entity entity { get { return _entity; } }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        _entity = GetComponent<Entity>();
    }

    private void Start()
    {
        moveComponent = GetComponent<MovingObject>();
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
        while (Player.Instance != null)
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
            Player.Instance.transform.position = roomGenerator.RandomPosition();
            transform.position = roomGenerator.RandomPosition();
        }
        room = roomGenerator.room;
        roomSize = new Vector2(room.roomBoundingBox.size.x, room.roomBoundingBox.size.y) * room.partSize;
        target = Player.Instance.transform;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Room part size and room size (4 observations)
        // sensor.AddObservation(room.partSize);
        // sensor.AddObservation(roomSize);



        // AI && target positions (4 observations)
        // sensor.AddObservation(transform.position / roomSize);
        // sensor.AddObservation(target.position / roomSize);

        Vector2 targetDirection = target.position - transform.position;
        // Direction to target (2 observations)
        sensor.AddObservation(targetDirection.normalized);
        // Distance to target (1 observation)
        sensor.AddObservation(Mathf.Min(targetDirection.magnitude / 20, 1f));

        // Raycasts around the AI (numRaycast observations)
        foreach (Vector2 direction in raycastDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction, raycastMaxLength, obstacleLayer);
            sensor.AddObservation(((hit.collider != null) ? hit.distance : raycastMaxLength) / raycastMaxLength);
        }

        // Room parts around the current one (8 observations)
        // Vector2Int currentRoomPart = room.GetPartPositionFromRoomPosition(transform.position);
        // for (int x = -1; x <= 1; x++)
        //     for (int y = -1; y <= 1; y++)
        //         if (!(x == 0 && y == 0))
        //             sensor.AddObservation(room.partsPositions.Contains(currentRoomPart + new Vector2Int(x, y)));

        // Total observations: 19 + numRaycast
        if (GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize != sensor.ObservationSize())
            GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = sensor.ObservationSize();
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
            // if (movementDirection == Vector2.zero)
            //     AddReward(-0.1f);
        }
        else
            movementDirection = new Vector2(actionBuffers.ContinuousActions.Array[0], actionBuffers.ContinuousActions.Array[1]).normalized;

        // Move the AI companion
        if (isTraining)
            transform.Translate(movementDirection * entity.speed * Time.deltaTime);
        else
            moveComponent.SetTarget((Vector2)transform.position + movementDirection);


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
