namespace IntelligentSystems.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using IntelligentSystems.Model;
    using IntelligentSystems.Utils;

    internal class DataItemsParser
    {
        public static IEnumerable<DataItem> ParseItemsFromResource(string resourceName)
        {
            Guard.NotNull(resourceName, "resourceName");

            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            foreach (string line in DataItemsParser.ReadLines(() => stream, Encoding.UTF8))
            {
                string[] lineParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                yield return new DataItem(int.Parse(lineParts[0]), Array.ConvertAll(lineParts.Skip(1)
                    .ToArray(), Double.Parse));
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