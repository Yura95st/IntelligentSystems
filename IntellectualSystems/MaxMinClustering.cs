namespace IntellectualSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntellectualSystems.Model;
    using IntellectualSystems.Utils;

    internal static class MaxMinClustering
    {
        public static IEnumerable<Cluster> PerformClustering(IEnumerable<IrisItem> items)
        {
            Guard.NotNull(items, "items");

            List<IrisItem> irisItems = items.ToList();

            List<IrisItem> clustersCenters = new List<IrisItem>();

            clustersCenters.Add(irisItems[0]);
            irisItems.Remove(irisItems[0]);

            double maxDistance = 0;
            IrisItem secondCenter = null;

            foreach (IrisItem irisItem in irisItems)
            {
                double distance = MaxMinClustering.GetEuclideanDistance(clustersCenters[0], irisItem);

                if (maxDistance < distance)
                {
                    maxDistance = distance;
                    secondCenter = irisItem;
                }
            }

            clustersCenters.Add(secondCenter);
            irisItems.Remove(secondCenter);

            while (irisItems.Any())
            {
                Dictionary<IrisItem, List<double>> distancesDictionary = new Dictionary<IrisItem, List<double>>();

                foreach (IrisItem irisItem in irisItems)
                {
                    List<double> distances = new List<double>();

                    foreach (IrisItem clustersCenter in clustersCenters)
                    {
                        distances.Add(MaxMinClustering.GetEuclideanDistance(clustersCenter, irisItem));
                    }

                    distancesDictionary.Add(irisItem, distances);
                }

                List<KeyValuePair<IrisItem, double>> minDistances = new List<KeyValuePair<IrisItem, double>>();

                foreach (KeyValuePair<IrisItem, List<double>> keyValuePair in distancesDictionary)
                {
                    minDistances.Add(new KeyValuePair<IrisItem, double>(keyValuePair.Key, keyValuePair.Value.Min()));
                }

                KeyValuePair<IrisItem, double> potentialCenter = minDistances.OrderByDescending(i => i.Value)
                    .First();

                if (MaxMinClustering.IsGreaterThanTypicalDistance(potentialCenter.Value, clustersCenters))
                {
                    clustersCenters.Add(potentialCenter.Key);
                    irisItems.Remove(potentialCenter.Key);
                }
                else
                {
                    break;
                }
            }

            return MaxMinClustering.BuildClusters(clustersCenters, irisItems);
        }

        private static IEnumerable<Cluster> BuildClusters(IList<IrisItem> clustersCenters, IList<IrisItem> irisItems)
        {
            List<Cluster> clusters = new List<Cluster>();

            foreach (IrisItem clustersCenter in clustersCenters)
            {
                clusters.Add(new Cluster(clustersCenter));
            }

            foreach (IrisItem irisItem in irisItems)
            {
                double minDistance = double.MaxValue;
                Cluster hostCluster = null;

                foreach (Cluster cluster in clusters)
                {
                    double distance = MaxMinClustering.GetEuclideanDistance(cluster.Center, irisItem);

                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        hostCluster = cluster;
                    }
                }

                hostCluster.AddItem(irisItem);
            }

            return clusters;
        }

        private static double GetEuclideanDistance(IrisItem itemOne, IrisItem itemTwo)
        {
            double sum = 0;

            for (int i = 0; i < itemOne.Data.Length; i++)
            {
                sum += Math.Pow(itemOne.Data[i] - itemTwo.Data[i], 2);
            }

            return Math.Sqrt(sum);
        }

        private static bool IsGreaterThanTypicalDistance(double distance, IList<IrisItem> clustersCenters)
        {
            double clustersDistancesSum = 0;
            for (int i = 0; i < clustersCenters.Count; i++)
            {
                for (int j = i + 1; j < clustersCenters.Count; j++)
                {
                    clustersDistancesSum += MaxMinClustering.GetEuclideanDistance(clustersCenters[i], clustersCenters[j]);
                }
            }

            return distance > 0.5 * clustersDistancesSum / clustersCenters.Count;
        }
    }
}