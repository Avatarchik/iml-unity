﻿using System;
using System.Collections.Generic;

namespace InteractML
{
    /// <summary>
    /// Holds address and configuration for a rapidlibmodel in memory. 
    /// Very basic and clean. Use if you are implementing training logic yourself
    /// </summary>
    public class RapidlibModel
    {
        #region Variables

        IntPtr m_ModelAddress;
        /// <summary>
        /// The address in memory of the rapidlib model (used to interface with the rapidlib DLL)
        /// </summary>
        public IntPtr ModelAddress { get => m_ModelAddress; }

        string m_ModelJSONString;
        /// <summary>
        /// The json string containing all info of a model
        /// </summary>
        public string ModelJSONString { get => m_ModelJSONString; }

        /// <summary>
        /// Different types of model available
        /// </summary>
        public enum ModelType { kNN, NeuralNetwork, DTW, None }

        private ModelType m_TypeOfModel;
        /// <summary>
        /// Which model is this one?
        /// </summary>
        public ModelType TypeOfModel { get => m_TypeOfModel; set => m_TypeOfModel = value; }

        private IMLSpecifications.ModelStatus m_ModelStatus;
        /// <summary>
        /// The current status of the model (running, trained, training)
        /// </summary>
        public IMLSpecifications.ModelStatus ModelStatus { get => m_ModelStatus; }


        #endregion

        #region Constructor

        /// <summary>
        /// Creates an empty instance without any model
        /// </summary>
        public RapidlibModel()
        {
            // Set default values
            m_ModelAddress = (IntPtr)0;
            m_ModelJSONString = "";
            m_TypeOfModel = ModelType.None;
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;

        }

        /// <summary>
        /// Creates a specific model type
        /// </summary>
        /// <param name="modelToCreate"></param>
        public RapidlibModel(ModelType modelToCreate)
        {
            // Set default values
            m_ModelAddress = (IntPtr)0;
            m_ModelJSONString = "";
            m_TypeOfModel = modelToCreate;
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;

            // Creates the specific model type
            switch (modelToCreate)
            {
                case ModelType.kNN:
                    CreateClassificationModel();
                    break;
                case ModelType.NeuralNetwork:
                    CreateRegressionModel();
                    break;
                case ModelType.DTW:
                    CreateTimeSeriesClassificationModel();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Destructor

        ~RapidlibModel()
        {
            // Make sure to destroy the model when the class is collected by the GC
            DestroyModel();
        }

        #endregion

        #region Public Methods

        /* MODEL IML LOGIC */

        /// <summary>
        /// Trains the model with a list of training examples (classification or regression)
        /// </summary>
        /// <param name="trainingExamples">True if training succeeded</param>
        public bool Train(List<RapidlibTrainingExample> trainingExamples)
        {
            bool isTrained = false;
            IntPtr trainingSetAddress = (IntPtr)0;
            // Only allow training of classification and regression (because of the format of the training data)
            switch (m_TypeOfModel)
            {
                case ModelType.kNN:
                    trainingSetAddress = TransformTrainingExamplesForRapidlib(trainingExamples);
                    isTrained = RapidlibLinkerDLL.TrainClassification(m_ModelAddress, trainingSetAddress);
                    break;
                case ModelType.NeuralNetwork:
                    trainingSetAddress = TransformTrainingExamplesForRapidlib(trainingExamples);
                    isTrained = RapidlibLinkerDLL.TrainRegression(m_ModelAddress, trainingSetAddress);
                    break;
                case ModelType.DTW:
                    throw new Exception("A list of training series is required to train a DTW model!");
                case ModelType.None:
                    throw new Exception("You can't train on an unitialised model!");
                default:
                    break;
            }

            if (isTrained)
                m_ModelStatus = IMLSpecifications.ModelStatus.Trained;

            // Once the training is done, we need to destroy the c++ training list outside of the GC scope
            RapidlibLinkerDLL.DestroyTrainingSet(trainingSetAddress);

            return isTrained;
        }

        /// <summary>
        /// Trains the model with a list of training series (DTW)
        /// </summary>
        /// <param name="trainingSeries"></param>
        /// <returns>True if training succeeded</returns>
        public bool Train(List<RapidlibTrainingSerie> trainingSeries)
        {
            bool isTrained = false;
            // Only allow training of DTW (because of the format of the training data)
            switch (m_TypeOfModel)
            {
                case ModelType.kNN:
                    throw new Exception("A list of training examples, not series is required to train a classification model!");
                case ModelType.NeuralNetwork:
                    throw new Exception("A list of training examples, not series is required to train a regression model!");
                case ModelType.DTW:
                    // TO DO 
                    // Implement DTW training functionality
                    break;
                case ModelType.None:
                    throw new Exception("You can't train on an unitialised model!");
                default:
                    break;
            }

            if (isTrained)
                m_ModelStatus = IMLSpecifications.ModelStatus.Trained;

            // Once the training is done, we need to destroy the c++ training list outside of the GC scope


            return isTrained;
        }

        /* MODEL CONFIGURATION */

        /// <summary>
        /// Destroys the model we have a reference for and set the address to none
        /// </summary>
        public void DestroyModel()
        {
            // If we have an address to destroy...
            if ((int)m_ModelAddress != 0)
            {
                // If our model is dtw, we need to call the specific destructor
                if (m_TypeOfModel == ModelType.DTW)
                {
                    RapidlibLinkerDLL.DestroySeriesClassificationModel(m_ModelAddress);
                }
                // If our model is either classification or regression, we call the other generic destructor
                else
                {
                    RapidlibLinkerDLL.DestroyModel(m_ModelAddress);
                }
            }
            // We set the pointer to none
            m_ModelAddress = (IntPtr)0;
            // We set the json string to nothing
            m_ModelJSONString = "";
            // We set the model type to none
            m_TypeOfModel = ModelType.None;
            // We set the training status to untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
        }

        /// <summary>
        /// Configures the model with data from the json string
        /// </summary>
        /// <param name="jsonstring"></param>
        public void ConfigureModelWithJson(string jsonstring)
        {
            if (!String.IsNullOrEmpty(jsonstring))
            {
                // Configure the model in memory
                RapidlibLinkerDLL.PutJSON(m_ModelAddress, jsonstring);
                // Set the status to trained (since we assume the model we loaded was trained)
                m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
            }
        }

        /* I/O FOR MODEL DATA */

        /// <summary>
        /// Saves the current model with the desired fileName (filePath configured in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveModelToDisk(string fileName)
        {
            // Transforms the model into a stylised json string
            m_ModelJSONString = RapidlibLinkerDLL.GetJSON(m_ModelAddress);
            // Saves the model as a stylised json with the specified fileName
            IMLDataSerialization.SaveRapidlibModelToDisk(m_ModelJSONString, fileName);
        }

        /// <summary>
        /// Loads the model with the specified name into this instance
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadModelFromDisk(string fileName)
        {
            // Attempt to load model
            string stringLoaded = IMLDataSerialization.LoadRapidlibModelFromDisk(fileName);

            // If we loaded something...
            if (!String.IsNullOrEmpty(stringLoaded))
            {
                // We configure our model with the configuration loaded
                ConfigureModelWithJson(stringLoaded);
                // We store the loaded string 
                m_ModelJSONString = stringLoaded;
            }
            // If we couldn't load anything, throw exception
            else
            {
                throw new Exception("Couldn't load rapidlib model because nothing was found in provided path: " + fileName);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a classification model in rapidlib and holds its memory address
        /// </summary>
        public void CreateClassificationModel()
        {
            // We first make sure that we don't have any model in memory...
            DestroyModel();
            // We create the new model in memory and get its address
            m_ModelAddress = RapidlibLinkerDLL.CreateClassificationModel();
            // We set the type of model to kNN
            m_TypeOfModel = ModelType.kNN;
            // Since it is a new model, the status of the model is now untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
        }

        /// <summary>
        /// Creates a regression model in rapidlib and holds its memory address
        /// </summary>
        public void CreateRegressionModel()
        {
            // We first make sure that we don't have any model in memory...
            DestroyModel();
            // We create the new model in memory and get its address
            m_ModelAddress = RapidlibLinkerDLL.CreateRegressionModel();
            // We set the type of model to kNN
            m_TypeOfModel = ModelType.NeuralNetwork;
            // Since it is a new model, the status of the model is now untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
        }

        /// <summary>
        /// Creates a timeSeriesClassification (DTW) model in rapidlib and holds its memory address
        /// </summary>
        public void CreateTimeSeriesClassificationModel()
        {
            // We first make sure that we don't have any model in memory...
            DestroyModel();
            // We create the new model in memory and get its address
            m_ModelAddress = RapidlibLinkerDLL.CreateSeriesClassificationModel();
            // We set the type of model to kNN
            m_TypeOfModel = ModelType.DTW;
            // Since it is a new model, the status of the model is now untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
        }

        /// <summary>
        /// Transforms the provided C# training examples list into a format suitable for the Rapidlib dll
        /// </summary>
        /// <param name="trainingExamplesToTransform"></param>
        /// <returns>The memory address to the newly created and configured training set</returns>
        private IntPtr TransformTrainingExamplesForRapidlib(List<RapidlibTrainingExample> trainingExamplesToTransform)
        {
            // Get the memory address for an empty c++ training set
            IntPtr rapidlibTrainingSetAddress = RapidlibLinkerDLL.CreateTrainingSet();
            // Configure the new training set in memory with the C# list of examples
            foreach (RapidlibTrainingExample example in trainingExamplesToTransform)
            {
                RapidlibLinkerDLL.AddTrainingExample(rapidlibTrainingSetAddress,
                    example.Input, example.Output.Length,
                    example.Output, example.Output.Length);
            }
            // Return the address for the training set in memory
            return rapidlibTrainingSetAddress;
        }

        #endregion
    }
}
