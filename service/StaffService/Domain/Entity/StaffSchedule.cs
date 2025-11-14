using Domain.Aggregate;
using Domain.DomainException;
using Domain.Enum;

namespace Domain.Entity
{
    public class StaffSchedule
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid StaffScheduleID { get; private set; }
        public Guid StaffID { get; private set; }
        public string DayOfWeek { get; private set; }
        public TimeSpan ShiftStart { get; private set; }
        public TimeSpan ShiftEnd { get; private set; }
        public bool IsActive { get; private set; }

        public Staff Staff { get; protected set; }
        #endregion

        protected StaffSchedule() { }

        public StaffSchedule(
            Guid staffScheduleID,
            Guid staffID,
            string dayOfWeek,
            TimeSpan shiftStart,
            TimeSpan shiftEnd,
            bool isActive = true)
        {
            ValidateStaffScheduleID(staffScheduleID);
            ValidateStaffID(staffID);
            ValidateDayOfWeek(dayOfWeek);
            ValidateShiftTime(shiftStart, shiftEnd);

            StaffScheduleID = staffScheduleID;
            StaffID = staffID;
            DayOfWeek = dayOfWeek;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            IsActive = isActive;
        }

        #region Methods
        public void UpdateDayOfWeek(string dayOfWeek)
        {
            ValidateDayOfWeek(dayOfWeek);
            DayOfWeek = dayOfWeek;
        }

        public void UpdateShift(TimeSpan start, TimeSpan end)
        {
            ValidateShiftTime(start, end);
            ShiftStart = start;
            ShiftEnd = end;
        }

        public void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }
        #endregion

        #region Validators
        private void ValidateStaffScheduleID(Guid staffScheduleId)
        {
            if (staffScheduleId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffScheduleID cannot be empty.");
        }

        private void ValidateStaffID(Guid staffId)
        {
            if (staffId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffID cannot be empty.");
        }

        private void ValidateDayOfWeek(string day)
        {
            var validDays = WeekEnum.DAY_OF_WEEK;
            if (!validDays.Contains(day))
                throw new InvalidStaffAggregateException($"Invalid day of week: {day}");
        }

        private void ValidateShiftTime(TimeSpan start, TimeSpan end)
        {
            if (start >= end)
                throw new InvalidStaffAggregateException("Shift start must be before shift end.");
        }
        #endregion
    }
}
