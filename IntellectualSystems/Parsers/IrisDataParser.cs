namespace IntellectualSystems.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using IntellectualSystems.Model;
    using IntellectualSystems.Utils;

    internal class IrisDataParser
    {
        public static IEnumerable<IrisItem> ParseItemsFromResource(string resourceName)
        {
            Guard.NotNull(resourceName, "resourceName");

            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            foreach (string line in IrisDataParser.ReadLines(() => stream, Encoding.UTF8))
            {
                string[] lineParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                yield return new IrisItem(int.Parse(lineParts[0]), Array.ConvertAll(lineParts.Skip(1)
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