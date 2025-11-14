import React, { useEffect, useState } from "react";
import "./update-patient-form.css";

function UpdatePatientForm({
  visible,
  onClose,
  patientData,
  updatePatient,
  statuses
}) {
  const [formData, setFormData] = useState({
    status: "",
    admissionDate: "",
    dischargeDate: "",
    fullName: "",
    dateOfBirth: "",
    address: "",
    gender: "",
  });

  // Load patient data
  useEffect(() => {
    const loadData = async () => {
      if (patientData) {
        setFormData({
          status: patientData.status || "",
          admissionDate: patientData.admissionDate
            ? patientData.admissionDate.split("T")[0]
            : "",
          dischargeDate: patientData.dischargeDate
            ? patientData.dischargeDate.split("T")[0]
            : "",
          fullName: patientData.fullName || "",
          dateOfBirth: patientData.dateOfBirth
            ? patientData.dateOfBirth.split("T")[0]
            : "",
          address: patientData.address || "",
          gender: patientData.gender || "",
        });
      }
    };

    loadData();
  }, [patientData]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await updatePatient({
      id: patientData.patientID,
      status: formData.status,
      admissionDate: formData.admissionDate || null,
      dischargeDate: formData.dischargeDate || null,
      fullName: formData.fullName,
      dateOfBirth: formData.dateOfBirth || null,
      address: formData.address,
      gender: formData.gender,
    });
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="update-patient-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Update Patient</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Status</label>
              <select
                name="status"
                value={formData.status}
                onChange={handleChange}
                required
              >
                <option value="">-- Select Status --</option>
                {statuses.length > 0 ? (
                  statuses.map((s) => (
                    <option key={s.patientStatusCode} value={s.patientStatusCode}>
                      {s.name}
                    </option>
                  ))
                ) : (
                  <option value="" disabled>
                    Status list cannot be loaded
                  </option>
                )}
              </select>
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

            <div className="form-group">
              <label>Discharge Date</label>
              <input
                type="date"
                name="dischargeDate"
                value={formData.dischargeDate}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Full Name</label>
              <input
                type="text"
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Date of Birth</label>
              <input
                type="date"
                name="dateOfBirth"
                value={formData.dateOfBirth}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Address</label>
              <input
                type="text"
                name="address"
                value={formData.address}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label>Gender</label>
              <select
                name="gender"
                value={formData.gender}
                onChange={handleChange}
                required
              >
                <option value="Female">
                  Female
                </option>
                <option value="Male">
                  Male
                </option>
              </select>
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

export default UpdatePatientForm;
