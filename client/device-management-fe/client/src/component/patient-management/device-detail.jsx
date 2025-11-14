import React, { useState } from "react";
import "./device-detail.css";

function ControllerCard({ controller, onRelease, onReassign, onAssignSensor, onUnassignSensor, onReassignSensor }) {
  const [type, setType] = useState("");
  const [unit, setUnit] = useState("");
  const [description, setDescription] = useState("");

  const handleAssignSensor = () => {
    if (!controller) return;
    onAssignSensor({
      controllerKey: controller.controllerKey,
      type,
      unit,
      description,
    });
    setType("");
    setUnit("");
    setDescription("");
  };

  return (
    <div className={`record-card ${controller.isActive ? "active-card" : "inactive-card"}`}>
      <div className="controller-info">
        <div className="controller-key">
          <strong>Controller Key:</strong> {controller.controllerKey}
        </div>
        <div className="details">
          <div><strong>Bed Number:</strong> {controller.bedNumber}</div>
          <div><strong>Firmware Version:</strong> {controller.firmwareVersion}</div>
          <div><strong>Status:</strong> {controller.isActive ? "Active" : "Inactive"}</div>
          {controller.isActive && <button onClick={() => onRelease({ controllerKey: controller.controllerKey })}>Release</button>}
          {!controller.isActive && <button onClick={() => onReassign({ controllerKey: controller.controllerKey })}>Re-assign</button>}
        </div>
      </div>

      {/* Sensor Assignment */}
      <div className="assign-controls">
        <input placeholder="Type" value={type} onChange={(e) => setType(e.target.value)} />
        <input placeholder="Unit" value={unit} onChange={(e) => setUnit(e.target.value)} />
        <input placeholder="Description" value={description} onChange={(e) => setDescription(e.target.value)} />
        <button onClick={handleAssignSensor}>Assign Sensor</button>
      </div>

      {/* Sensors */}
      <div className="sensor-info">
        {(controller.sensors || []).map((s, i) => (
          <div key={i} className="record-card sensor-card">
            <div><strong>Sensor Key:</strong> {s.sensorKey}</div>
            <div><strong>Status:</strong> {s.isActive ? "Active" : "Inactive"}</div>
            {s.isActive && <button onClick={() => onUnassignSensor({ controllerKey: controller.controllerKey, sensorKey: s.sensorKey })}>Unassign</button>}
            {!s.isActive && <button onClick={() => onReassignSensor({ controllerKey: controller.controllerKey, sensorKey: s.sensorKey })}>Reassign</button>}
          </div>
        ))}
      </div>
    </div>
  );
}

function DeviceDetail({ device, assignController, unassignController, assignSensor, unassignSensor, updateController, updateSensor, setReloadDetail, onClose }) {
  if (!device) return null;

  const [bedNumber, setBedNumber] = useState("");
  const [ipAddress, setIpAddress] = useState("");
  const [firmwareVersion, setFirmwareVersion] = useState("");
  const [showReleasedControllers, setShowReleasedControllers] = useState("active");

  // -------------------------
  // Handlers
  // -------------------------
  const handleUpdateController = async ({ controllerKey, bedNumber, ipAddress, firmwareVersion, isActive }) => {
    await updateController({ edgeKey: device.edgeKey, controllerKey, bedNumber, ipAddress, firmwareVersion, isActive });
    setReloadDetail(prev => prev + 1);
  };

  const handleAssignController = async () => {
    if (!device) return;
    await assignController({ edgeKey: device.edgeKey, bedNumber, ipAddress, firmwareVersion });
    setReloadDetail(prev => prev + 1);
  };

  const handleReleaseController = async ({ controllerKey }) => {
    await unassignController({ edgeKey: device.edgeKey, controllerKey });
    setReloadDetail(prev => prev + 1);
  };

  const handleUpdateSensor = async ({ controllerKey, sensorKey, type, unit, description, isActive }) => {
    await updateSensor({ edgeKey: device.edgeKey, controllerKey, sensorKey, type, unit, description, isActive });
    setReloadDetail(prev => prev + 1);
  };

  const handleAssignSensor = async ({ controllerKey, type = "", unit = "", description = "" }) => {
    if (!controllerKey) return;
    await assignSensor({ edgeKey: device.edgeKey, controllerKey, type, unit, description });
    setReloadDetail(prev => prev + 1);
  };

  const handleUnassignSensor = async ({ controllerKey, sensorKey }) => {
    await unassignSensor({ edgeKey: device.edgeKey, controllerKey, sensorKey });
    setReloadDetail(prev => prev + 1);
  };

  const handleReassignController = async ({ controllerKey }) => {
    await handleAssignController({ controllerKey });
  };

  const handleReassignSensor = async ({ controllerKey, sensorKey }) => {
    await handleAssignSensor({ controllerKey, sensorKey });
  };

  return (
    <div id="device-detail-child">
      <div className="header">
        <h3>📟 {device.edgeKey} Details</h3>
        <button onClick={onClose}>Close detail</button>
      </div>

      {/* Basic info */}
      <div className="info-grid">
        <div>
          <div><strong>Edge Key:</strong> {device.edgeKey}</div>
          <div><strong>Room Name:</strong> {device.roomName}</div>
          <div><strong>IP Address:</strong> {device.ipAddress}</div>
          <div><strong>Description:</strong> {device.description}</div>
          <div><strong>Activation:</strong> {device.isActive ? "Active" : "Inactive"}</div>
        </div>
      </div>

      {/* Controller assignment */}
      <section className="section">
        <h4>🖥️ Controller Assignment</h4>
        <div className="assign-controls">
          <input type="text" placeholder="Bed Number" value={bedNumber} onChange={(e) => setBedNumber(e.target.value)} />
          <input type="text" placeholder="Firmware Version" value={firmwareVersion} onChange={(e) => setFirmwareVersion(e.target.value)} />
          <input type="text" placeholder="IP Address" value={ipAddress} onChange={(e) => setIpAddress(e.target.value)} />
          <button onClick={handleAssignController}>Assign Controller</button>
        </div>

        {/* Toggle released controllers */}
        <button className="toggle-btn" onClick={() => {
          setShowReleasedControllers(prev => {
            if (prev === "active") return "inactive";
            if (prev === "inactive") return "all";
            return "active";
          });
        }}>
          {showReleasedControllers === "active" && "Show Active Controllers"}
          {showReleasedControllers === "inactive" && "Show Inactive Controllers"}
          {showReleasedControllers === "all" && "Show All Controllers"}
        </button>

        {/* Controllers */}
        <div>
          {(device.controllers || [])
            .filter(c => showReleasedControllers === "all" ? true : (showReleasedControllers === "active" ? c.isActive : !c.isActive))
            .map(c => (
              <ControllerCard
                key={c.controllerKey}
                controller={c}
                onReassign={handleReassignController}
                onRelease={handleReleaseController}
                onAssignSensor={handleAssignSensor}
                onUnassignSensor={handleUnassignSensor}
                onReassignSensor={handleReassignSensor}
              />
            ))
          }
        </div>
      </section>
    </div>
  );
}

export default DeviceDetail;
