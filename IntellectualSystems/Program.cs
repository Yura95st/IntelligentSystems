namespace IntellectualSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IntellectualSystems.Model;
    using IntellectualSystems.Parsers;

    internal class Program
    {
        private static readonly string irisDataResourceName = "IntellectualSystems.iris.dat.txt";

        private static void Main(string[] args)
        {
            List<DataItem> dataItems = DataItemsParser.ParseItemsFromResource(Program.irisDataResourceName)
                .ToList();

            //foreach (var cluster in MaxMinClustering.PerformClustering(dataItems))
            //{
            //    Console.WriteLine(cluster);
            //}

            foreach (Cluster cluster in KMeansClustering.PerformClustering(dataItems, 3))
            {
                Console.WriteLine(cluster);
            }
        }
    }
}