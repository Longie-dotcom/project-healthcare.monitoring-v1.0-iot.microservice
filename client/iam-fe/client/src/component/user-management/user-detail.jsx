import React from "react";
import "./user-detail.css";

function UserDetail({ visible, onClose, user }) {
  if (!visible || !user) return null;

  return (
    <div id="user-detail">
      <div className="modal-overlay" onClick={onClose}></div>

      <div className="modal-content animate-slide">
        <div className="modal-header">
          <h2>User Detail</h2>
          <button className="close-btn" onClick={onClose}>
            ×
          </button>
        </div>

        <div className="modal-body">
          {/* BASIC INFO */}
          <section className="section">
            <h3>👤 Basic Information</h3>
            <div className="info-grid">
              <div><strong>Full Name:</strong> {user.fullName}</div>
              <div><strong>Email:</strong> {user.email}</div>
              <div><strong>Phone:</strong> {user.phoneNumber}</div>
              <div><strong>Gender:</strong> {user.gender}</div>
              <div><strong>Address:</strong> {user.address}</div>
              <div><strong>Date of Birth:</strong> {user.dateOfBirth?.split("T")[0]}</div>
              <div><strong>Age:</strong> {user.age}</div>
              <div><strong>Identity Number:</strong> {user.identityNumber}</div>
              <div><strong>Status:</strong> {user.isActive ? "Active" : "Inactive"}</div>
            </div>
          </section>

          {/* ROLES */}
          <section className="section">
            <h3>🛡️ Roles</h3>
            {user.userRoles?.length > 0 ? (
              <div className="roles-container">
                {user.userRoles.map((userRole, index) => {
                  const role = userRole.role;
                  return (
                    <div key={index} className={`role-card-detail ${!userRole.isActive ? "inactive" : ""}`}>
                      <div className="role-header">
                        <h4>{role.name}</h4>
                        <span className="role-code">{role.roleCode}</span>
                      </div>
                      <p className="role-desc">{role.description}</p>
                      <p className="role-status">
                        Status: <strong>{userRole.isActive ? "Active" : "Inactive"}</strong>
                      </p>
                    </div>
                  );
                })}
              </div>
            ) : (
              <p className="no-roles">No roles assigned</p>
            )}
          </section>

          {/* PRIVILEGES */}
          <section className="section">
            <h3>🔑 Privileges</h3>
            {user.userPrvileges?.length > 0 ? (
              <ul className="privilege-list">
                {user.userPrvileges.map((p, index) => (
                  <li key={index}>
                    <span className="privilege-name">{p.privilege.name} — {p.privilege.description}</span>
                    <span className={`privilege-status ${p.isGranted ? 'granted' : 'revoked'}`}>
                      {p.isGranted ? "Granted" : "Revoked"}
                    </span>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="no-privileges">No privileges assigned</p>
            )}
          </section>
        </div>

        <div className="modal-actions">
          <button className="btn-close" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}

export default UserDetail;
