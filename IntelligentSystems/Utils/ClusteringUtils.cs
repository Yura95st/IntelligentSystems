namespace IntelligentSystems.Utils
{
    using System;

    using IntelligentSystems.Model;

    internal static class ClusteringUtils
    {
        public static double GetEuclideanDistance(DataItem d1, DataItem d2)
        {
            Guard.NotNull(d1, "d1");
            Guard.NotNull(d2, "d2");

            double sum = 0;

            for (int i = 0; i < d1.Values.Length; i++)
            {
                sum += Math.Pow(d1.Values[i] - d2.Values[i], 2);
            }

            return Math.Sqrt(sum);
        }
    }
}