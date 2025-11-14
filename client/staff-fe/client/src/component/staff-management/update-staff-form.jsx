import React, { useState, useEffect } from "react";
import "./update-staff-form.css";

function UpdateStaffForm({ visible, onClose, staffData, updateStaff, getStaffById }) {
  const emptyLicense = { licenseNumber: "", licenseType: "", issuedBy: "", issueDate: "", expiryDate: "", isValid: true };
  const emptySchedule = { dayOfWeek: "", shiftStart: "", shiftEnd: "", isActive: true };
  const emptyAssignment = { department: "", role: "", startDate: "", endDate: "", isActive: true };
  const emptyExperience = { institution: "", position: "", startYear: "", endYear: "", description: "" };

  const [formData, setFormData] = useState({
    professionalTitle: "",
    specialization: "",
    avatarUrl: "",
    isActive: true,

    fullName: "",
    dateOfBirth: null,
    gender: "",
    address: "",

    staffLicenses: [],
    staffSchedules: [],
    staffAssignments: [],
    staffExperiences: [],
  });

  useEffect(() => {
    if (!visible) return; // Only fetch when form is visible
    if (!staffData?.staffID) return;
    console.log(staffData);
    const fetchStaff = async () => {
      try {
        const latestStaff = await getStaffById({ id: staffData.staffID });
        setFormData({
          professionalTitle: latestStaff.professionalTitle || "",
          specialization: latestStaff.specialization || "",
          avatarUrl: latestStaff.avatarUrl || "",
          isActive: latestStaff.isActive ?? true,

          fullName: latestStaff.fullName || "",
          dateOfBirth: latestStaff.dateOfBirth
            ? latestStaff.dateOfBirth.split("T")[0]
            : "",
          gender: latestStaff.gender || "",
          address: latestStaff.address || "",

          staffLicenses: latestStaff.staffLicenses || [],
          staffSchedules: latestStaff.staffSchedules || [],
          staffAssignments: latestStaff.staffAssignments || [],
          staffExperiences: latestStaff.staffExperiences || [],
        });
      } catch (err) {
        console.error("Failed to fetch staff data:", err);
      }
    };

    fetchStaff();
  }, [staffData]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({ ...prev, [name]: type === "checkbox" ? checked : value }));
  };

  const handleListChange = (listName, index, field, value) => {
    const updatedList = [...formData[listName]];
    updatedList[index][field] = value;
    setFormData(prev => ({ ...prev, [listName]: updatedList }));
  };

  const handleAddItem = (listName, emptyItem) => {
    setFormData(prev => ({ ...prev, [listName]: [...prev[listName], emptyItem] }));
  };

  const handleRemoveItem = (listName, index) => {
    const updatedList = [...formData[listName]];
    updatedList.splice(index, 1);
    setFormData(prev => ({ ...prev, [listName]: updatedList }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!staffData?.staffID) return;
    updateStaff({ id: staffData.staffID, ...formData });
    onClose();
  };

  if (!visible) return null;

  return (
    <div id="update-staff-form">
      <div className="modal-overlay">
        <div className="modal-content">
          <h2>Update Staff Information</h2>
          <form onSubmit={handleSubmit}>
            {/* Top Column */}
            <div className="top-form">
              <div className="form-group">
                <label>Professional Title</label>
                <input type="text" name="professionalTitle" value={formData.professionalTitle} onChange={handleChange} />
              </div>
              <div className="form-group">
                <label>Specialization</label>
                <input type="text" name="specialization" value={formData.specialization} onChange={handleChange} />
              </div>
              <div className="form-group">
                <label>Avatar URL</label>
                <input type="text" name="avatarUrl" value={formData.avatarUrl} onChange={handleChange} />
              </div>
              <div className="form-group checkbox-group">
                <label>
                  <input type="checkbox" name="isActive" checked={formData.isActive} onChange={handleChange} />
                  Active
                </label>
              </div>

              <div className="form-group">
                <label>Address</label>
                <input type="text" name="address" value={formData.address} onChange={handleChange} />
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
              <div className="form-group">
                <label>Birthdate</label>
                <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} />
              </div>
              <div className="form-group">
                <label>Name</label>
                <input type="text" name="fullName" value={formData.fullName} onChange={handleChange} />
              </div>
            </div>

            <div className="bottom-form">
              {/* Staff Licenses */}
              <div className="form-group">
                <label>Licenses</label>
                {formData.staffLicenses.map((item, i) => (
                  <div key={i} className="sub-item">
                    <div className="field-group">
                      <label>License Number</label>
                      <input type="text" value={item.licenseNumber} onChange={e => handleListChange("staffLicenses", i, "licenseNumber", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>License Type</label>
                      <input type="text" value={item.licenseType} onChange={e => handleListChange("staffLicenses", i, "licenseType", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Issued By</label>
                      <input type="text" value={item.issuedBy} onChange={e => handleListChange("staffLicenses", i, "issuedBy", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Issue Date</label>
                      <input type="date" value={item.issueDate ? new Date(item.issueDate).toISOString().split("T")[0] : ""} onChange={e => handleListChange("staffLicenses", i, "issueDate", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Expiry Date</label>
                      <input type="date" value={item.expiryDate ? new Date(item.expiryDate).toISOString().split("T")[0] : ""} onChange={e => handleListChange("staffLicenses", i, "expiryDate", e.target.value)} />
                    </div>
                    <div className="field-group checkbox-group">
                      <label>
                        <input type="checkbox" checked={item.isValid} onChange={e => handleListChange("staffLicenses", i, "isValid", e.target.checked)} /> Valid
                      </label>
                    </div>
                    <button type="button" onClick={() => handleRemoveItem("staffLicenses", i)}>Remove</button>
                  </div>
                ))}
                <button type="button" onClick={() => handleAddItem("staffLicenses", emptyLicense)}>Add License</button>
              </div>

              {/* Staff Schedules */}
              <div className="form-group">
                <label>Schedules</label>
                {formData.staffSchedules.map((item, i) => (
                  <div key={i} className="sub-item">
                    <div className="field-group">
                      <label>Day of Week</label>
                      <input type="text" value={item.dayOfWeek} onChange={e => handleListChange("staffSchedules", i, "dayOfWeek", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Shift Start</label>
                      <input type="time" value={item.shiftStart} onChange={e => handleListChange("staffSchedules", i, "shiftStart", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Shift End</label>
                      <input type="time" value={item.shiftEnd} onChange={e => handleListChange("staffSchedules", i, "shiftEnd", e.target.value)} />
                    </div>
                    <div className="field-group checkbox-group">
                      <label>
                        <input type="checkbox" checked={item.isActive} onChange={e => handleListChange("staffSchedules", i, "isActive", e.target.checked)} /> Active
                      </label>
                    </div>
                    <button type="button" onClick={() => handleRemoveItem("staffSchedules", i)}>Remove</button>
                  </div>
                ))}
                <button type="button" onClick={() => handleAddItem("staffSchedules", emptySchedule)}>Add Schedule</button>
              </div>

              {/* Staff Assignments */}
              <div className="form-group">
                <label>Assignments</label>
                {formData.staffAssignments.map((item, i) => (
                  <div key={i} className="sub-item">
                    <div className="field-group">
                      <label>Department</label>
                      <input type="text" value={item.department} onChange={e => handleListChange("staffAssignments", i, "department", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Role</label>
                      <input type="text" value={item.role} onChange={e => handleListChange("staffAssignments", i, "role", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Start Date</label>
                      <input type="date" value={item.startDate} onChange={e => handleListChange("staffAssignments", i, "startDate", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>End Date</label>
                      <input type="date" value={item.endDate} onChange={e => handleListChange("staffAssignments", i, "endDate", e.target.value)} />
                    </div>
                    <div className="field-group checkbox-group">
                      <label>
                        <input type="checkbox" checked={item.isActive} onChange={e => handleListChange("staffAssignments", i, "isActive", e.target.checked)} /> Active
                      </label>
                    </div>
                    <button type="button" onClick={() => handleRemoveItem("staffAssignments", i)}>Remove</button>
                  </div>
                ))}
                <button type="button" onClick={() => handleAddItem("staffAssignments", emptyAssignment)}>Add Assignment</button>
              </div>

              {/* Staff Experiences */}
              <div className="form-group">
                <label>Experiences</label>
                {formData.staffExperiences.map((item, i) => (
                  <div key={i} className="sub-item">
                    <div className="field-group">
                      <label>Institution</label>
                      <input type="text" value={item.institution} onChange={e => handleListChange("staffExperiences", i, "institution", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Position</label>
                      <input type="text" value={item.position} onChange={e => handleListChange("staffExperiences", i, "position", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Start Year</label>
                      <input type="number" value={item.startYear} onChange={e => handleListChange("staffExperiences", i, "startYear", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>End Year</label>
                      <input type="number" value={item.endYear} onChange={e => handleListChange("staffExperiences", i, "endYear", e.target.value)} />
                    </div>
                    <div className="field-group">
                      <label>Description</label>
                      <input type="text" value={item.description} onChange={e => handleListChange("staffExperiences", i, "description", e.target.value)} />
                    </div>
                    <button type="button" onClick={() => handleRemoveItem("staffExperiences", i)}>Remove</button>
                  </div>
                ))}
                <button type="button" onClick={() => handleAddItem("staffExperiences", emptyExperience)}>Add Experience</button>
              </div>

              {/* Actions */}
              <div className="modal-actions">
                <button type="submit" className="btn update">Update</button>
                <button type="button" className="btn cancel" onClick={onClose}>Cancel</button>
              </div>

            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default UpdateStaffForm;
