import React, { useEffect, useState } from "react";
import "./update-patient-status-form.css";

function UpdatePatientStatusForm({
  visible,
  onClose,
  patientStatus,
  updatePatientStatus,
}) {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
  });

  // Fetch current patient status details when modal opens
  useEffect(() => {
      if (!patientStatus || !visible) return;
        setFormData({
          name: patientStatus.name || "",
          description: patientStatus.description || "",
        });
  }, [patientStatus, visible]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await updatePatientStatus({
      code: patientStatus.patientStatusCode,
      ...formData,
    });
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="update-patient-status-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Update Patient Status</h2>
            <form onSubmit={handleSubmit}>
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

export default UpdatePatientStatusForm;
