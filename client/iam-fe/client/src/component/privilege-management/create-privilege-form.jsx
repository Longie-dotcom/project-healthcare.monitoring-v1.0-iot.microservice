import React, { useState } from "react";
import "./create-privilege-form.css";

function CreatePrivilegeForm({ visible, onClose, addPrivilege }) {
  const [formData, setFormData] = useState({ name: "", description: "" });

  if (!visible) return null;

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    addPrivilege(formData);
    onClose();
  };

  return (
    <div id="create-privilege-form">
      <div className="modal-overlay" onClick={onClose}></div>
      <div className="modal-content">
        <h2>Create Privilege</h2>
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
            <button type="submit" className="btn create">Create</button>
            <button type="button" className="btn cancel" onClick={onClose}>Cancel</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreatePrivilegeForm;
