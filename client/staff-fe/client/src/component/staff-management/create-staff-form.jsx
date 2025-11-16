import React, { useState } from "react";
import "./create-staff-form.css";

function CreateStaffForm({ visible, onClose, addStaff }) {
  const [formData, setFormData] = useState({
    identityNumber: "",
    professionalTitle: "",
    specialization: "",
    avatarUrl: "",
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    addStaff(formData);
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="create-staff-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Create Staff</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Identity Number</label>
              <input
                type="text"
                name="identityNumber"
                value={formData.identityNumber}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Professional Title</label>
              <input
                type="text"
                name="professionalTitle"
                value={formData.professionalTitle}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Specialization</label>
              <input
                type="text"
                name="specialization"
                value={formData.specialization}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Avatar URL</label>
              <input
                type="text"
                name="avatarUrl"
                value={formData.avatarUrl}
                onChange={handleChange}
              />
            </div>
            
            <div className="modal-actions">
              <button type="submit" className="btn create">Create</button>
              <button type="button" className="btn cancel" onClick={onClose}>Cancel</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default CreateStaffForm;
