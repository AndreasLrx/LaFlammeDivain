import numpy as np
from mlagents_envs.environment import UnityEnvironment, ActionTuple
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from agent import Agent

agent = Agent()

# Create the Unity environment
engine_configuration_channel = EngineConfigurationChannel()
env = UnityEnvironment(file_name="build", side_channels=[engine_configuration_channel])

# Set up the communication with Unity
engine_configuration_channel.set_configuration_parameters(time_scale=1.0, quality_level=5)
env.reset()

behavior_name = list(env.behavior_specs)[0]
spec = env.behavior_specs[behavior_name]

def split_observations(observations):
   agent_observations = observations[:12]
   expert_observations = observations[12:24]
   expert_actions = observations[24:26]
   return agent_observations, expert_observations, expert_actions

# Run the main loop
try:
    while True:
        # Get observations from Unity
        decision_steps, terminal_steps = env.get_steps(behavior_name)
        agent_observations, expert_observations, expert_actions = split_observations(decision_steps.obs[0][0])
        print("Agent observations: %s\nExpert observations: %s\nExpert actions: %s" % (agent_observations, expert_observations, expert_actions))

        agent.learn_from_expert(expert_observations, expert_actions)
        actions = agent.get_actions(agent_observations).detach().numpy()

        print("Agent Actions %s" % actions)

        # Send actions back to Unity
        newActs = ActionTuple(continuous=actions)
        env.set_actions(behavior_name, newActs)
        env.step()

finally:
    env.close()
