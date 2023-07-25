using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class Behavior : Agent
{
    public bool isTraining;
    private AICompanion _aiCompanion;
    protected AICompanion aiCompanion { get { return _aiCompanion; } }

    protected virtual void Awake()
    {
        _aiCompanion = GetComponentInParent<AICompanion>();
    }

    protected virtual void Start()
    {
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

    public override void CollectObservations(VectorSensor sensor) { }

    public override void OnActionReceived(ActionBuffers actionBuffers) { }

    protected virtual bool CheckEndEpisodeCondition()
    {
        return false;
    }
    public override void OnEpisodeBegin()
    {
        if (isTraining)
        {
            aiCompanion.roomGenerator.Regenerate();
            PlayerController.Instance.transform.position = aiCompanion.roomGenerator.RandomPosition();
            transform.position = aiCompanion.roomGenerator.RandomPosition();
        }
        AICompanion.Instance.target = null;
    }
}
