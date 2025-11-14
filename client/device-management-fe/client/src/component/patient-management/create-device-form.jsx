import React, { useState } from "react";
import "./create-device-form.css";

function CreateDeviceForm({ visible, onClose, addDevice }) {
  const [formData, setFormData] = useState({
    roomName: "",
    ipAddress: "",
    description: "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await addDevice(formData);
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="create-device-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Create Device</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Room Name</label>
              <input
                type="text"
                name="roomName"
                value={formData.roomName}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>IP Address</label>
              <input
                type="text"
                name="ipAddress"
                value={formData.ipAddress}
                onChange={handleChange}
                required
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
              <button type="submit" className="btn create">
                Create
              </button>
              <button type="button" className="btn cancel" onClick={onClose}>
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default CreateDeviceForm;
