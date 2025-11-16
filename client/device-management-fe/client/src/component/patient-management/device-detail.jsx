import React, { useState, useEffect } from "react";
import "./device-detail.css";

function ControllerCard({
  controller,
  onRelease,
  onAssignSensor,
  onUnassignSensor,
  onUpdateController,
  onUpdateSensor
}) {
  // Editable fields for controller
  const [editBed, setEditBed] = useState(controller.bedNumber || "");
  const [editIp, setEditIp] = useState(controller.ipAddress || "");
  const [editFirmware, setEditFirmware] = useState(controller.firmwareVersion || "");
  const [editIsActive, setEditIsActive] = useState(
    controller.isActive
  );

  // Sensor creation fields
  const [type, setType] = useState("");
  const [unit, setUnit] = useState("");
  const [description, setDescription] = useState("");

  // Sensor edit states
  const [sensorEdits, setSensorEdits] = useState([]);

  // Initialize sensor edits when controller.sensors changes
  useEffect(() => {
    setSensorEdits(
      (controller.sensors || []).map(s => ({
        sensorKey: s.sensorKey,
        type: s.type || "",
        unit: s.unit || "",
        description: s.description || "",
        isActive: s.isActive ?? false
      }))
    );
  }, [controller.sensors]);

  // Update individual sensor field
  const handleSensorChange = (index, field, value) => {
    setSensorEdits(prev => {
      const newEdits = [...prev];
      newEdits[index][field] = value;
      return newEdits;
    });
  };

  const handleUpdateController = () => {
    onUpdateController({
      controllerKey: controller.controllerKey,
      bedNumber: editBed,
      ipAddress: editIp,
      firmwareVersion: editFirmware,
      isActive: editIsActive
    });
  };

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

  const handleUpdateSensor = (index) => {
    const s = sensorEdits[index];
    const originalSensor = controller.sensors.find(sen => sen.sensorKey === s.sensorKey);
    if (!originalSensor) return;
    onUpdateSensor({
      controllerKey: controller.controllerKey,
      sensorKey: s.sensorKey,
      type: s.type,
      unit: s.unit,
      description: s.description,
      isActive: s.isActive
    });
  };

  return (
    <div className={`record-card ${controller.isActive ? "active-card" : "inactive-card"}`}>
      <div className="controller-info">
        <div className="controller-key">
          <strong>Controller Key:</strong> {controller.controllerKey}
        </div>

        <div className="details">
          <div>
            <strong>Bed Number:</strong>
            <input value={editBed} onChange={(e) => setEditBed(e.target.value)} />
          </div>

          <div>
            <strong>Firmware Version:</strong>
            <input value={editFirmware} onChange={(e) => setEditFirmware(e.target.value)} />
          </div>

          <div>
            <strong>IP Address:</strong>
            <input value={editIp} onChange={(e) => setEditIp(e.target.value)} />
          </div>

          <div>
            <strong>
              Status:
            </strong>
            <div>
              <input type="checkbox" checked={!!editIsActive} onChange={(e) => setEditIsActive(e.target.checked)} />
              {editIsActive ? "Active" : "Inactive"}
            </div>
          </div>

          <button
            className="update-btn"
            onClick={handleUpdateController}
          >
            Update
          </button>

          <button className="release-btn" onClick={() => onRelease({ controllerKey: controller.controllerKey })}>
            Release
          </button>
        </div>
      </div>

      {/* Assign new sensor */}
      <div className="assign-controls">
        <input placeholder="Type" value={type} onChange={(e) => setType(e.target.value)} />
        <input placeholder="Unit" value={unit} onChange={(e) => setUnit(e.target.value)} />
        <input placeholder="Description" value={description} onChange={(e) => setDescription(e.target.value)} />
        <button onClick={handleAssignSensor}>Assign Sensor</button>
      </div>

      {/* Sensors */}
      <div className="sensor-info">
        {(controller.sensors || []).map((s, i) => (
          <div key={s.sensorKey} className="record-card sensor-card">
            <div className="sensor-key"><strong>Sensor Key:</strong> {s.sensorKey}</div>

            <div className="details">
              <div>
                <strong>Type:</strong>
                <input
                  value={sensorEdits[i]?.type || ""}
                  onChange={(e) => handleSensorChange(i, "type", e.target.value)}
                />
              </div>

              <div>
                <strong>Unit:</strong>
                <input
                  value={sensorEdits[i]?.unit || ""}
                  onChange={(e) => handleSensorChange(i, "unit", e.target.value)}
                />
              </div>

              <div>
                <strong>Description:</strong>
                <input
                  value={sensorEdits[i]?.description || ""}
                  onChange={(e) => handleSensorChange(i, "description", e.target.value)}
                />
              </div>

              <div>
                <strong>
                  Status:
                </strong>
                <div>
                  <input
                    type="checkbox"
                    checked={!!sensorEdits[i]?.isActive}
                    onChange={(e) => handleSensorChange(i, "isActive", e.target.checked)}
                  />
                  {sensorEdits[i]?.isActive ? "Active" : "Inactive"}
                </div>
              </div>

              <button
                className="update-btn"
                onClick={() => handleUpdateSensor(i)}
              >
                Update
              </button>

              <button
                className="release-btn"
                onClick={() => onUnassignSensor({ controllerKey: controller.controllerKey, sensorKey: s.sensorKey })}
              >
                Unassign
              </button>
            </div>
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
  const [showReleasedControllers, setShowReleasedControllers] = useState("all");

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
                onRelease={handleReleaseController}
                onAssignSensor={handleAssignSensor}
                onUnassignSensor={handleUnassignSensor}
                onUpdateController={handleUpdateController}
                onUpdateSensor={handleUpdateSensor}
              />
            ))
          }
        </div>
      </section>
    </div>
  );
}

export default DeviceDetail;
