import React, { useState, useEffect } from "react";
import InfoBox from "../info-box/info-box";
import Loading from "../loading/loading";
import ConfirmBox from "../confirm-box/confirm-box";
import UpdateUserForm from "./update-user-form";
import UserDetail from "./user-detail";
import "./user-management.css";

import CreateUserForm from "./create-user-form";
import { getRoleList } from "../../api/useRoles";
import { usePrivileges } from "../../api/usePrivileges";
import { useUsers } from "../../api/useUsers";

function UserManagement() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [info, setInfo] = useState(null);
    const [openForm, setOpenForm] = useState(false);
    const [reload, setReload] = useState(0);
    const [showConfirmDelete, setShowConfirmDelete] = useState(false);

    const [queryType, setQueryType] = useState("gender");
    const [filters, setFilters] = useState({});
    const [isActive, setIsActive] = useState(null);
    const [sortBy, setSortBy] = useState("SORT_BY_IDENTITY")
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(5);
    const [isLastPage, setIsLastPage] = useState(false);

    const [filterInputs, setFilterInputs] = useState({
        gender: "Female",
        fullName: "",
        dateOfBirthFrom: "",
        dateOfBirthTo: "",
    });

    const [deletedUser, setDeletedUser] = useState(null);
    const [updatedUser, setUpdatedUser] = useState(null);
    const [detailedUser, setDetailedUser] = useState(null);
    const { getSortedUserList, addUser, deleteUser, updateUser, getUserById } =
        useUsers({ setLoading, setError, setInfo, setReload });
    const { getPrivilegeList } =
        usePrivileges({ setError, setInfo, setLoading, setReload });

    useEffect(() => {
        const fetchUsers = async () => {
            const params = {
                sortBy: sortBy,
                pageIndex: currentPage,
                pageLength: pageSize,
                search: filters.fullName || "",
                gender: filters.gender || "",
                isActive: isActive,
                dateOfBirthFrom: filters.dateOfBirthRange?.from || null,
                dateOfBirthTo: filters.dateOfBirthRange?.to || null,
            };

            const data = await getSortedUserList(params);
            setUsers(data);

            if (data) {
                setUsers(data);
                setIsLastPage(data.length < pageSize); // mark last page if returned less
            }
        };

        fetchUsers();
    }, [reload, filters, currentPage, sortBy, isActive]);

    const handleDelete = (user) => {
        setDeletedUser(user);
        setShowConfirmDelete(true);
    };
    const handleUpdate = (user) => {
        setUpdatedUser(user);
    };
    const handleDetail = async (user) => {
        const userDetail = await getUserById({ id: user.userID });
        setDetailedUser(userDetail);
    };

    const applyFilter = () => {
        if (queryType === "DateOfBirth") {
            const from = filterInputs.dateOfBirthFrom;
            const to = filterInputs.dateOfBirthTo;

            // only apply if at least one is set
            if (!from && !to) return;

            setFilters((prev) => ({
                ...prev,
                dateOfBirthRange: { from, to }, // override if already exists
            }));
            return;
        }

        // normal filters (single key-value)
        let type, value;
        if (queryType === "Gender") {
            type = "gender";
            value = filterInputs.gender;
        } else if (queryType === "Name") {
            type = "fullName";
            value = filterInputs.fullName.trim();
        }

        if (!value) return;

        setCurrentPage(1);
        setFilters((prev) => ({
            ...prev,
            [type]: value, // override if already exists
        }));
    };

    // remove filter
    const removeFilter = (type) => {
        setCurrentPage(1);
        setFilters((prev) => {
            const updated = { ...prev };
            delete updated[type];
            return updated;
        });
    };

    const handleInputChange = (field, value) => {
        setFilterInputs((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    return (
        <div id="user-management">
            {loading && <Loading />}
            {error && <InfoBox title="Error" message={error} onClose={() => setError(null)} />}
            {info && <InfoBox title="Information" message={info} onClose={() => setInfo(null)} />}
            {showConfirmDelete && (
                <ConfirmBox
                    title="Deleting confirmation"
                    message="Are you sure you want to delete this user?"
                    onConfirm={() => {
                        deleteUser({ id: deletedUser.userID });
                        setShowConfirmDelete(false);
                    }}
                    onCancel={() => setShowConfirmDelete(false)}
                />
            )}

            <div className="header">
                <div>User account management</div>

                <div className="filter">
                    <select value={queryType} onChange={(e) => setQueryType(e.target.value)}>
                        <option value="Gender">Gender</option>
                        <option value="Name">Full name</option>
                        <option value="DateOfBirth">Date of Birth</option>
                    </select>

                    {queryType === "Gender" && (
                        <select
                            value={filterInputs.gender}
                            onChange={(e) => handleInputChange("gender", e.target.value)}
                        >
                            <option value="Female">Female</option>
                            <option value="Male">Male</option>
                        </select>
                    )}

                    {queryType === "Name" && (
                        <input
                            type="text"
                            value={filterInputs.fullName}
                            onChange={(e) => handleInputChange("fullName", e.target.value)}
                        />
                    )}

                    {queryType === "DateOfBirth" ? (
                        <div className="date-range-inputs">
                            <input
                                type="date"
                                value={filterInputs.dateOfBirthFrom}
                                onChange={(e) => handleInputChange("dateOfBirthFrom", e.target.value)}
                                placeholder="From"
                            />
                            <input
                                type="date"
                                value={filterInputs.dateOfBirthTo}
                                onChange={(e) => handleInputChange("dateOfBirthTo", e.target.value)}
                                placeholder="To"
                            />
                        </div>
                    ) : null}

                    <button onClick={applyFilter}>Apply</button>
                    
                    <button onClick={() => setIsActive(prev => prev === true ? false : prev === false ? null : true)}>
                        {isActive === true ? "Show active user" : isActive === false ? "Show inactive user" : "Show any"}
                    </button>
                </div>

                <button onClick={() => setOpenForm(true)}>
                    Create
                </button>
            </div>

            <div className="filtering-list">
                {Object.keys(filters).length === 0 ? (
                    <span className="no-filter">No active filters</span>
                ) : (
                    <>
                        <div className="title">
                            Filtering types:
                        </div>
                        {Object.entries(filters).map(([type, value]) => (
                            <div key={type} className="filter-tag">
                                <span onClick={() => removeFilter(type)}>
                                    {type === "dateOfBirthRange"
                                        ? `Date of Birth: ${value.from || "any"} to ${value.to || "any"}`
                                        : `${type}: ${value}`}
                                </span>
                            </div>
                        ))}
                    </>
                )}
            </div>


            {users ? (
                <>
                    <div className="user-list">
                        <div className="user-list-header">
                            <div>Full name</div>
                            <div onClick={() => setSortBy("SORT_BY_EMAIL")}>Email</div>
                            <div onClick={() => setSortBy("SORT_BY_PHONE")}>Phone number</div>
                            <div onClick={() => setSortBy("SORT_BY_GENDER")}>Gender</div>
                            <div>Action</div>
                        </div>

                        {users.map((user, index) => (
                            <div key={index} className={`user-card ${user.isActive ? "user-active" : "user-inactive"}`}>
                                <div>{user.fullName}</div>
                                <div>{user.email}</div>
                                <div>{user.phoneNumber}</div>
                                <div>{user.gender}</div>
                                <div className="user-actions">
                                    <button className="btn delete" onClick={() => handleDelete(user)}>Delete</button>
                                    <button className="btn update" onClick={() => handleUpdate(user)}>Update</button>
                                    <button className="btn detail" onClick={() => handleDetail(user)}>Detail</button>
                                </div>
                            </div>
                        ))}
                    </div>
                    <div className="paging">
                        <button
                            className="page-btn"
                            disabled={currentPage === 1}
                            onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                        >
                            ‹ Prev
                        </button>

                        <span className="page-info">Page {currentPage}</span>

                        <button
                            className="page-btn"
                            disabled={isLastPage}
                            onClick={() => setCurrentPage((p) => p + 1)}
                        >
                            Next ›
                        </button>
                    </div>
                </>
            ) : (
                <div className="empty-list">
                    The user list is empty.
                </div>
            )}

            <CreateUserForm
                visible={openForm}
                onClose={() => setOpenForm(false)}
                addUser={addUser}
            />

            <UpdateUserForm
                visible={updatedUser}
                onClose={() => setUpdatedUser(null)}
                userId={updatedUser?.userID}
                updateUser={updateUser}
                getPrivilegeList={getPrivilegeList}
                getUserById={getUserById}
                getRoleList={getRoleList}
            />

            <UserDetail
                onClose={() => setDetailedUser(null)}
                user={detailedUser}
                visible={detailedUser}
            />
        </div>
    );
}

export default UserManagement;
