namespace IntelligentSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    internal class BayesСlassifierWithNormalDistribution
    {
        public static IList<IList<double>> PerformLearning(IList<DataItem> dataItems)
        {
            Guard.NotNull(dataItems, "dataItems");

            int valuesDimension = dataItems.First()
                .Values.Length;

            List<List<DataItem>> grouppedDataItems =
                dataItems.GroupBy(item => item.ClassId, (key, groupedItems) => groupedItems.ToList())
                    .ToList();

            List<IList<double>> coefficients = new List<IList<double>>(grouppedDataItems.Count);

            for (int i = 0; i < grouppedDataItems.Count; i++)
            {
                coefficients.Add(new double[valuesDimension + 1]);
            }

            IList<IList<double>> meanVectors = BayesСlassifierWithNormalDistribution.GetMeanVectors(grouppedDataItems);

            IList<IList<IList<double>>> covarianceMatrices =
                BayesСlassifierWithNormalDistribution.GetCovarianceMatrices(grouppedDataItems, meanVectors);

            List<IList<IList<double>>> inverseCovarianceMatrices =
                covarianceMatrices.Select(BayesСlassifierWithNormalDistribution.GetInverseMatrix)
                    .ToList();

            for (int i = 0; i < grouppedDataItems.Count; i++)
            {
                IList<double> coefficient = coefficients[i];

                for (int j = 0; j < coefficient.Count - 1; j++)
                {
                    coefficient[j] = inverseCovarianceMatrices[i][j].Aggregate((x, y) => x * y) * meanVectors[i][j];
                }

                coefficient[coefficient.Count - 1] = -0.5
                    * BayesСlassifierWithNormalDistribution.MultiplyTransposedVectorOnMatrix(meanVectors[i],
                        inverseCovarianceMatrices[i])
                        .Select((elem, index) => elem * meanVectors[i][index])
                        .Sum();
            }

            return coefficients;
        }

        private static IList<IList<IList<double>>> GetCovarianceMatrices(IList<List<DataItem>> grouppedDataItems,
                                                                         IList<IList<double>> meanVectors)
        {
            List<IList<IList<double>>> covarianceMatrices = new List<IList<IList<double>>>(grouppedDataItems.Count);

            for (int i = 0; i < grouppedDataItems.Count; i++)
            {
                IList<IList<double>> covarianceMatrix = new List<IList<double>>();

                List<DataItem> dataItemsGroup = grouppedDataItems[i];

                foreach (DataItem dataItem in dataItemsGroup)
                {
                    covarianceMatrix = BayesСlassifierWithNormalDistribution.SumTwoMatrixes(covarianceMatrix,
                        BayesСlassifierWithNormalDistribution.MultiplyVectorOnTransposedVector(dataItem.Values,
                            dataItem.Values));
                }

                covarianceMatrix = BayesСlassifierWithNormalDistribution.MultiplyMatrixByNumber(covarianceMatrix,
                    dataItemsGroup.Count);

                covarianceMatrices.Add(BayesСlassifierWithNormalDistribution.SumTwoMatrixes(covarianceMatrix,
                    BayesСlassifierWithNormalDistribution.MultiplyMatrixByNumber(
                        BayesСlassifierWithNormalDistribution.MultiplyVectorOnTransposedVector(meanVectors[i], meanVectors[i]),
                        -1)));
            }

            return covarianceMatrices;
        }

        private static IList<IList<double>> GetInverseMatrix(IList<IList<double>> matrixToInverse)
        {
            throw new NotImplementedException();
        }

        private static IList<IList<double>> GetMeanVectors(IList<List<DataItem>> grouppedDataItems)
        {
            List<IList<double>> meanVectors = new List<IList<double>>(grouppedDataItems.Count);

            foreach (List<DataItem> dataItemsGroup in grouppedDataItems)
            {
                DataItem meanDataItem = dataItemsGroup.First();

                foreach (DataItem dataItem in dataItemsGroup.Skip(1))
                {
                    meanDataItem += dataItem;
                }

                meanDataItem /= dataItemsGroup.Count;

                meanVectors.Add(meanDataItem.Values);
            }

            return meanVectors;
        }

        private static IList<IList<double>> MultiplyMatrixByNumber(IList<IList<double>> matrix, double number)
        {
            List<IList<double>> result = new List<IList<double>>(matrix);

            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Count; j++)
                {
                    result[i][j] *= number;
                }
            }

            return result;
        }

        private static IList<double> MultiplyTransposedVectorOnMatrix(IList<double> vectorToTranspose,
                                                                      IList<IList<double>> matrix)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < matrix.First()
                .Count; i++)
            {
                result[i] = vectorToTranspose.Select((item, index) => item * matrix[index][i])
                    .Sum();
            }

            return result;
        }

        private static IList<IList<double>> MultiplyVectorOnTransposedVector(IList<double> vector,
                                                                             IList<double> vectorToTranspose)
        {
            List<IList<double>> result = new List<IList<double>>(vector.Count);

            foreach (double rowItem in vector)
            {
                List<double> row = new List<double>();

                foreach (double columnItem in vectorToTranspose)
                {
                    row.Add(rowItem * columnItem);
                }

                result.Add(row);
            }

            return result;
        }

        private static IList<IList<double>> SumTwoMatrixes(IList<IList<double>> m1, IList<IList<double>> m2)
        {
            List<IList<double>> result = new List<IList<double>>(m1);

            for (int i = 0; i < m2.Count; i++)
            {
                for (int j = 0; j < m2[i].Count; j++)
                {
                    result[i][j] += m2[i][j];
                }
            }

            return result;
        }
    }
}