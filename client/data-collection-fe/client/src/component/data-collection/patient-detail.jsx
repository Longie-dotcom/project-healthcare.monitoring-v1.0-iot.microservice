// PatientDetail.jsx
import React from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from "recharts";
import "./patient-detail.css";

export default function PatientDetail({ patient, onClose }) {
  if (!patient) return null;

  const renderCharts = () => {
    if (!patient.sensors || patient.sensors.length === 0) return null;

    // Merge all sensor data by type
    const chartSensors = {};

    patient.sensors.forEach((sensor) => {
      sensor.sensorDatas?.forEach((d) => {
        const type = d.type?.toLowerCase() || sensor.sensorKey.toLowerCase(); // fallback to sensorKey if type missing
        if (!chartSensors[type]) chartSensors[type] = [];
        chartSensors[type].push({
          timestamp: d.recordedAt,
          value: Number(d.value),
        });
      });
    });

    const sensorOrder = ["bpm", "spo2", "red", "ir", "temp"]; // common order

    return (
      <div className="charts-wrapper">
        {sensorOrder.map(
          (type) =>
            chartSensors[type]?.length > 0 && (
              <div key={type} className="sensor-type-chart">
                <h3>
                  {type === "bpm"
                    ? "Heart Rate (BPM)"
                    : type === "spo2"
                    ? "SpO₂ (%)"
                    : type === "temp"
                    ? "Temperature (°C)"
                    : type.toUpperCase()}
                </h3>
                <ResponsiveContainer width="100%" height={250}>
                  <LineChart data={chartSensors[type]}>
                    <XAxis dataKey="timestamp" hide />
                    <YAxis domain={type === "spo2" ? [50, 100] : undefined} />
                    <Tooltip />
                    <Line
                      type="monotone"
                      dataKey="value"
                      stroke={
                        type === "bpm"
                          ? "#1e90ff"
                          : type === "spo2"
                          ? "#32cd32"
                          : type === "red"
                          ? "#ff0000"
                          : type === "ir"
                          ? "#8b0000"
                          : "#ff7f50" // temp
                      }
                      dot={false}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            )
        )}
      </div>
    );
  };

  return (
    <div className="overlay">
      <div className="detail-card">
        <button className="close-detail-btn" onClick={onClose}>
          × Close
        </button>

        <div className="detail-content">
          <h2>{patient.fullName}</h2>
          <p><b>ID:</b> {patient.identityNumber}</p>
          <p><b>Gender:</b> {patient.gender}</p>
          <p><b>DOB:</b> {new Date(patient.dob).toLocaleDateString()}</p>
          <p><b>Bed:</b> {patient.bedNumber}</p>
          <p><b>IP:</b> {patient.ipAddress || "-"}</p>
          <p><b>Controller:</b> {patient.controllerKey || "-"}</p>

          <h3>Sensors Assigned</h3>
          <ul>
            {patient.sensors?.map((s) => (
              <li key={s.patientSensorID}>{s.sensorKey}</li>
            ))}
          </ul>

          {renderCharts()}
        </div>
      </div>
    </div>
  );
}
