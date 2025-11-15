using Domain.ValueObject;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entity
{
    public class PatientSensor
    {
        #region Attributes
        #endregion

        #region Properties
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid PatientSensorID { get; private set; }
        public string ControllerKey { get; private set; }
        public string SensorKey { get; private set; } = string.Empty;
        public DateTime AssignedAt { get; private set; }
        public DateTime? UnassignedAt { get; private set; }
        public bool IsActive { get; private set; }


        [BsonElement("SensorDatas")]
        public List<SensorData> SensorDatas { get; private set; } = new();

        public bool CanReceiveData
        {
            get { return IsActive && !IsUnassigned(); }
        }
        #endregion

        public PatientSensor() { }

        public PatientSensor(
            Guid patientSensorId,
            string controllerKey,
            string sensorKey,
            DateTime assignedAt)
        {
            PatientSensorID = patientSensorId;
            ControllerKey = controllerKey;
            SensorKey = sensorKey;
            AssignedAt = assignedAt;
            IsActive = true;
        }

        #region Methods
        public void UpdateSensorInfo(
            bool? isActive)
        {
            if (IsUnassigned())
                return;

            if (isActive.HasValue)
                IsActive = isActive.Value;
        }

        public void Unassign()
        {
            if (IsUnassigned())
                return;

            UnassignedAt = DateTime.UtcNow;
            IsActive = false;
        }

        public void AddData(BsonValue value)
        {
            if (!CanReceiveData)
                return;

            SensorDatas.Add(new SensorData(value));
        }

        private bool IsUnassigned()
        {
            return UnassignedAt != null;
        }
        #endregion
    }
}
