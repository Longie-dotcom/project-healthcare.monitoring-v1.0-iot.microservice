import React, { useState, useEffect } from "react";
import "./update-user-form.css";

function UpdateUserForm({
  visible,
  onClose,
  userId,
  updateUser,
  getUserById,
  getRoleList,
  getPrivilegeList,
}) {
  const [loading, setLoading] = useState(false);
  const [roles, setRoles] = useState([]);
  const [privileges, setPrivileges] = useState([]);
  const [formData, setFormData] = useState({
    email: "",
    phoneNumber: "",
    fullName: "",
    gender: "Female",
    address: "",
    dateOfBirth: "",
  });

  const [roleStates, setRoleStates] = useState({});
  const [privilegeStates, setPrivilegeStates] = useState({});

  useEffect(() => {
    if (!visible || !userId) return;

    const fetchData = async () => {
      setLoading(true);
      try {
        const [user, allRoles, allPrivileges] = await Promise.all([
          getUserById({ id: userId }),
          getRoleList(),
          getPrivilegeList(),
        ]);

        // Fill basic info
        setFormData({
          email: user.email || "",
          phoneNumber: user.phoneNumber || "",
          fullName: user.fullName || "",
          gender: user.gender || "Female",
          address: user.address || "",
          dateOfBirth: user.dateOfBirth ? user.dateOfBirth.split("T")[0] : "",
        });

        // Store master lists (unchanged)
        setRoles(allRoles);
        setPrivileges(allPrivileges);

        // -------------------------------
        // Map only user's assigned roles
        // -------------------------------
        const userRoleMap = {};
        user.userRoles?.forEach((ur) => {
          const id = ur.role?.roleID;
          if (id) userRoleMap[id] = ur.isActive;
        });
        setRoleStates(userRoleMap);

        // -------------------------------
        // Map only user's assigned privileges
        // -------------------------------
        const userPrivMap = {};
        user.userPrvileges?.forEach((up) => {
          const id = up.privilege?.privilegeID;
          if (id) userPrivMap[id] = up.isGranted;
        });
        setPrivilegeStates(userPrivMap);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [visible, userId]);

  // --- input change
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  // --- submit handler
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!userId) return;

    const userRoleUpdateDTOs = Object.entries(roleStates).map(([roleID, isActive]) => ({
      roleID,
      isActive,
    }));

    const userPrivilegeUpdateDTOs = Object.entries(privilegeStates).map(
      ([privilegeID, isGranted]) => ({
        privilegeID,
        isGranted,
      })
    );

    await updateUser({
      id: userId,
      fullName: formData.fullName,
      dateOfBirth: formData.dateOfBirth,
      gender: formData.gender,
      address: formData.address,
      email: formData.email,
      phoneNumber: formData.phoneNumber,
      userRoleUpdateDTOs,
      userPrivilegeUpdateDTOs,
    });

    onClose();
  };

  if (!visible) return null;

  return (
    <div id="update-user-form">
      <div className="modal-overlay" onClick={onClose}></div>

      <div className="modal-content animate-slide">
        <h2>Update User Information</h2>

        {loading ? (
          <p>Loading...</p>
        ) : (
          <form onSubmit={handleSubmit}>
            <div className="form-sections">
              {/* Basic info */}
              <div className="section">
                <div className="top">
                  <h3>👤 Basic Info</h3>
                </div>
                <div className="bottom">
                  <div className="info-grid">
                    <div className="info-grid-col">
                      <div className="info-grid-group">
                        <label>Full Name</label>
                        <input
                          type="text"
                          name="fullName"
                          value={formData.fullName}
                          onChange={handleChange}
                        />
                      </div>
                      <div className="info-grid-group">
                        <label>Email</label>
                        <input
                          type="email"
                          name="email"
                          value={formData.email}
                          onChange={handleChange}
                        />
                      </div>
                      <div className="info-grid-group">
                        <label>Phone</label>
                        <input
                          type="text"
                          name="phoneNumber"
                          value={formData.phoneNumber}
                          onChange={handleChange}
                        />
                      </div>
                    </div>
                    <div className="info-grid-col">
                      <div className="info-grid-group">
                        <label>Gender</label>
                        <select
                          name="gender"
                          value={formData.gender}
                          onChange={handleChange}
                        >
                          <option value="Female">Female</option>
                          <option value="Male">Male</option>
                        </select>
                      </div>
                      <div className="info-grid-group">
                        <label>DOB</label>
                        <input
                          type="date"
                          name="dateOfBirth"
                          value={formData.dateOfBirth}
                          onChange={handleChange}
                        />
                      </div>
                      <div className="info-grid-group">
                        <label>Address</label>
                        <input
                          type="text"
                          name="address"
                          value={formData.address}
                          onChange={handleChange}
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* 🧱 Roles Section */}
              <div className="section role">
                <div className="top">
                  <h3>
                    Role Assignment
                  </h3>
                </div>

                {roles ? (
                  <div className="bottom">
                    {/* Master Role List */}
                    <div className="section-col">
                      <h4>All Roles</h4>
                      <div className="checkbox-grid">
                        {roles.map((r) => (
                          <div
                            key={r.roleID}
                            className={`checkbox-item ${roleStates.hasOwnProperty(r.roleID) ? "active" : ""}`}
                            onClick={() => {
                              setRoleStates((prev) => {
                                if (prev.hasOwnProperty(r.roleID)) {
                                  const copy = { ...prev };
                                  delete copy[r.roleID];
                                  return copy;
                                } else {
                                  return { ...prev, [r.roleID]: true };
                                }
                              });
                            }}
                          >
                            {r.name}
                          </div>
                        ))}
                      </div>
                    </div>

                    {/* User’s Roles */}
                    <div className="section-col">
                      <h4>Role Assignment</h4>
                      <div className="user-list">
                        {Object.keys(roleStates).length === 0 && (
                          <p className="empty-note">No roles assigned.</p>
                        )}
                        {roles
                          .filter((r) => roleStates[r.roleID] !== undefined)
                          .map((r) => (
                            <div key={r.roleID} className="user-item">
                              <span>{r.name}</span>
                              <div className="actions">
                                <button
                                  type="button"
                                  className={`btn-mini ${roleStates[r.roleID] ? "active" : "inactive"
                                    }`}
                                  onClick={() =>
                                    setRoleStates((prev) => ({
                                      ...prev,
                                      [r.roleID]: !prev[r.roleID],
                                    }))
                                  }
                                >
                                  {roleStates[r.roleID] ? "Active" : "Inactive"}
                                </button>
                                <button
                                  type="button"
                                  className="btn-mini remove"
                                  onClick={() =>
                                    setRoleStates((prev) => {
                                      const copy = { ...prev };
                                      delete copy[r.roleID];
                                      return copy;
                                    })
                                  }
                                >
                                  Remove
                                </button>
                              </div>
                            </div>
                          ))}
                      </div>
                    </div>
                  </div>
                ) : (
                  <div className="empty">
                    The role list is empty
                  </div>
                )}
              </div>

              {/* 🛡️ Privileges Section */}
              <div className="section privilege">
                <div className="top">
                  <h4>Privilege customization</h4>
                </div>

                {privileges ? (
                  <div className="bottom">
                    <div className="section-col">
                      <h4>All Privileges</h4>
                      <div className="checkbox-grid scroll">
                        {privileges.map((p) => (
                          <div
                            key={p.privilegeID}
                            className={`checkbox-item ${privilegeStates.hasOwnProperty(p.privilegeID) ? "active" : ""
                              }`}
                            onClick={() => {
                              setPrivilegeStates((prev) => {
                                if (prev.hasOwnProperty(p.privilegeID)) {
                                  const copy = { ...prev };
                                  delete copy[p.privilegeID];
                                  return copy;
                                } else {
                                  return { ...prev, [p.privilegeID]: true };
                                }
                              });
                            }}
                          >
                            {p.name}
                          </div>
                        ))}
                      </div>
                    </div>

                    <div className="section-col">
                      {/* User’s Privileges */}
                      <h4>Assigned Privileges</h4>
                      <div className="user-list scroll">
                        {Object.keys(privilegeStates).length === 0 && (
                          <p className="empty-note">No privileges assigned.</p>
                        )}
                        {privileges
                          .filter((p) => privilegeStates[p.privilegeID] !== undefined)
                          .map((p) => (
                            <div key={p.privilegeID} className="user-item">
                              <span>{p.name}</span>
                              <div className="actions">
                                <button
                                  type="button"
                                  className={`btn-mini ${privilegeStates[p.privilegeID] ? "granted" : "revoked"
                                    }`}
                                  onClick={() =>
                                    setPrivilegeStates((prev) => ({
                                      ...prev,
                                      [p.privilegeID]: !prev[p.privilegeID],
                                    }))
                                  }
                                >
                                  {privilegeStates[p.privilegeID] ? "Granted" : "Revoked"}
                                </button>
                                <button
                                  type="button"
                                  className="btn-mini remove"
                                  onClick={() =>
                                    setPrivilegeStates((prev) => {
                                      const copy = { ...prev };
                                      delete copy[p.privilegeID];
                                      return copy;
                                    })
                                  }
                                >
                                  Remove
                                </button>
                              </div>
                            </div>
                          ))}
                      </div>
                    </div>
                  </div>
                ) : (
                  <div className="empty">
                    The privileges list is empty
                  </div>
                )}
              </div>

              {/* Actions */}
              <div className="modal-actions">
                <button type="submit" className="btn update">
                  Save Changes
                </button>
                <button
                  type="button"
                  className="btn cancel"
                  onClick={onClose}
                >
                  Cancel
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

export default UpdateUserForm;
