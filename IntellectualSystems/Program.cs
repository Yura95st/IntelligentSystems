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
            IList<IrisItem> irisItems = IrisDataParser.ParseItemsFromResource(Program.irisDataResourceName)
                .ToList();

            foreach (Cluster cluster in MaxMinClustering.PerformClustering(irisItems))
            {
                Console.WriteLine(cluster);
            }
        }
    }
}