import React, { useState, useEffect } from "react";
import "./update-privilege-form.css";

function UpdatePrivilegeForm({ visible, onClose, privilegeData, updatePrivilege }) {
  const [formData, setFormData] = useState({ name: "", description: "" });

  useEffect(() => {
    if (privilegeData) {
      setFormData({
        name: privilegeData.name || "",
        description: privilegeData.description || "",
      });
    }
  }, [privilegeData]);

  if (!visible) return null;

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!privilegeData?.privilegeID) return;
    updatePrivilege({
      id: privilegeData.privilegeID,
      ...formData,
    });
    onClose();
  };

  return (
    <div id="update-privilege-form">
      <div className="modal-overlay" onClick={onClose}></div>
      <div className="modal-content">
        <h2>Update Privilege</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Name</label>
            <input name="name" value={formData.name} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label>Description</label>
            <textarea name="description" value={formData.description} onChange={handleChange} />
          </div>

          <div className="modal-actions">
            <button type="submit" className="btn update">Update</button>
            <button type="button" className="btn cancel" onClick={onClose}>Cancel</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default UpdatePrivilegeForm;
