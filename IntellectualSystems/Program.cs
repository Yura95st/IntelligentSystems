namespace IntellectualSystems
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class Program
    {
        private static readonly string irisDataResourceName = "IntellectualSystems.iris.dat.txt";

        private static IDictionary<IrisItem, IList<IrisItem>> BuildClustersDictionary(IList<IrisItem> clustersCenters,
                                                                                      IList<IrisItem> irisItems)
        {
            Dictionary<IrisItem, IList<IrisItem>> clustersDictionary = new Dictionary<IrisItem, IList<IrisItem>>();

            foreach (IrisItem clustersCenter in clustersCenters)
            {
                clustersDictionary.Add(clustersCenter, new List<IrisItem>());
            }

            foreach (IrisItem irisItem in irisItems)
            {
                double minDistance = double.MaxValue;
                IrisItem hostCluster = null;

                foreach (IrisItem clustersCenter in clustersCenters)
                {
                    double distance = Program.GetEuclideanDistance(clustersCenter, irisItem);

                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        hostCluster = clustersCenter;
                    }
                }

                clustersDictionary[hostCluster].Add(irisItem);
            }

            return clustersDictionary;
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

        private static IList<IrisItem> GetIrisItems()
        {
            List<IrisItem> items = new List<IrisItem>();

            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Program.irisDataResourceName);

            foreach (string line in Program.ReadLines(() => stream, Encoding.UTF8))
            {
                string[] lineParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                items.Add(new IrisItem(int.Parse(lineParts[0]), Array.ConvertAll(lineParts.Skip(1)
                    .ToArray(), Double.Parse)));
            }

            return items;
        }

        private static bool IsGreaterThanTypicalDistance(double distance, IList<IrisItem> clustersCenters)
        {
            double clustersDistancesSum = 0;
            for (int i = 0; i < clustersCenters.Count; i++)
            {
                for (int j = i + 1; j < clustersCenters.Count; j++)
                {
                    clustersDistancesSum += Program.GetEuclideanDistance(clustersCenters[i], clustersCenters[j]);
                }
            }

            return distance > 0.5 * clustersDistancesSum / clustersCenters.Count;
        }

        private static void Main(string[] args)
        {
            IList<IrisItem> irisItems = Program.GetIrisItems();

            List<IrisItem> clustersCenters = new List<IrisItem>();

            clustersCenters.Add(irisItems[0]);
            irisItems.Remove(irisItems[0]);

            double maxDistance = 0;
            IrisItem secondCenter = null;

            foreach (IrisItem irisItem in irisItems)
            {
                double distance = Program.GetEuclideanDistance(clustersCenters[0], irisItem);

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
                        distances.Add(Program.GetEuclideanDistance(clustersCenter, irisItem));
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

                if (Program.IsGreaterThanTypicalDistance(potentialCenter.Value, clustersCenters))
                {
                    clustersCenters.Add(potentialCenter.Key);
                    irisItems.Remove(potentialCenter.Key);
                }
                else
                {
                    break;
                }
            }

            IDictionary<IrisItem, IList<IrisItem>> clustersDictionary = Program.BuildClustersDictionary(clustersCenters,
                irisItems);

            Program.PrintClustersInfoToConsole(clustersDictionary);
        }

        private static void PrintClustersInfoToConsole(IDictionary<IrisItem, IList<IrisItem>> clustersDictionary)
        {
            foreach (KeyValuePair<IrisItem, IList<IrisItem>> cluster in clustersDictionary)
            {
                Console.WriteLine("Cluster's center: {0}", cluster.Key);

                foreach (IrisItem irisItem in cluster.Value)
                {
                    Console.WriteLine("\t{0}", irisItem);
                }

                Console.WriteLine();
            }
        }

        private static IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
        {
            using (Stream stream = streamProvider())
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }
    }
}