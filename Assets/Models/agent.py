import torch
import torch.nn as nn
import torch.optim as optim
from typing import List

# Define the neural network architecture
class NeuralNetwork(nn.Module):
    def __init__(self):
        super(NeuralNetwork, self).__init__()
        self.fc1 = nn.Linear(12, 64)
        self.fc2 = nn.Linear(64, 64)
        self.fc3 = nn.Linear(64, 2)
        self.tanh = nn.Tanh()

    def forward(self, x):
        x = self.tanh(self.fc1(x))
        x = self.tanh(self.fc2(x))
        x = self.tanh(self.fc3(x))
        return x
    
class Agent:
    def __init__(self):
        self.model = NeuralNetwork()
        self.criterion = nn.MSELoss()
        self.optimizer = optim.Adam(self.model.parameters())

    def _float_list_to_tensor(self, list: List[float]) -> torch.Tensor:
        return torch.tensor([list], dtype=torch.float32)

    def learn_from_expert(self, expert_observations: List[float], expert_actions: List[float]):
        outputs = self.get_actions(expert_observations)
        loss = self.criterion(outputs, self._float_list_to_tensor(expert_actions))

        # Backward pass and optimization
        self.optimizer.zero_grad()
        loss.backward()
        self.optimizer.step()

    def get_actions(self, observations: List[float]) -> torch.Tensor:
        return self.model(self._float_list_to_tensor(observations))
