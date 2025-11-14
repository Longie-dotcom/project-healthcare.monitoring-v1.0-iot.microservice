import React, { useEffect, useState } from "react";
import "./update-device-form.css";

function UpdateDeviceForm({ visible, onClose, deviceData, updateDevice }) {
  const [formData, setFormData] = useState({
    roomName: "",
    ipAddress: "",
    description: "",
  });

  // Load device data
  useEffect(() => {
    if (deviceData) {
      setFormData({
        roomName: deviceData.roomName || "",
        ipAddress: deviceData.ipAddress || "",
        description: deviceData.description || "",
      });
    }
  }, [deviceData]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await updateDevice({
      edgeDeviceID: deviceData.edgeDeviceID,
      roomName: formData.roomName,
      ipAddress: formData.ipAddress,
      description: formData.description,
    });
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="update-device-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Update Device</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Room Name</label>
              <input
                type="text"
                name="roomName"
                value={formData.roomName}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>IP Address</label>
              <input
                type="text"
                name="ipAddress"
                value={formData.ipAddress}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Description</label>
              <input
                type="text"
                name="description"
                value={formData.description}
                onChange={handleChange}
              />
            </div>

            <div className="modal-actions">
              <button type="submit" className="btn update">
                Update
              </button>
              <button
                type="button"
                className="btn cancel"
                onClick={onClose}
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default UpdateDeviceForm;
