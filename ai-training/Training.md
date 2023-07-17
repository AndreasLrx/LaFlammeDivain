# How to train new models ?

To train the an AI using unity ml-agents package, we need to setup a python environnment with the mlagents package.

For more complete informations, look at this [tutorial](https://www.youtube.com/watch?v=zPFU30tbyKs).

## Setup environnment

1. Install python 3.8


2. Create a new python environment and activate it. 

    ```sh
    python38 -m venv venv
    venv\Scripts\activate
    ```
    Remember how to activate the venv, it is required each time to train the models.
    **From now all commands must be made with the venv activated.**

3. Make sure pip is up to date

    ```sh
    python -m pip install --upgrade pip
    ```

4. Install the required packages

    ```sh
    python -m pip install -r requirements.txt
    ```

## Training

To start training a model, you need to start a python server and then hit play in unity.

```sh
mlagents-learn --run-id=ProceduralRooms
```

To resume a training simply add `--resume` and to overwrite it add `--force`.

### Learning evolution
You can start [tensorboard dashboard](http://localhost:6006/) to see the agents learning evolution:

```sh
$ tensorboard --logdir results
Serving TensorBoard on localhost; to expose to the network, use a proxy or pass --bind_all
TensorBoard 2.13.0 at http://localhost:6006/ (Press CTRL+C to quit)
```
