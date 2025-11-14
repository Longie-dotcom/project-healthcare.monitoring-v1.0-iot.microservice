import React, { useState } from "react";
import "./create-patient-form.css";

function CreatePatientForm({ visible, onClose, addPatient }) {
  const [formData, setFormData] = useState({
    identityNumber: "",
    admissionDate: "",
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
    await addPatient(formData);
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="create-patient-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Create Patient</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Identity Number</label>
              <input
                type="text"
                name="identityNumber"
                value={formData.identityNumber}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Admission Date</label>
              <input
                type="date"
                name="admissionDate"
                value={formData.admissionDate}
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

export default CreatePatientForm;
