namespace IntelligentSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    internal static class PerceptronLearning
    {
        private static readonly double alpha = 1;

        private static readonly int learningSamplesCount = 15;

        public static IEnumerable<Cluster> PerformClustering(IList<DataItem> dataItems, IList<IList<double>> weightsVectors)
        {
            Guard.NotNull(dataItems, "dataItems");
            Guard.NotNull(weightsVectors, "weightsVectors");

            Dictionary<int, IList<DataItem>> clustersDictionary = new Dictionary<int, IList<DataItem>>();

            for (int i = 0; i < weightsVectors.Count; i++)
            {
                if (!clustersDictionary.ContainsKey(i))
                {
                    clustersDictionary.Add(i, new List<DataItem>());
                }
            }

            foreach (DataItem dataItem in PerceptronLearning.SupplementDataItems(dataItems))
            {
                double maxD = Double.MinValue;
                int clusterId = -1;

                for (int i = 0; i < weightsVectors.Count; i++)
                {
                    double d = weightsVectors[i].Select((elem, index) => elem * dataItem.Values[index])
                        .Sum();

                    if (maxD < d)
                    {
                        maxD = d;
                        clusterId = i;
                    }
                }

                if (!clustersDictionary.ContainsKey(clusterId))
                {
                    clustersDictionary.Add(clusterId, new List<DataItem>());
                }

                clustersDictionary[clusterId].Add(dataItem);
            }

            for (int i = 0; i < weightsVectors.Count; i++)
            {
                yield return new Cluster(clustersDictionary[i]);
            }
        }

        public static IList<IList<double>> PerformLearning(IList<DataItem> dataItems, int classesNum)
        {
            Guard.NotNull(dataItems, "dataItems");
            Guard.IntMoreThanZero(classesNum, "classesNum");

            int valuesDimension = dataItems.First()
                .Values.Length + 1;

            List<DataItem> newDataItems = PerceptronLearning.SupplementDataItems(dataItems)
                .ToList();

            List<IList<double>> weights = new List<IList<double>>(classesNum);

            for (int i = 0; i < classesNum; i++)
            {
                weights.Add(new double[valuesDimension]);
            }

            IEnumerable<DataItem> learningDataItems = newDataItems.GroupBy(item => item.ClassId,
                (key, groupedItems) => groupedItems.Take(PerceptronLearning.learningSamplesCount))
                .SelectMany(groupedItems => groupedItems.ToList());

            while (true)
            {
                List<IList<double>> oldWeights = weights.ToList();

                foreach (DataItem dataItem in learningDataItems)
                {
                    int classNum = dataItem.ClassId - 1;

                    double d = weights[classNum].Select((elem, index) => elem * dataItem.Values[index])
                        .Sum();

                    for (int i = 0; i < classesNum; i++)
                    {
                        if (i != classNum)
                        {
                            double d2 = weights[i].Select((elem, index) => elem * dataItem.Values[index])
                                .Sum();

                            if (d <= d2)
                            {
                                weights[classNum] =
                                    weights[classNum].Select(
                                        (elem, index) => elem + PerceptronLearning.alpha * dataItem.Values[index])
                                        .ToList();
                                weights[i] =
                                    weights[i].Select(
                                        (elem, index) => elem - PerceptronLearning.alpha * dataItem.Values[index])
                                        .ToList();

                                break;
                            }
                        }
                    }
                }

                bool weightsChanged = false;

                for (int i = 0; i < weights.Count; i++)
                {
                    if (!weights[i].SequenceEqual(oldWeights[i]))
                    {
                        weightsChanged = true;
                        break;
                    }
                }

                if (!weightsChanged)
                {
                    break;
                }
            }

            return weights;
        }

        private static IEnumerable<DataItem> SupplementDataItems(IList<DataItem> dataItems)
        {
            int valuesDimension = dataItems.First()
                .Values.Length + 1;

            foreach (DataItem dataItem in dataItems)
            {
                List<double> newValues = new List<double>(valuesDimension);

                newValues.Add(1);
                newValues.AddRange(dataItem.Values);

                yield return new DataItem(dataItem.ClassId, newValues.ToArray());
            }
        }
    }
}