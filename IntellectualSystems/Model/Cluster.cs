namespace IntellectualSystems.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IntellectualSystems.Utils;

    internal class Cluster
    {
        public Cluster(DataItem center, IEnumerable<DataItem> dataItems)
        {
            Guard.NotNull(center, "center");
            Guard.NotNull(dataItems, "dataItems");

            this.Center = center;
            this.Items = dataItems.ToList();
        }

        public DataItem Center
        {
            get;
            set;
        }

        public IEnumerable<DataItem> Items
        {
            get;
            private set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("Cluster's count: {0}", this.Items.Count()));
            sb.AppendLine(string.Format("Cluster's center: {0}", this.Center));

            foreach (DataItem irisItem in this.Items)
            {
                sb.AppendLine(string.Format("\t{0}", irisItem));
            }

            return sb.ToString();
        }
    }
}