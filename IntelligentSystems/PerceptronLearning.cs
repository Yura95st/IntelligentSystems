namespace IntelligentSystems
{
    using System.Collections.Generic;
    using System.Linq;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    internal static class PerceptronLearning
    {
        private static readonly double alpha = 0.5;
        private static readonly int learningSamplesCount = 10;

        public static IEnumerable<IList<double>> PerformLearning(IList<DataItem> dataItems, int classesNum)
        {
            Guard.NotNull(dataItems, "dataItems");
            Guard.IntMoreThanZero(classesNum, "classesNum");

            int valuesDimension = dataItems.First()
                .Values.Length + 1;

            List<DataItem> newDataItems = new List<DataItem>(dataItems.Count);

            foreach (DataItem dataItem in dataItems)
            {
                List<double> newValues = new List<double>(valuesDimension);

                newValues.Add(1);
                newValues.AddRange(dataItem.Values);

                newDataItems.Add(new DataItem(dataItem.ClassId, newValues.ToArray()));
            }

            List<IList<double>> weights = new List<IList<double>>(classesNum);

            for (int i = 0; i < classesNum; i++)
            {
                weights.Add(new double[valuesDimension]);
            }

            while (true)
            {
                List<IList<double>> oldWeights = weights.ToList();

                foreach (
                    DataItem dataItem in
                        newDataItems.GroupBy(item => item.ClassId, (key, groupedItems) => groupedItems.Take(learningSamplesCount))
                            .SelectMany(groupedItems => groupedItems.ToList()))
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

                            if (d >= d2)
                            {
                                weights[i] =
                                    weights[i].Select(
                                        (elem, index) => elem + PerceptronLearning.alpha * dataItem.Values[index])
                                        .ToList();
                                weights[classNum] =
                                    weights[classNum].Select(
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
    }
}