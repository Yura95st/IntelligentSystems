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

        private static readonly string dataResourceName = "IntelligentSystems.lab_4.dat.txt";

        private static readonly string irisDataResourceName = "IntelligentSystems.iris.dat.txt";

        private static string GetFunctionStringFromVector(IList<double> vector)
        {
            Guard.NotNull(vector, "vector");

            StringBuilder sb = new StringBuilder();

            sb.Append("d(x) = ");
            sb.Append(vector[0]);

            for (int i = 1; i < vector.Count; i++)
            {
                sb.Append(string.Format(" {0} {1} * x{2}", vector[i] > 0 ? "+" : "-", Math.Abs(vector[i]), i));
            }

            return sb.ToString();
        }

        private static void Lab1(IList<DataItem> dataItems)
        {
            foreach (Cluster cluster in MaxMinClustering.PerformClustering(dataItems))
            {
                Console.WriteLine(cluster);
            }
        }

        private static void Lab2(IList<DataItem> dataItems)
        {
            foreach (Cluster cluster in CMeansClustering.PerformClustering(dataItems, Program.classesCount))
            {
                Console.WriteLine(cluster);
            }
        }

        private static void Lab3(IList<DataItem> dataItems)
        {
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
                Console.WriteLine(Program.GetFunctionStringFromVector(weightsVector));
            }
        }

        private static void Lab4(IList<DataItem> dataItems)
        {
            IList<IList<double>> coefficientsVectors = BayesСlassifierWithNormalDistribution.BuildDecisionFunctions(dataItems);

            Console.WriteLine("Functions:");

            foreach (IList<double> coefficientsVector in coefficientsVectors)
            {
                Console.WriteLine();
                Console.WriteLine(Program.GetFunctionStringFromVector(coefficientsVector));
            }
        }

        private static void Main(string[] args)
        {
            //List<DataItem> dataItems = DataItemsParser.ParseItemsFromResource(Program.irisDataResourceName)
            //    .ToList();

            //Program.Lab1(dataItems);
            //Program.Lab2(dataItems);
            //Program.Lab3(dataItems);

            List<DataItem> dataItems = DataItemsParser.ParseItemsFromResource(Program.dataResourceName)
                .ToList();

            Program.Lab4(dataItems);
        }
    }
}