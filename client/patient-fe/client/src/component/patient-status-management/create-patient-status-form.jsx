import React, { useState } from "react";
import "./create-patient-status-form.css";

function CreatePatientStatusForm({ visible, onClose, addPatientStatus }) {
  const [formData, setFormData] = useState({
    patientStatusCode: "",
    name: "",
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
    await addPatientStatus(formData);
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="create-patient-status-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Create Patient Status</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Status Code</label>
              <input
                type="text"
                name="patientStatusCode"
                value={formData.patientStatusCode}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Status Name</label>
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Description</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleChange}
                rows="3"
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

export default CreatePatientStatusForm;
