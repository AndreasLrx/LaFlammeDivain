import tensorflow as tf
import numpy as np
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, LSTM, Masking
from mlagents_envs.environment import ActionTuple



class RNNAgent:
    def __init__(self, entry_size, max_entries, output_size):
        self.entry_size = entry_size
        self.max_entries = max_entries
        self.output_size = output_size
        # Define the RNN model
        self.model = Sequential()
        self.model.add(Masking(mask_value=0, input_shape=(max_entries, entry_size)))
        self.model.add(LSTM(64)) 
        self.model.add(Dense(output_size, activation='softmax'))
        # Compile the model
        self.model.compile(loss='sparse_categorical_crossentropy', optimizer='adam')

    def get_action(self, observations):
        action = self.model.predict(observations[None, :, :])  # Make the decision based on the current state
        action = tf.argmax(action, axis=-1)[0].numpy()  # Convert action tensor to an integer
        return self.model(self._float_list_to_tensor(observations))
    
    def learn(self, expert_observations, expert_action):
        observations_padded = tf.keras.preprocessing.sequence.pad_sequences(expert_observations, maxlen=self.max_entries)
        # Create a mask to handle variable-length sequences
        mask = (observations_padded != 0)
        print("Observations: ", observations_padded, ", mask: ", mask, ", actions: ", expert_action)
        self.model.fit(observations_padded[None, :, :], [expert_action, expert_action], epochs=1, batch_size=1)#, sample_weight=mask)

    def raw_observations_to_entries(self, observations):
        res = []
        for i in range(0, int(len(observations) / self.entry_size)):
            res.append(observations[i * self.entry_size:(i + 1) * self.entry_size])
        return res


    def process_unity_observations(self, observations):
        print("Observations: ", observations)
        inputs = int((len(observations) - self.output_size) / 2)
        print("Inputs: ", str(inputs))
        # No entries -> random action and don't affect the network
        if (inputs == 0):
            action = 0
        else:
            agent_observations = self.raw_observations_to_entries(observations[:inputs])
            expert_observations = self.raw_observations_to_entries(observations[inputs:inputs * 2])
            expert_action = observations[-self.output_size:]
            self.learn(expert_observations, expert_action)
            action = self.get_action(agent_observations)
        print("Output action: ", action)
        return ActionTuple(discrete=np.array([[action]]))
        