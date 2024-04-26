# Installation 
- Install Python Version 3.9.13
- Inside the root folder of the project open a command prompt
- Type: python -m venv venv
- Type: venv\scripts\activate
- Type: python -m pip install --upgrade pip
- Type: pip install mlagents
- Type: pip3 install torch torchvision torchaudio
- Type: pip install protobuf==3.20.3

# To Test
- Enter Reinforcement Learning scene in unity
- In the same command prompt enter mlagents-learn config/MoveToGoal.yaml --run-id=<TestName>
