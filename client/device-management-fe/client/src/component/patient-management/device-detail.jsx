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
          <strong>Controller Key:</strong>&nbsp;{controller.controllerKey}
        </div>
        <div className="details">
          <div>
            <strong>Bed Number:</strong>&nbsp;{controller.bedNumber}
          </div>
          <div>
            <strong>Firmware Version:</strong>&nbsp;{controller.firmwareVersion}
          </div>
          <div>
            <strong>Status:</strong>&nbsp;{controller.isActive ? "Active" : "Inactive"}
          </div>
          {controller.isActive && (
            <button onClick={() => onRelease(controller.controllerKey)}>Release</button>
          )}
          {!controller.isActive && (
            <button className="re-assign" onClick={() => onReassign(controller.controllerKey)}>Re-assign</button>
          )}
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
            {s.isActive && (
              <button className="btn unassign" onClick={() => onUnassignSensor(s.sensorKey)}>
                Unassign
              </button>
            )}
            {!s.isActive && (
              <button className="btn re-assign" onClick={() => onReassignSensor(s.sensorKey)}>
                Reassign
              </button>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

function DeviceDetail({
  device,
  assignController,
  unassignController,
  assignSensor,
  unassignSensor,
  setReloadDetail,
  onClose,
  reactivateController,
  reactivateSensor
}) {
  if (!device) return null;
  const [bedNumber, setBedNumber] = useState("");
  const [ipAddress, setIpAddress] = useState("");
  const [firmwareVersion, setFirmwareVersion] = useState("");

  const [showReleasedControllers, setShowReleasedControllers] = useState("active");
  const formatDate = (date) => (date ? date.split("T")[0] : "");

  const handleReassignController = async ({ controllerKey }) => {
    await reactivateController({
      controllerKey
    });
    setReloadDetail((prev) => prev + 1);
  };

  const handleAssignController = async () => {
    if (!device) return;
    await assignController({
      edgeKey: device.edgeKey, bedNumber, ipAddress, firmwareVersion
    });
    setReloadDetail((prev) => prev + 1);
  };

  const handleReleaseController = async ({ controllerKey }) => {
    await unassignController({
      controllerKey
    });
    setReloadDetail((prev) => prev + 1);
  };

  const handleReassignSensor = async ({ sensorKey }) => {
    await reactivateSensor({
      sensorKey
    });
    setReloadDetail((prev) => prev + 1);
  };

  const handleAssignSensor = async ({ controllerKey, type = "", unit = "", description = "" }) => {
    if (!controllerKey) return;
    await assignSensor({
      controllerKey, type, unit, description,
    });
    setReloadDetail((prev) => prev + 1);
  };

  const handleUnassignSensor = async ({ sensorKey }) => {
    await unassignSensor({
      sensorKey
    });
    setReloadDetail((prev) => prev + 1);
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
          <div>
            <strong>Edge Key:</strong>
            <div>{device.edgeKey}</div>
          </div>
          <div>
            <strong>Room Name</strong>
            <div>{device.roomName}</div>
          </div>
          <div>
            <strong>IP Address</strong>
            <div>{device.ipAddress}</div>
          </div>
          <div>
            <strong>Description</strong>
            <div>{device.description}</div>
          </div>
          <div>
            <strong>Activation</strong>
            <div>{device.isActive ? "Active" : "Inactive"}</div>
          </div>
        </div>
      </div>

      {/* Controller assignment */}
      <section className="section">
        <h4>🖥️ Controller Assignment</h4>
        <div className="assign-controls">
          <input
            type="text"
            placeholder="Bed Number"
            value={bedNumber}
            onChange={(e) => setBedNumber(e.target.value)}
          />
          <input
            type="text"
            placeholder="Firmware Version"
            value={firmwareVersion}
            onChange={(e) => setFirmwareVersion(e.target.value)}
          />
          <input
            type="text"
            placeholder="IP Address"
            value={ipAddress}
            onChange={(e) => setIpAddress(e.target.value)}
          />
          <button onClick={handleAssignController}>
            Assign Controller
          </button>
        </div>

        {/* Toggle released controllers */}
        <button
          className="toggle-btn"
          onClick={() => {
            setShowReleasedControllers((prev) => {
              if (prev === "active") return "inactive";
              if (prev === "inactive") return "all";
              return "active"; // cycle back to active
            });
          }}
        >
          {showReleasedControllers === "active" && "Show Active Controllers"}
          {showReleasedControllers === "inactive" && "Show Inactive Controllers"}
          {showReleasedControllers === "all" && "Show All Controllers"}
        </button>

        {/* Controllers */}
        <div>
          {(device.controllers || [])
            .filter((c) => {
              if (showReleasedControllers === "active") return c.isActive;
              if (showReleasedControllers === "inactive") return !c.isActive;
              if (showReleasedControllers === "all") return true; // show all controllers
              return true;
            })
            .map((c) => (
              <ControllerCard
                key={c.controllerKey} // stable key
                controller={c}
                onReassign={(controllerKey) => handleReassignController({ controllerKey })}
                onRelease={(controllerKey) => handleReleaseController({ controllerKey })}
                onAssignSensor={(data) => handleAssignSensor(data)} // just pass the object
                onUnassignSensor={(sensorKey) => handleUnassignSensor({ sensorKey })}
                onReassignSensor={(sensorKey) => handleReassignSensor({ sensorKey })}
              />
            ))}
        </div>
      </section>
    </div>
  );
}

export default DeviceDetail;
