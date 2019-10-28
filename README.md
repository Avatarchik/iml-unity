# InteractML, an Interactive Machine Learning (IML) framework for Unity3D

<p align="center">
  <img src="https://github.com/Interactml/iml-unity/blob/master/Docs/Imgs/screenshot_only_graph_crop.PNG">
</p>

InteractML is an Unity3d Plugin that enables developers to configure, train, and use IML systems within the game editor. Using a visual node scripting system, developers can visualise incoming sensor data, configure game inputs (e.g., specifying what data to extract from sensors or objects in the game); train and refine ML models (by iteratively adding new training examples in realtime); and connect the ML model outputs (the real-time predictions calculated based on the training data) to other objects/scripts in the game scene. In addition, since InteractML doesn't rely on external software, the ML models can be trained and/or refined by player-provided examples in the released version of the game.

### Key features
* Lightweight machine learning models (seconds to train)
* Node-based interface
* You can code your own nodes to satisfy needs not currenly covered
* Integration with any kind of sensor data
* Supported from Unity 5.3 and up

### Limitations
* Windows/Mac compatibility (Mac only supports classification at the moment)
* Alpha stage

