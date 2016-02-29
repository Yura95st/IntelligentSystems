namespace IntelligentSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntelligentSystems.Model;
    using IntelligentSystems.Parsers;

    internal class Program
    {
        private static readonly string irisDataResourceName = "IntelligentSystems.iris.dat.txt";

        private static void Main(string[] args)
        {
            List<DataItem> dataItems = DataItemsParser.ParseItemsFromResource(Program.irisDataResourceName)
                .ToList();

            //foreach (var cluster in MaxMinClustering.PerformClustering(dataItems))
            //{
            //    Console.WriteLine(cluster);
            //}

            foreach (Cluster cluster in CMeansClustering.PerformClustering(dataItems, 3))
            {
                Console.WriteLine(cluster);
            }
        }
    }
}