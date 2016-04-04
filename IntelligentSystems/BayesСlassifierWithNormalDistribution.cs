namespace IntelligentSystems
{
    using System.Collections.Generic;
    using System.Linq;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    using MathNet.Numerics.LinearAlgebra;

    internal class BayesСlassifierWithNormalDistribution
    {
        private static readonly MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;

        private static readonly VectorBuilder<double> vectorBuilder = Vector<double>.Build;

        public static IList<IList<double>> BuildDecisionFunctions(IList<DataItem> dataItems)
        {
            Guard.NotNull(dataItems, "dataItems");

            int valuesDimension = dataItems.First()
                .Values.Length;

            List<List<DataItem>> grouppedDataItems =
                dataItems.GroupBy(item => item.ClassId, (key, groupedItems) => groupedItems.ToList())
                    .ToList();

            List<Vector<double>> meanVectors = BayesСlassifierWithNormalDistribution.GetMeanVectors(grouppedDataItems);

            List<Matrix<double>> covarianceMatrices =
                BayesСlassifierWithNormalDistribution.GetCovarianceMatrices(grouppedDataItems, meanVectors, valuesDimension);

            List<Matrix<double>> inverseCovarianceMatrices = covarianceMatrices.Select(matrix => matrix.Inverse())
                .ToList();

            List<IList<double>> coefficients = BayesСlassifierWithNormalDistribution.GetCoefficientsVectors(
                grouppedDataItems, meanVectors, inverseCovarianceMatrices);

            return coefficients;
        }

        private static List<IList<double>> GetCoefficientsVectors(IList<List<DataItem>> grouppedDataItems,
                                                                  IList<Vector<double>> meanVectors,
                                                                  IList<Matrix<double>> inverseCovarianceMatrices)
        {
            List<IList<double>> coefficients = new List<IList<double>>(grouppedDataItems.Count);

            for (int i = 0; i < grouppedDataItems.Count; i++)
            {
                Vector<double> v = inverseCovarianceMatrices[i] * meanVectors[i];

                IList<double> coefficientsVector = new double[v.Count + 1];
                for (int j = 0; j < v.Count; j++)
                {
                    coefficientsVector[j + 1] = v[j];
                }

                coefficientsVector[0] = -0.5
                    * (meanVectors[i].ToRowMatrix() * inverseCovarianceMatrices[i] * meanVectors[i])[0];

                coefficients.Add(coefficientsVector);
            }

            return coefficients;
        }

        private static List<Matrix<double>> GetCovarianceMatrices(IList<List<DataItem>> grouppedDataItems,
                                                                  IList<Vector<double>> meanVectors, int valuesDimension)
        {
            List<Matrix<double>> covarianceMatrices = new List<Matrix<double>>(grouppedDataItems.Count);

            for (int i = 0; i < grouppedDataItems.Count; i++)
            {
                Matrix<double> covarianceMatrix = BayesСlassifierWithNormalDistribution.matrixBuilder.Dense(valuesDimension,
                    valuesDimension);

                List<DataItem> dataItemsGroup = grouppedDataItems[i];

                foreach (DataItem dataItem in dataItemsGroup)
                {
                    Vector<double> v1 = BayesСlassifierWithNormalDistribution.vectorBuilder.DenseOfEnumerable(dataItem.Values);

                    covarianceMatrix += v1.ToColumnMatrix() * v1.ToRowMatrix();
                }

                covarianceMatrix /= dataItemsGroup.Count;

                covarianceMatrix -= meanVectors[i].ToColumnMatrix() * meanVectors[i].ToRowMatrix();

                covarianceMatrices.Add(covarianceMatrix);
            }

            return covarianceMatrices;
        }

        private static List<Vector<double>> GetMeanVectors(IList<List<DataItem>> grouppedDataItems)
        {
            List<Vector<double>> meanVectors = new List<Vector<double>>(grouppedDataItems.Count);

            foreach (List<DataItem> dataItemsGroup in grouppedDataItems)
            {
                DataItem meanDataItem = dataItemsGroup.First();

                foreach (DataItem dataItem in dataItemsGroup.Skip(1))
                {
                    meanDataItem += dataItem;
                }

                meanDataItem /= dataItemsGroup.Count;

                meanVectors.Add(BayesСlassifierWithNormalDistribution.vectorBuilder.DenseOfArray(meanDataItem.Values));
            }

            return meanVectors;
        }
    }
}