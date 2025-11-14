import React from "react";
import "./staff-detail.css";

function StaffDetail({ visible, onClose, staff }) {
  if (!visible || !staff) return null;

  const formatDate = (date) => date ? date.split("T")[0] : "";
  const formatTime = (time) => time ? time : "";
  console.log(staff);
  return (
    <div id="staff-detail">
      <div className="modal-overlay" onClick={onClose}></div>

      <div className="modal-content animate-slide">
        <div className="modal-header">
          <h2>Staff Detail</h2>
          <button className="close-btn" onClick={onClose}>×</button>
        </div>

        <div className="modal-body">
          {/* Basic Info */}
          <section className="section">
            <h3>👤 Basic Information</h3>
            <div className="info-grid">
              <div><strong>Staff Code:</strong> {staff.staffCode}</div>
              <div><strong>Full Name:</strong> {staff.fullName}</div>
              <div><strong>Email:</strong> {staff.email}</div>
              <div><strong>Phone:</strong> {staff.phoneNumber}</div>
              <div><strong>Gender:</strong> {staff.gender}</div>
              <div><strong>Address:</strong> {staff.address}</div>
              <div><strong>Date of Birth:</strong> {formatDate(staff.dateOfBirth)}</div>
              <div><strong>Professional Title:</strong> {staff.professionalTitle}</div>
              <div><strong>Specialization:</strong> {staff.specialization}</div>
              <div><strong>Active:</strong> {staff.isActive ? "Yes" : "No"}</div>
            </div>
          </section>

          {/* Assignments */}
          <section className="section">
            <h3>🏢 Assignments</h3>
            {staff.staffAssignments?.length > 0 ? (
              staff.staffAssignments.map((a, idx) => (
                <div key={idx} className="record-card">
                  <div><strong>Department:</strong> {a.department}</div>
                  <div><strong>Role:</strong> {a.role}</div>
                  <div><strong>Start:</strong> {formatDate(a.startDate)}</div>
                  <div><strong>End:</strong> {formatDate(a.endDate)}</div>
                  <div><strong>Active:</strong> {a.isActive ? "Yes" : "No"}</div>
                </div>
              ))
            ) : <p className="no-records">No assignments</p>}
          </section>

          {/* Experiences */}
          <section className="section">
            <h3>💼 Experiences</h3>
            {staff.staffExperiences?.length > 0 ? (
              staff.staffExperiences.map((e, idx) => (
                <div key={idx} className="record-card">
                  <div><strong>Institution:</strong> {e.institution}</div>
                  <div><strong>Position:</strong> {e.position}</div>
                  <div><strong>Start Year:</strong> {e.startYear}</div>
                  <div><strong>End Year:</strong> {e.endYear}</div>
                  {e.description && <div><strong>Description:</strong> {e.description}</div>}
                </div>
              ))
            ) : <p className="no-records">No experiences</p>}
          </section>

          {/* Licenses */}
          <section className="section">
            <h3>🛡️ Licenses</h3>
            {staff.staffLicenses?.length > 0 ? (
              staff.staffLicenses.map((l, idx) => (
                <div key={idx} className="record-card">
                  <div><strong>License Number:</strong> {l.licenseNumber}</div>
                  <div><strong>Type:</strong> {l.licenseType}</div>
                  <div><strong>Issued By:</strong> {l.issuedBy}</div>
                  <div><strong>Issue Date:</strong> {formatDate(l.issueDate)}</div>
                  <div><strong>Expiry Date:</strong> {formatDate(l.expiryDate)}</div>
                  <div><strong>Valid:</strong> {l.isValid ? "Yes" : "No"}</div>
                </div>
              ))
            ) : <p className="no-records">No licenses</p>}
          </section>

          {/* Schedules */}
          <section className="section">
            <h3>⏰ Schedules</h3>
            {staff.staffSchedules?.length > 0 ? (
              staff.staffSchedules.map((s, idx) => (
                <div key={idx} className="record-card">
                  <div><strong>Day:</strong> {s.dayOfWeek}</div>
                  <div><strong>Shift:</strong> {formatTime(s.shiftStart)} - {formatTime(s.shiftEnd)}</div>
                  <div><strong>Active:</strong> {s.isActive ? "Yes" : "No"}</div>
                </div>
              ))
            ) : <p className="no-records">No schedules</p>}
          </section>
        </div>

        <div className="modal-actions">
          <button className="btn-close" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
}

export default StaffDetail;
