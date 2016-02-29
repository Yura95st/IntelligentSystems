namespace IntelligentSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    internal static class CMeansClustering
    {
        private static readonly double alpha = 0.005;

        public static IEnumerable<Cluster> PerformClustering(IList<DataItem> dataItems, int clustersNum)
        {
            Guard.NotNull(dataItems, "dataItems");
            Guard.IntMoreThanZero(clustersNum, "clustersNum");

            List<DataItem> clusterCenters = new List<DataItem>();
            Dictionary<int, IList<DataItem>> clustersDictionary = new Dictionary<int, IList<DataItem>>();

            for (int i = 0; i < clustersNum; i++)
            {
                clusterCenters.Add(dataItems[i]);
            }

            while (true)
            {
                clustersDictionary.Clear();

                foreach (DataItem dataItem in dataItems)
                {
                    double minDistance = Double.MaxValue;
                    int clusterId = -1;

                    for (int i = 0; i < clusterCenters.Count; i++)
                    {
                        double distance = ClusteringUtils.GetEuclideanDistance(clusterCenters[i], dataItem);

                        if (minDistance > distance)
                        {
                            minDistance = distance;
                            clusterId = i;
                        }
                    }

                    if (!clustersDictionary.ContainsKey(clusterId))
                    {
                        clustersDictionary.Add(clusterId, new List<DataItem>());
                    }

                    clustersDictionary[clusterId].Add(dataItem);
                }

                List<DataItem> newClusterCenters = new List<DataItem>();

                foreach (KeyValuePair<int, IList<DataItem>> cluster in clustersDictionary.OrderBy(kvp => kvp.Key))
                {
                    DataItem newClusterCenter = cluster.Value.First();
                    foreach (DataItem dataItem in cluster.Value.Skip(1))
                    {
                        newClusterCenter += dataItem;
                    }
                    newClusterCenter *= 1.0 / cluster.Value.Count;

                    newClusterCenters.Add(newClusterCenter);
                }

                bool centersChanged = false;
                for (int i = 0; i < clusterCenters.Count; i++)
                {
                    if (ClusteringUtils.GetEuclideanDistance(clusterCenters[i], newClusterCenters[i])
                        > CMeansClustering.alpha)
                    {
                        centersChanged = true;
                        break;
                    }
                }

                if (!centersChanged)
                {
                    break;
                }

                clusterCenters = newClusterCenters;
            }

            for (int i = 0; i < clusterCenters.Count; i++)
            {
                yield return new Cluster(clusterCenters[i], clustersDictionary[i]);
            }
        }
    }
}