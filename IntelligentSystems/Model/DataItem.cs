namespace IntelligentSystems.Model
{
    using System.Linq;

    using IntelligentSystems.Utils;

    internal class DataItem
    {
        public DataItem(int classId, double[] values)
        {
            Guard.NotNull(values, "values");

            this.ClassId = classId;
            this.Values = values;
        }

        public int ClassId
        {
            get;
            private set;
        }

        public double[] Values
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return string.Format("Class: {0}; Values: {1}", this.ClassId,
                string.Join(" ", this.Values.Select(i => i.ToString())
                    .ToArray()));
        }

        public static DataItem operator +(DataItem d1, DataItem d2)

        {
            Guard.NotNull(d1, "d1");
            Guard.NotNull(d2, "d2");

            double[] values = new double[d1.Values.Length];

            for (int i = 0; i < d1.Values.Length; i++)
            {
                values[i] = d1.Values[i] + d2.Values[i];
            }

            return new DataItem(-1, values);
        }

        public static DataItem operator /(DataItem dataItem, double num)
        {
            return dataItem * (1.0 / num);
        }

        public static DataItem operator *(DataItem dataItem, double num)

        {
            Guard.NotNull(dataItem, "dataItem");

            return new DataItem(-1, dataItem.Values.Select(v => num * v)
                .ToArray());
        }
    }
}