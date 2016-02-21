namespace IntellectualSystems
{
    using System.Linq;

    using IntellectualSystems.Utils;

    internal class IrisItem
    {
        public IrisItem(int classId, double[] data)
        {
            Guard.IntMoreThanZero(classId, "classId");
            Guard.NotNull(data, "data");

            this.ClassId = classId;
            this.Data = data;
        }

        public int ClassId
        {
            get;
            private set;
        }

        public double[] Data
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.ClassId, string.Join(" ", this.Data.Select(i => i.ToString())
                .ToArray()));
        }
    }
}