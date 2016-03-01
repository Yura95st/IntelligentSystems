namespace IntelligentSystems.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IntelligentSystems.Model;

    internal static class ClusteringUtils
    {
        public static int GetClusteringErrorsNumber(Cluster cluster)
        {
            int wrongItemsCount = cluster.Items.Count() - cluster.Items.GroupBy(item => item.ClassId)
                .Select(groupedItems => groupedItems.Count())
                .Max();

            return wrongItemsCount;
        }

        public static double GetClusteringQuality(IEnumerable<Cluster> clusters)
        {
            Guard.NotNull(clusters, "clusters");

            int totalItems = clusters.Select(cluster => cluster.Items.Count())
                .Sum();

            int errors = clusters.Select(ClusteringUtils.GetClusteringErrorsNumber)
                .Sum();

            double quality = 1 - ((double)errors / totalItems);

            return quality;
        }

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