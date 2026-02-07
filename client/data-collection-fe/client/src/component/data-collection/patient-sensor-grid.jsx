import React, { useState } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from "recharts";
import "./patient-sensor-grid.css";

export default function PatientSensorGrid({ history, getPatientDetailOfStaff }) {
  const patientIds = Object.keys(history);

  const [detail, setDetail] = useState(null);
  const [loadingDetail, setLoadingDetail] = useState(false);

  // --- Open detail view ---
  const openDetail = async (patient) => {
    setLoadingDetail(true);

    // Fetch full detail from API
    const detailData = await getPatientDetailOfStaff({
      patientIdentityNumber: patient.identityNumber,
    });

    // Convert sensorDatas array into object by type for charting
    const sensors = {};
    if (detailData?.sensors) {
      detailData.sensors.forEach((s) => {
        const type = s.sensorKey.toLowerCase();
        sensors[type] = s.sensorDatas.map((d) => ({
          timestamp: d.recordedAt,
          value: Number(d.value),
        }));
      });
    }
    detailData.processedSensors = sensors;

    setDetail(detailData);
    setLoadingDetail(false);
  };

  const closeDetail = () => setDetail(null);

  // --- Render charts in detail view ---
  const renderDetailCharts = (detail) => {
    // pick the MAX30102 sensor
    const maxSensor = detail.sensors?.find(s => s.sensorKey.includes("MAX30102"));
    if (!maxSensor || !maxSensor.sensorDatas) return null;

    // group by type
    const sensors = {};
    maxSensor.sensorDatas.forEach(d => {
      const type = d.type?.toLowerCase();
      if (!type) return;
      if (!sensors[type]) sensors[type] = [];
      sensors[type].push({ timestamp: d.recordedAt, value: Number(d.value) });
    });

    return (
      <div className="charts-wrapper">
        {["bpm", "spo2", "red", "ir"].map((type) =>
          sensors[type] && sensors[type].length > 0 ? (
            <div key={type} className="sensor-type-chart">
              <h3>
                {type === "bpm" ? "Heart Rate (BPM)" :
                  type === "spo2" ? "SpO₂ (%)" :
                    type.toUpperCase()}
              </h3>
              <ResponsiveContainer width="100%" height={250}>
                <LineChart data={sensors[type]}>
                  <XAxis dataKey="timestamp" hide />
                  <YAxis domain={type === "spo2" ? [50, 100] : undefined} />
                  <Tooltip />
                  <Line type="monotone" dataKey="value" stroke={
                    type === "bpm" ? "#1e90ff" :
                      type === "spo2" ? "#32cd32" :
                        type === "red" ? "#ff0000" :
                          "#8b0000"
                  } dot={false} />
                </LineChart>
              </ResponsiveContainer>
            </div>
          ) : null
        )}
      </div>
    );
  };

  return (
    <>
      {/* ================= DETAIL VIEW ================= */}
      {detail && (
        <div className="detail-card">
          <button className="close-detail-btn" onClick={closeDetail}>
            ← Back
          </button>

          {loadingDetail ? (
            <p>Loading...</p>
          ) : (
            <div className="detail-content">
              <h2>{detail.fullName}</h2>
              <p><b>ID:</b> {detail.identityNumber}</p>
              <p><b>Gender:</b> {detail.gender}</p>
              <p><b>DOB:</b> {new Date(detail.dob).toLocaleDateString()}</p>
              <p><b>Bed:</b> {detail.bedNumber}</p>
              <p><b>IP:</b> {detail.ipAddress}</p>
              <p><b>Controller:</b> {detail.controllerKey}</p>

              <h3>Sensors Assigned</h3>
              <ul>
                {detail.sensors?.map((s) => (
                  <li key={s.patientSensorID}>{s.sensorKey}</li>
                ))}
              </ul>

              {renderDetailCharts(detail)}
            </div>
          )}
        </div>
      )}

      {/* ================= LIST VIEW ================= */}
      {!detail && (
        <div className="grid-container">
          {patientIds.map((pid) => {
            const patient = history[pid];
            const sensors = patient.sensors || {};
            return (
              <div key={pid} className="sensor-card">
                {/* HEADER */}
                <div className="card-header">
                  <h2>Patient: {pid}</h2>
                  {patient.bedNumber && <p>Bed: {patient.bedNumber}</p>}
                  {patient.roomNumber && <p>Room: {patient.roomNumber}</p>}
                  {patient.fullName && <p>Name: {patient.fullName}</p>}
                </div>

                {/* VIEW DETAIL BUTTON */}
                <button
                  className="view-detail-btn"
                  onClick={() => openDetail(patient)}
                >
                  View Details
                </button>

                {/* ORIGINAL CHARTS */}
                <div className="charts-wrapper">
                  {/* Temp */}
                  {sensors.temp && sensors.temp.length > 0 && (
                    <div className="sensor-type-chart">
                      <h3>Temperature (°C)</h3>
                      <ResponsiveContainer width="100%" height={250}>
                        <LineChart data={sensors.temp}>
                          <XAxis dataKey="timestamp" hide />
                          <YAxis />
                          <Tooltip />
                          <Line type="monotone" dataKey="value" stroke="#ff7f50" dot={false} />
                        </LineChart>
                      </ResponsiveContainer>
                      <div className="latest-value">
                        Latest: {sensors.temp[sensors.temp.length - 1].value}
                      </div>
                    </div>
                  )}

                  {/* BPM */}
                  {sensors.bpm && sensors.bpm.length > 0 && (
                    <div className="sensor-type-chart">
                      <h3>Heart Rate (BPM)</h3>
                      <ResponsiveContainer width="100%" height={250}>
                        <LineChart data={sensors.bpm}>
                          <XAxis dataKey="timestamp" hide />
                          <YAxis />
                          <Tooltip />
                          <Line type="monotone" dataKey="value" stroke="#1e90ff" dot={false} />
                        </LineChart>
                      </ResponsiveContainer>
                      <div className="latest-value">
                        Latest: {sensors.bpm[sensors.bpm.length - 1].value}
                      </div>
                    </div>
                  )}

                  {/* SpO2 */}
                  {sensors.spo2 && sensors.spo2.length > 0 && (
                    <div className="sensor-type-chart">
                      <h3>SpO₂ (%)</h3>
                      <ResponsiveContainer width="100%" height={250}>
                        <LineChart data={sensors.spo2}>
                          <XAxis dataKey="timestamp" hide />
                          <YAxis domain={[50, 100]} />
                          <Tooltip />
                          <Line type="monotone" dataKey="value" stroke="#32cd32" dot={false} />
                        </LineChart>
                      </ResponsiveContainer>
                      <div className="latest-value">
                        Latest: {sensors.spo2[sensors.spo2.length - 1].value}
                      </div>
                    </div>
                  )}

                  {/* Red & IR */}
                  {sensors.red && sensors.ir && (
                    <div className="sensor-type-chart">
                      <h3>Red & IR Signals</h3>
                      <ResponsiveContainer width="100%" height={250}>
                        <LineChart
                          data={sensors.red.map((r, i) => ({
                            timestamp: r.timestamp,
                            red: r.value,
                            ir: sensors.ir[i]?.value || 0,
                          }))}
                        >
                          <XAxis dataKey="timestamp" hide />
                          <YAxis />
                          <Tooltip />
                          <Legend />
                          <Line type="monotone" dataKey="red" stroke="#ff0000" dot={false} />
                          <Line type="monotone" dataKey="ir" stroke="#8b0000" dot={false} />
                        </LineChart>
                      </ResponsiveContainer>
                      <div className="latest-value">
                        Latest: Red {sensors.red[sensors.red.length - 1].value},
                        IR {sensors.ir[sensors.ir.length - 1].value}
                      </div>
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}
    </>
  );
}
