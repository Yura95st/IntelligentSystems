namespace IntellectualSystems.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IntellectualSystems.Utils;

    internal class Cluster
    {
        private readonly ISet<IrisItem> _items;

        public Cluster(IrisItem center)
        {
            Guard.NotNull(center, "center");

            this.Center = center;

            this._items = new HashSet<IrisItem>();
        }

        public IrisItem Center
        {
            get;
            private set;
        }

        public IEnumerable<IrisItem> Items
        {
            get
            {
                return this._items.ToList();
            }
        }

        public void AddItem(IrisItem item)
        {
            Guard.NotNull(item, "item");

            this._items.Add(item);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("Cluster's count: {0}", this.Items.Count()));
            sb.AppendLine(string.Format("Cluster's center: {0}", this.Center));

            foreach (IrisItem irisItem in this.Items)
            {
                sb.AppendLine(string.Format("\t{0}", irisItem));
            }

            return sb.ToString();
        }
    }
}