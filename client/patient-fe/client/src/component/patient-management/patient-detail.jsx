import React, { useState } from "react";
import "./patient-detail.css";

function PatientDetail({
  patient,
  assignBed,
  releaseBed,
  assignStaff,
  unassignStaff,
  setReloadDetail,
  onClose,
}) {
  if (!patient) return null;

  const [bedControllerKey, setBedControllerKey] = useState("");
  const [staffIdentityNumber, setStaffIdentityNumber] = useState("");
  const [showReleasedBeds, setShowReleasedBeds] = useState(false);
  const [showReleasedStaff, setShowReleasedStaff] = useState(false);

  const formatDate = (date) => (date ? date.split("T")[0] : "");

  const handleAssignBed = async () => {
    if (!bedControllerKey) return;
    await assignBed({
      patientId: patient.patientID,
      controllerKey: bedControllerKey,
    });
    setBedControllerKey("");
    setReloadDetail((prev) => prev + 1);
  };

  const handleReleaseBed = async ({ patientBedAssignmentID }) => {
    await releaseBed({
      patientId: patient.patientID,
      patientBedAssignmentID,
    });
    setReloadDetail((prev) => prev + 1);
  };

  const handleAssignStaff = async () => {
    if (!staffIdentityNumber) return;
    await assignStaff({
      patientId: patient.patientID,
      staffIdentityNumber,
    });
    setStaffIdentityNumber("");
    setReloadDetail((prev) => prev + 1);
  };

  const handleUnassignStaff = async (s) => {
    await unassignStaff({
      patientId: patient.patientID,
      patientStaffAssignmentID: s.patientStaffAssignmentID,
    });
    setReloadDetail((prev) => prev + 1);
  };

  return (
    <div id="patient-detail-child">
      <div className="header">
        <h3>👤 {patient.fullName} Details</h3>
        <button onClick={onClose}>Close detail</button>
      </div>

      {/* Basic info */}
      <div className="info-grid">
        <div>
          <div><strong>Identity Number:</strong> {patient.identityNumber}</div>
          <div><strong>Name:</strong> {patient.fullName}</div>
          <div><strong>Phone:</strong> {patient.phoneNumber}</div>
          <div><strong>Address:</strong> {patient.address}</div>
          <div><strong>Email:</strong> {patient.email}</div>
          <div><strong>Birthdate:</strong> {formatDate(patient.dateOfBirth)}</div>
          <div><strong>Gender:</strong> {patient.gender}</div>
        </div>
        <div>
          <div><strong>Patient Code:</strong> {patient.patientCode}</div>
          <div><strong>Status:</strong> {patient.status}</div>
          <div><strong>Admission Date:</strong> {formatDate(patient.admissionDate)}</div>
          <div><strong>Discharge Date:</strong> {formatDate(patient.dischargeDate)}</div>
          <div><strong>Activation:</strong> {patient.isActive ? "Active" : "Inactive"}</div>
        </div>
      </div>

      {/* Bed assignment */}
      <section className="section">
        <h4>🛏️ Bed Assignment</h4>
        <div className="assign-controls">
          <input
            type="text"
            placeholder="Controller Key"
            value={bedControllerKey}
            onChange={(e) => setBedControllerKey(e.target.value)}
          />
          <button onClick={handleAssignBed}>Assign Bed</button>
        </div>

        {/* Active beds */}
        <div>
          {[...(patient.patientBedAssignments || [])]
            .filter((b) => b.isActive)
            .map((b, idx) => (
              <div key={idx} className="record-card active-card">
                <div><strong>Controller Key:</strong> {b.controllerKey}</div>
                <div><strong>Assigned At:</strong> {formatDate(b.assignedAt)}</div>
                <div><strong>Status:</strong> Active</div>
                <button
                  className="btn release"
                  onClick={() => handleReleaseBed({ patientBedAssignmentID: b.patientBedAssignmentID })}
                >
                  Release
                </button>
              </div>
            ))}
        </div>

        {/* Toggle released beds */}
        <button
          className="toggle-btn"
          onClick={() => setShowReleasedBeds(!showReleasedBeds)}
        >
          {showReleasedBeds ? "Hide Released Beds" : "Show Released Beds"}
        </button>

        {showReleasedBeds && (
          <div className="scrollable-list">
            {[...(patient.patientBedAssignments || [])]
              .filter((b) => !b.isActive)
              .map((b, idx) => (
                <div key={idx} className="record-card inactive-card">
                  <div><strong>Controller Key:</strong> {b.controllerKey}</div>
                  <div><strong>Released At:</strong> {formatDate(b.releasedAt)}</div>
                  <div><strong>Status:</strong> Released</div>
                  <div><strong>Assigned At:</strong> {formatDate(b.assignedAt)}</div>
                </div>
              ))}
          </div>
        )}
      </section>

      {/* Staff assignment */}
      <section className="section">
        <h4>👨‍⚕️ Staff Assignment</h4>
        <div className="assign-controls">
          <input
            type="text"
            placeholder="Staff Identity Number"
            value={staffIdentityNumber}
            onChange={(e) => setStaffIdentityNumber(e.target.value)}
          />
          <button onClick={handleAssignStaff}>Assign Staff</button>
        </div>

        {/* Active staff */}
        <div>
          {[...(patient.patientStaffAssignments || [])]
            .filter((s) => s.isActive)
            .map((s, idx) => (
              <div key={idx} className="record-card active-card">
                <div><strong>Staff identity number:</strong> {s.staffIdentityNumber}</div>
                <div><strong>Assigned At:</strong> {formatDate(s.assignedAt)}</div>
                <div><strong>Status:</strong> Active</div>
                <button
                  className="btn unassign"
                  onClick={() => handleUnassignStaff(s)}
                >
                  Unassign
                </button>
              </div>
            ))}
        </div>

        {/* Toggle released staff */}
        <button
          className="toggle-btn"
          onClick={() => setShowReleasedStaff(!showReleasedStaff)}
        >
          {showReleasedStaff ? "Hide Unassigned Staff" : "Show Unassigned Staff"}
        </button>

        {showReleasedStaff && (
          <div className="scrollable-list">
            {[...(patient.patientStaffAssignments || [])]
              .filter((s) => !s.isActive)
              .map((s, idx) => (
                <div key={idx} className="record-card inactive-card">
                  <div><strong>Staff identity number:</strong> {s.staffIdentityNumber}</div>
                  <div><strong>Unassigned At:</strong> {formatDate(s.unassignedAt)}</div>
                  <div><strong>Status:</strong> Unassigned</div>
                </div>
              ))}
          </div>
        )}
      </section>
    </div>
  );
}

export default PatientDetail;
