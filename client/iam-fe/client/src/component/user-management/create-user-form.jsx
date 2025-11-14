import React, { useState, useEffect } from "react";
import "./create-user-form.css";
import { getRoleList } from "../../api/useRoles";

function CreateUserForm({ visible, onClose, addUser }) {
    const [formData, setFormData] = useState({
        roleCodes: [],
        email: "",
        phoneNumber: "",
        fullName: "",
        identityNumber: "",
        gender: "Female",
        address: "",
        dateOfBirth: "",
        password: ""
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const [roles, setRoles] = useState([]);
    useEffect(() => {
        async function fetchRoles() {
            try {
                const data = await getRoleList();
                setRoles(data);
            } catch (error) {
                console.error("Failed to fetch roles", error);
            }
        }
        fetchRoles();
    }, []);

    const handleRoleChange = (roleCode) => {
        setFormData(prev => {
            const currentRoles = prev.roleCodes;
            if (currentRoles.includes(roleCode)) {
                return { ...prev, roleCodes: currentRoles.filter(r => r !== roleCode) };
            } else {
                return { ...prev, roleCodes: [...currentRoles, roleCode] };
            }
        });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        addUser({
            address: formData.address,
            dateOfBirth: formData.dateOfBirth,
            email: formData.email,
            gender: formData.gender,
            fullName: formData.fullName,
            identityNumber: formData.identityNumber,
            password: formData.password,
            phoneNumber: formData.phoneNumber,
            roleCodes: formData.roleCodes
        });
        onClose();
    };

    if (!visible) return null;

    return (
        <div id="create-user-form">
            <div className="modal-overlay">
                <div className="modal-content">
                    <h2>Create user account</h2>
                    <form onSubmit={handleSubmit}>
                        <div className="left-form">
                            <div className="form-group">
                                <label>Email</label>
                                <input type="email" name="email" value={formData.email} onChange={handleChange} required />
                            </div>

                            <div className="form-group">
                                <label>Password</label>
                                <input type="password" name="password" value={formData.password} onChange={handleChange} required />
                            </div>

                            <div className="form-group">
                                <label>Phone number</label>
                                <input type="text" name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} />
                            </div>

                            <div className="form-group">
                                <label>Identity number</label>
                                <input type="text" name="identityNumber" value={formData.identityNumber} onChange={handleChange} />
                            </div>
                        </div>

                        <div className="middle-form">
                            <div className="form-group">
                                <label>Full name</label>
                                <input type="text" name="fullName" value={formData.fullName} onChange={handleChange} required />
                            </div>

                            <div className="form-group">
                                <label>Gender</label>
                                <select
                                    name="gender"
                                    value={formData.gender}
                                    onChange={handleChange}
                                    className="form-control"
                                >
                                    <option value="Female">Female</option>
                                    <option value="Male">Male</option>
                                </select>
                            </div>

                            <div className="form-group">
                                <label>Address</label>
                                <input type="text" name="address" value={formData.address} onChange={handleChange} />
                            </div>

                            <div className="form-group">
                                <label>Date of birth</label>
                                <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} />
                            </div>
                        </div>

                        {roles ? (
                            <div className="right-form">
                                <div className="role-checkboxes">
                                    <label>Role permissions</label>
                                    <div className="role-toggle-wrapper">
                                        {roles.map(role => {
                                            const isActive = formData.roleCodes.includes(role.roleCode);
                                            return (
                                                <div
                                                    key={role.roleID}
                                                    className={`role-toggle ${isActive ? "active" : "inactive"}`}
                                                    onClick={() => handleRoleChange(role.roleCode)}
                                                >
                                                    {role.name}  {/* Display name */}
                                                </div>
                                            );
                                        })}
                                    </div>
                                </div>

                                <div className="modal-actions">
                                    <button type="submit" className="btn create">Create</button>
                                    <button type="button" className="btn cancel" onClick={onClose}>Cancel</button>
                                </div>
                            </div>
                        ) : (
                            <div>
                                The role list is empty
                            </div>
                        )}
                    </form>
                </div>
            </div>
        </div>
    );
}

export default CreateUserForm;
