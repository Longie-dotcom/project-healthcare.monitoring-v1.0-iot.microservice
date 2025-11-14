namespace Domain.ValueObject
{
    public static class ControllerStatusMemo
    {
        public static readonly ControllerStatus Online =
            new("Online", "Controller is on");
        public static readonly ControllerStatus Offline =
            new("Offline", "Controller is off");
        public static readonly ControllerStatus Maintenance =
            new("Maintenance", "Controller is maintained");

        public static readonly List<ControllerStatus> All = new()
        {
            Online, Offline, Maintenance
        };

        public static ControllerStatus? GetByName(string name)
        {
            return All.FirstOrDefault(
                x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class ControllerStatus
    {
        #region Attributes
        #endregion

        #region Properties
        public string Name { get; }
        public string Description { get; }
        #endregion

        public ControllerStatus(string name, string description)
        {
            Name = name;
            Description = description;
        }

        #region Methods
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
