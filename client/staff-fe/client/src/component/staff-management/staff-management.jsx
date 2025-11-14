import React, { useState, useEffect } from "react";
import { useStaffs } from "../../api/useStaffs";
import Loading from "../loading/loading";
import InfoBox from "../info-box/info-box";
import ConfirmBox from "../confirm-box/confirm-box";
import CreateStaffForm from "./create-staff-form";
import UpdateStaffForm from "./update-staff-form";
import StaffDetail from "./staff-detail";
import "./staff-management.css";

function StaffManagement() {
  const [staffs, setStaffs] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [reload, setReload] = useState(0);

  const [openCreateForm, setOpenCreateForm] = useState(false);
  const [updatedStaff, setUpdatedStaff] = useState(null);
  const [detailedStaff, setDetailedStaff] = useState(null);
  const [deletedStaff, setDeletedStaff] = useState(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);

  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 5;
  const [isLastPage, setIsLastPage] = useState(false);

  const [search, setSearch] = useState("");

  const { getSortedStaffList, addStaff, updateStaff, deleteStaff, getStaffById } =
    useStaffs({ setLoading, setError, setInfo, setReload });

  useEffect(() => {
    const fetchStaffs = async () => {
      const data = await getSortedStaffList({
        search,
        pageIndex: currentPage,
        pageLength: pageSize,
        sortBy: ""
      });
      if (data) {
        setIsLastPage(data.length < pageSize);
      }
      setStaffs(data);
    };
    fetchStaffs();
  }, [reload, currentPage, search]);

  const handleDelete = (staff) => {
    setDeletedStaff(staff);
    setShowConfirmDelete(true);
  };

  const handleUpdate = (staff) => {
    setUpdatedStaff(staff);
  };

  const handleDetail = async (staff) => {
    const data = await getStaffById({ id: staff.staffID });
    setDetailedStaff(data);
  };

  return (
    <div id="staff-management">
      {loading && <Loading />}
      {error && <InfoBox title="Error" message={error} onClose={() => setError(null)} />}
      {info && <InfoBox title="Info" message={info} onClose={() => setInfo(null)} />}
      {showConfirmDelete && (
        <ConfirmBox
          title="Confirm Delete"
          message={`Are you sure you want to delete ${deletedStaff.staffCode}?`}
          onConfirm={async () => {
            const result = await deleteStaff({ id: deletedStaff.staffID });
            if (result) {
              setReload(prev => prev + 1);
            }
            setShowConfirmDelete(false);
          }}
          onCancel={() => setShowConfirmDelete(false)}
        />
      )}

      {/* Header */}
      <div className="header">
        <div>Staff Management</div>
        <div className="filter">
          <input
            type="text"
            placeholder="Search by name or code..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button onClick={() => setCurrentPage(1)}>Search</button>
        </div>
        <button onClick={() => setOpenCreateForm(true)}>Create</button>
      </div>

      {/* Filter tags */}
      <div className="filtering-list">
        {search && (
          <div className="filter-tag" onClick={() => setSearch("")}>
            Search: {search} ×
          </div>
        )}
      </div>

      {/* Staff list */}
      {staffs?.length > 0 ? (
        <>
          <div className="staff-list">
            <div className="staff-list-header">
              <div>Staff Name</div>
              <div>Identity Number</div>
              <div>Professional Title</div>
              <div>Specialization</div>
              <div>Actions</div>
            </div>

            {staffs.map((staff, idx) => (
              <div key={idx} className="staff-card">
                <div>{staff.fullName}</div>
                <div>{staff.identityNumber}</div>
                <div>{staff.professionalTitle}</div>
                <div>{staff.specialization}</div>
                <div className="staff-actions">
                  <button className="btn delete" onClick={() => handleDelete(staff)}>Delete</button>
                  <button className="btn update" onClick={() => handleUpdate(staff)}>Update</button>
                  <button className="btn detail" onClick={() => handleDetail(staff)}>Detail</button>
                </div>
              </div>
            ))}
          </div>

          {/* Pagination */}
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
        <div className="empty-list">No staff found</div>
      )}

      {/* Forms & Detail */}
      <CreateStaffForm
        visible={openCreateForm}
        onClose={() => setOpenCreateForm(false)}
        addStaff={addStaff}
      />
      <UpdateStaffForm
        visible={updatedStaff}
        onClose={() => setUpdatedStaff(null)}
        staffData={updatedStaff}
        updateStaff={updateStaff}
        getStaffById={getStaffById}
      />
      <StaffDetail
        visible={detailedStaff}
        onClose={() => setDetailedStaff(null)}
        staff={detailedStaff}
      />
    </div>
  );
}

export default StaffManagement;
