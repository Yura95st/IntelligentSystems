namespace IntelligentSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    using MathNet.Numerics.LinearAlgebra;

    internal class BayesСlassifierWithUnknownDistribution
    {
        private static readonly int funcCount = 4;

        private static readonly VectorBuilder<double> vectorBuilder = Vector<double>.Build;

        public static IList<Vector<double>> BuildDecisionFunctions(IList<DataItem> dataItems)
        {
            Guard.NotNull(dataItems, "dataItems");

            List<List<DataItem>> grouppedDataItems =
                dataItems.GroupBy(item => item.ClassId, (key, groupedItems) => groupedItems.ToList())
                    .ToList();

            int[][] orthonormalFunctions = BayesСlassifierWithUnknownDistribution.GetOrthonormalFunctions();

            List<Vector<double>> coefficients = new List<Vector<double>>(grouppedDataItems.Count);

            foreach (List<DataItem> dataItemsGroup in grouppedDataItems)
            {
                Vector<double> coefficientsVector =
                    BayesСlassifierWithUnknownDistribution.vectorBuilder.Dense(
                        BayesСlassifierWithUnknownDistribution.funcCount);

                for (int i = 0; i < BayesСlassifierWithUnknownDistribution.funcCount; i++)
                {
                    coefficientsVector[i] = 0.5
                        * (dataItemsGroup.Select(
                            item =>
                                BayesСlassifierWithUnknownDistribution.ComputeOrthonormalFuncResult(orthonormalFunctions[i],
                                    item))
                            .Sum() / dataItemsGroup.Count) * orthonormalFunctions[i][i];
                }

                coefficients.Add(coefficientsVector);
            }

            return coefficients;
        }

        public static string GetFunctionStringFromDecisionFunction(IList<double> vector)
        {
            Guard.NotNull(vector, "vector");

            StringBuilder sb = new StringBuilder();
            sb.Append("d(x) = ");
            sb.Append(vector[0]);

            string[] xLiterals = { "x1", "x2", "x1x2" };

            for (int i = 1; i < vector.Count; i++)
            {
                sb.Append(string.Format(" {0} {1} * {2}", vector[i] > 0 ? "+" : "-", Math.Abs(vector[i]), xLiterals[i - 1]));
            }

            return sb.ToString();
        }

        private static double ComputeOrthonormalFuncResult(int[] orthonormalFuncCoefficients, DataItem dataItem)
        {
            return orthonormalFuncCoefficients[0] + orthonormalFuncCoefficients[1] * dataItem.Values[0]
                + orthonormalFuncCoefficients[2] * dataItem.Values[1]
                + orthonormalFuncCoefficients[3] * dataItem.Values[0] * dataItem.Values[1];
        }

        private static int[][] GetOrthonormalFunctions()
        {
            int[][] funcCoefficients = new int[BayesСlassifierWithUnknownDistribution.funcCount][];

            for (int i = 0; i < BayesСlassifierWithUnknownDistribution.funcCount; i++)
            {
                funcCoefficients[i] = new int[BayesСlassifierWithUnknownDistribution.funcCount];
            }

            funcCoefficients[0][0] = 1;
            funcCoefficients[1][1] = 2;
            funcCoefficients[2][2] = 2;
            funcCoefficients[3][3] = 4;

            return funcCoefficients;
        }
    }
}