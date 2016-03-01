namespace IntelligentSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IntelligentSystems.Model;
    using IntelligentSystems.Parsers;
    using IntelligentSystems.Utils;

    internal class Program
    {
        private static readonly int classesCount = 3;

        private static readonly string irisDataResourceName = "IntelligentSystems.iris.dat.txt";

        private static string GetFunctionStringFromWeightsVector(IList<double> weightsVector)
        {
            Guard.NotNull(weightsVector, "weightsVector");

            StringBuilder sb = new StringBuilder();

            sb.Append("d(x) = ");
            sb.Append(weightsVector[0]);

            for (int i = 1; i < weightsVector.Count; i++)
            {
                sb.Append(string.Format(" {0} {1} * x{2}", weightsVector[i] > 0 ? "+" : "-", Math.Abs(weightsVector[i]), i));
            }

            return sb.ToString();
        }

        private static void Main(string[] args)
        {
            List<DataItem> dataItems = DataItemsParser.ParseItemsFromResource(Program.irisDataResourceName)
                .ToList();

            //foreach (var cluster in MaxMinClustering.PerformClustering(dataItems))
            //{
            //    Console.WriteLine(cluster);
            //}

            //foreach (Cluster cluster in CMeansClustering.PerformClustering(dataItems, Program.classesCount))
            //{
            //    Console.WriteLine(cluster);
            //}

            IList<IList<double>> weightsVectors = PerceptronLearning.PerformLearning(dataItems, Program.classesCount);
            List<Cluster> clusters = PerceptronLearning.PerformClustering(dataItems, weightsVectors)
                .ToList();

            foreach (Cluster cluster in clusters)
            {
                Console.WriteLine("Errors in cluster: {0}", ClusteringUtils.GetClusteringErrorsNumber(cluster));
                Console.WriteLine(cluster);
            }

            Console.WriteLine("Clustering quality is: {0}%",
                Math.Floor(100.0 * ClusteringUtils.GetClusteringQuality(clusters)));
            Console.WriteLine();
            Console.WriteLine("Discriminant functions:");

            foreach (IList<double> weightsVector in weightsVectors)
            {
                Console.WriteLine();
                Console.WriteLine(Program.GetFunctionStringFromWeightsVector(weightsVector));
            }
        }
    }
}