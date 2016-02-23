namespace IntellectualSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntellectualSystems.Model;
    using IntellectualSystems.Utils;

    internal static class MaxMinClustering
    {
        public static IEnumerable<Cluster> PerformClustering(IEnumerable<DataItem> dataItems)
        {
            Guard.NotNull(dataItems, "dataItems");

            List<DataItem> nonClusteredItems = dataItems.ToList();

            List<DataItem> clusterCenters = new List<DataItem>();

            clusterCenters.Add(nonClusteredItems[0]);
            nonClusteredItems.Remove(nonClusteredItems[0]);

            double maxDistance = 0;
            DataItem secondCenter = null;

            foreach (DataItem dataItem in nonClusteredItems)
            {
                double distance = ClusteringUtils.GetEuclideanDistance(clusterCenters[0], dataItem);

                if (maxDistance < distance)
                {
                    maxDistance = distance;
                    secondCenter = dataItem;
                }
            }

            clusterCenters.Add(secondCenter);
            nonClusteredItems.Remove(secondCenter);

            while (nonClusteredItems.Any())
            {
                Dictionary<DataItem, List<double>> distancesDictionary = new Dictionary<DataItem, List<double>>();

                foreach (DataItem irisItem in nonClusteredItems)
                {
                    List<double> distances = new List<double>();

                    foreach (DataItem clustersCenter in clusterCenters)
                    {
                        distances.Add(ClusteringUtils.GetEuclideanDistance(clustersCenter, irisItem));
                    }

                    distancesDictionary.Add(irisItem, distances);
                }

                List<KeyValuePair<DataItem, double>> minDistances = new List<KeyValuePair<DataItem, double>>();

                foreach (KeyValuePair<DataItem, List<double>> keyValuePair in distancesDictionary)
                {
                    minDistances.Add(new KeyValuePair<DataItem, double>(keyValuePair.Key, keyValuePair.Value.Min()));
                }

                KeyValuePair<DataItem, double> potentialCenter = minDistances.OrderByDescending(i => i.Value)
                    .First();

                if (!MaxMinClustering.IsGreaterThanTypicalDistance(potentialCenter.Value, clusterCenters))
                {
                    break;
                }

                clusterCenters.Add(potentialCenter.Key);
                nonClusteredItems.Remove(potentialCenter.Key);
            }

            return MaxMinClustering.BuildClusters(clusterCenters, nonClusteredItems);
        }

        private static IEnumerable<Cluster> BuildClusters(IList<DataItem> clusterCenters, IList<DataItem> dataItems)
        {
            Dictionary<int, IList<DataItem>> clustersDictionary = new Dictionary<int, IList<DataItem>>();

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

            for (int i = 0; i < clusterCenters.Count; i++)
            {
                yield return new Cluster(clusterCenters[i], clustersDictionary[i]);
            }
        }

        private static bool IsGreaterThanTypicalDistance(double distance, IList<DataItem> clusterCenters)
        {
            double distancesSum = 0;
            for (int i = 0; i < clusterCenters.Count; i++)
            {
                for (int j = i + 1; j < clusterCenters.Count; j++)
                {
                    distancesSum += ClusteringUtils.GetEuclideanDistance(clusterCenters[i], clusterCenters[j]);
                }
            }

            return distance > 0.5 * distancesSum / clusterCenters.Count;
        }
    }
}