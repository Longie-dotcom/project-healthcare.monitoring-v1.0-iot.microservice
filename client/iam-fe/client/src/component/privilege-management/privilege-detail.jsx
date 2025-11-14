import React from "react";
import "./privilege-detail.css";

function PrivilegeDetail({ visible, onClose, privilege }) {
  if (!visible || !privilege) return null;

  return (
    <div id="privilege-detail">
      <div className="modal-overlay" onClick={onClose}></div>
      <div className="modal-content animate-slide">
        <div className="modal-header">
          <h2>Privilege Detail</h2>
          <button className="close-btn" onClick={onClose}>×</button>
        </div>

        <div className="modal-body">
          <div><strong>Name:</strong> {privilege.name}</div>
          <div><strong>Description:</strong> {privilege.description || "—"}</div>
        </div>

        <div className="modal-actions">
          <button className="btn-close" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
}

export default PrivilegeDetail;
