import { useSensorHub } from "../../api/useSensorHub";
import { useRoomProfile } from "../../api/useRoomProfile";
import PatientSensorGrid from "./patient-sensor-grid";
import { useState } from "react";

function RoomProfile() {
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const sensorHistory = useSensorHub();
  const {
    getPatientDetailOfStaff
  } = useRoomProfile({ setError, setLoading });
  
  return (
    <PatientSensorGrid history={sensorHistory} getPatientDetailOfStaff={getPatientDetailOfStaff} />
  );
}

export default RoomProfile;
