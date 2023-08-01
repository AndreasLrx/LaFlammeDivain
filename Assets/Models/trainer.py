import numpy as np
from mlagents_envs.environment import UnityEnvironment, ActionTuple
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from RNNagent import RNNAgent


def main():
    # Create the Unity environment
    engine_configuration_channel = EngineConfigurationChannel()
    env = UnityEnvironment(file_name="build", side_channels=[engine_configuration_channel])

    # Set up the communication with Unity
    engine_configuration_channel.set_configuration_parameters(time_scale=1.0, quality_level=5)
    env.reset()

    try:
        behavior_names = [(name, name.split('?')[0]) for name in list(env.behavior_specs)]
        agents = {'EnemyFocus': RNNAgent(1, 4, 1)}
        for _, behavior_name in behavior_names:
            print("Train behavior: " + behavior_name + " = " + str(behavior_name in agents))
        while True:
            for behavior_name in behavior_names:
                if behavior_name[1] not in agents:
                    continue
                # Get observations from Unity
                decision_steps, _ = env.get_steps(behavior_name[0])
                
                actions = agents[behavior_name[1]].process_unity_observations(decision_steps.obs[0][0])
                env.set_actions(behavior_name[0], actions)
            env.step()

    finally:
        env.close()

if __name__ == "__main__":
    main()
