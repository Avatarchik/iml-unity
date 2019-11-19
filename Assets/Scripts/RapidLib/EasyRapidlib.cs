﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices; 

namespace InteractML
{
    /// <summary>
    /// Higher level class that holds a model, its training set and methods to deal with both. 
    /// Use this if you want more functionalities than just a model
    /// </summary>
    public class EasyRapidlib
    {

        #region Variables

        /// <summary>
        /// Pointer to the rapidlib dll model
        /// </summary>
        RapidlibModel m_Model;

        /// <summary>
        /// Options of learning type in Rapidlib
        /// </summary>
        public enum LearningType { Classification, Regression, DTW };

        /// <summary>
        /// The specific learning type selected
        /// </summary>
        private LearningType m_LearningType;

        /// <summary>
        /// The list of training examples (for classification and regression)
        /// </summary>
        private List<RapidlibTrainingExample> m_TrainingExamples;

        /// <summary>
        /// List of different training examples series (for DTW)
        /// </summary>
        private List<RapidlibTrainingSerie> m_TrainingExamplesSeries;

        /// <summary>
        /// Returns the current model status
        /// </summary>
        private IMLSpecifications.ModelStatus m_ModelStatus;

        /// <summary>
        /// Is the model running?
        /// </summary>
        public bool Running { get { return m_ModelStatus == IMLSpecifications.ModelStatus.Running ? true : false; } }

        private bool collectData = false;
        /// <summary>
        /// Can the model collect data?
        /// </summary>
        public bool CollectingData { get { return collectData; } }

        /// <summary>
        /// Is the model training?
        /// </summary>
        public bool Training { get { return m_ModelStatus == IMLSpecifications.ModelStatus.Training ? true : false; } }
        /// <summary>
        /// Is the model trained?
        /// </summary>
        public bool Trained { get { return m_ModelStatus == IMLSpecifications.ModelStatus.Trained ? true : false; } }

        /// <summary>
        /// String containing the model in a JSON format
        /// </summary>
        public string jsonModelString = "";

        #endregion

        #region Constructors

        public EasyRapidlib()
        {
            // Init internal values
            Initialize();
        }

        public EasyRapidlib(LearningType learningType)
        {
            // Set the specified learning type
            m_LearningType = learningType;
            // Init internal values
            Initialize();
        }

        public EasyRapidlib(LearningType learningType, string dataPathModel)
        {
            // Set the specified learning type
            m_LearningType = learningType;
            // Init internal values
            Initialize();
            // We attempt to load the model 
            LoadModelFromDisk(dataPathModel);

        }

        public EasyRapidlib(LearningType learningType, string dataPathModel, string dataPathTrainingSet)
        {
            // Set the specified learning type
            m_LearningType = learningType;
            // Init internal values
            Initialize();
            // We attempt to load the training examples
            LoadModelAndTrainingDataFromDisk(dataPathModel, dataPathTrainingSet);

        }

        #endregion

        #region Public Methods

        /* TRAINING AND RUNNING METHODS */

        public bool TrainModel()
        {
            bool isTrained = false;
            // Will train differently depending on the learning type
            switch (m_LearningType)
            {
                case LearningType.Classification:
                    isTrained = m_Model.Train(m_TrainingExamples);
                    break;
                case LearningType.Regression:
                    isTrained = m_Model.Train(m_TrainingExamples);
                    break;
                case LearningType.DTW:
                    isTrained = m_Model.Train(m_TrainingExamplesSeries);
                    break;
                default:
                    break;
            }
            return isTrained;
        }

        /* SAVING TO DISK METHODS*/

        /// <summary>
        /// Saves the internal model to disk with the specified name (folderPath configured in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveModelToDisk(string fileName)
        {
            m_Model.SaveModelToDisk(fileName);
        }

        /// <summary>
        /// Saves the interal training examples with the specified name (folderPath configured in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveTrainingExamplesToDisk(string fileName)
        {
            IMLDataSerialization.SaveTrainingSetToDiskRapidlib(m_TrainingExamples, fileName);
        }

        /// <summary>
        /// Saves the internal training series with the specified name (folderPath configured in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveTrainingSeriesToDisk(string fileName)
        {
            IMLDataSerialization.SaveTrainingSeriesSetsToDiskRapidlib(m_TrainingExamplesSeries, fileName);
        }

        /* LOADING FROM DISK METHODS */

        /// <summary>
        /// (folderPath specified in IMLDataserialization) Loads both model and training set from disk (will only load the ones that have a name)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingExamplesList"></param>
        /// <param name="fileNameModel"></param>
        /// <param name="fileNameTrainingExamples"></param>
        /// <returns>True if managed to load everything, false otherwise</returns>
        public bool LoadModelAndTrainingDataFromDisk(string fileNameModel, string fileNameTrainingExamples)
        {
            bool successLoadingAll = true;
            // Only load data if we have the datapaths
            if (!String.IsNullOrEmpty(fileNameModel))
                successLoadingAll = LoadModelFromDisk(fileNameModel);

            if (!String.IsNullOrEmpty(fileNameTrainingExamples))
                successLoadingAll = LoadTrainigExamplesFromDisk(fileNameTrainingExamples);

            // Succesfully loaded everything?
            return successLoadingAll;

        }

        /// <summary>
        /// Loads a model with the specified fileName into EasyRapidlib's model
        /// (folderPath specified in IMLDataserialization)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool LoadModelFromDisk(string fileName)
        {
            bool isLoaded = false;

            m_Model = LoadModelFromDiskPrivate(fileName);

            if (m_Model.TypeOfModel != RapidlibModel.ModelType.None)
            {
                isLoaded = true;
            }

            return isLoaded;
        }

        /// <summary>
        /// Loads the training examples in the specified fileName into EasyRapidlib's training examples list
        /// (folderPath specified in IMLDataserialization)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool LoadTrainigExamplesFromDisk(string fileName)
        {
            bool isLoaded = false;

            m_TrainingExamples = LoadTrainingSetFromDiskPrivate(fileName);

            return isLoaded;
        }

        /// <summary>
        /// Loads the training series with the specific fileName into EasyRapidlib's internal training series list
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool LoadTrainingSeriesFromDisk(string fileName)
        {
            bool isLoaded = false;

            m_TrainingExamplesSeries = IMLDataSerialization.LoadTrainingSeriesSetsFromDiskRapidlib(fileName);

            return isLoaded;

        }

        /// <summary>
        /// Overrides the current model with the specified type
        /// </summary>
        /// <param name="type"></param>
        public void OverrideModel(LearningType type)
        {
            m_Model = CreateRapidlibModel(type);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialises rapidlib class to default values
        /// </summary>
        private void Initialize()
        {
            // Set the model status to untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;

            // Code to initialize model 

            m_Model = CreateRapidlibModel(m_LearningType);

            //Debug.Log("Training Examples list Count: " + trainingExamples.Count);

        }

        private RapidlibModel CreateClassificationModel()
        {
            return new RapidlibModel(RapidlibModel.ModelType.kNN); ;
        }

        private RapidlibModel CreateRegressionModel()
        {
            return new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
        }

        private RapidlibModel CreateTimeSeriesClassificationModel()
        {
            return new RapidlibModel(RapidlibModel.ModelType.DTW);
        }

        /// <summary>
        /// Creates a rapidlib model based on the learning type specified
        /// </summary>
        /// <param name="learningType"></param>
        /// <returns></returns>
        private RapidlibModel CreateRapidlibModel(LearningType learningType)
        {
            RapidlibModel tempModel;

            // We create a different model depending on what kind of learning type is selected
            switch (learningType)
            {
                case LearningType.Classification:
                    tempModel = CreateClassificationModel(); ;
                    break;
                case LearningType.Regression:
                    tempModel = CreateRegressionModel(); ;
                    break;
                case LearningType.DTW:
                    tempModel = CreateTimeSeriesClassificationModel(); ;
                    break;
                default:
                    throw new Exception("Error: Unkown learning type in RapidlibClass");
                    break;
            }

            return tempModel;

        }

        /* === Model IO Methods === */

        /// <summary>
        /// Saves the model to disk
        /// </summary>
        /// <param name="modelJSON">The model in JSON format</param>
        /// <param name="fileName">The name to give the file (filePath configured in IMLDataSerialization)</param>
        private void SaveModelToDiskPrivate(string modelJSON, string fileName)
        {
            IMLDataSerialization.SaveRapidlibModelToDisk(modelJSON, fileName);
        }

        /// <summary>
        /// Loads a model from disk with the specified name (filePath is configured in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName">The name of the file (file path configured in IMLDataSerialization)</param>
        /// <returns>Returns a model </returns>
        private RapidlibModel LoadModelFromDiskPrivate(string fileName)
        {
            // Creates a model and loads from the path
            RapidlibModel auxModel = new RapidlibModel();
            auxModel.LoadModelFromDisk(fileName);
            // Returns the loaded model
            return auxModel;
        }

        /* === Training Examples IO Methods === */

        /// <summary>
        /// Saves Training Data Set to disk
        /// </summary>
        /// <param name="listToSave">The list of training examples</param>
        /// <param name="fileName">File name (file path configured in IMLDataSerialization)</param>
        private void SaveTrainingSetToDisk(List<RapidlibTrainingExample> listToSave, string fileName)
        {
            IMLDataSerialization.SaveTrainingSetToDiskRapidlib(listToSave, fileName);
        }

        /// <summary>
        /// Loads Training Data Set from Disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Returns a list with training set</returns>
        private List<RapidlibTrainingExample> LoadTrainingSetFromDiskPrivate(string fileName)
        {
            return IMLDataSerialization.LoadTrainingSetFromDiskRapidlib(fileName);
        }

        #endregion

    }

}
