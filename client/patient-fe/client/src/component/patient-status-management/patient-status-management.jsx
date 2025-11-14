import React, { useEffect, useState } from "react";
import "./patient-status-management.css";
import { usePatientStatuses } from "../../api/usePatientStatuses";
import Loading from "../loading/loading";
import InfoBox from "../info-box/info-box";
import ConfirmBox from "../confirm-box/confirm-box";
import CreatePatientStatusForm from "./create-patient-status-form";
import UpdatePatientStatusForm from "./update-patient-status-form";

function PatientStatusManagement() {
  const [statuses, setStatuses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [reload, setReload] = useState(0);

  const [openCreateForm, setOpenCreateForm] = useState(false);
  const [updatedStatus, setUpdatedStatus] = useState(null);
  const [deletedStatus, setDeletedStatus] = useState(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);

  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 6;
  const [isLastPage, setIsLastPage] = useState(false);
  const [search, setSearch] = useState("");

  const {
    getAllPatientStatuses,
    getPatientStatusByCode,
    addPatientStatus,
    updatePatientStatus,
    deletePatientStatus,
  } = usePatientStatuses({ setLoading, setError, setInfo, setReload });

  // Fetch list
  useEffect(() => {
    const fetchStatuses = async () => {
      const data = await getAllPatientStatuses();
      if (data) {
        setStatuses(data);
        setIsLastPage(data.length < pageSize);
      }
    };
    fetchStatuses();
  }, [reload, currentPage, search]);

  const handleDelete = (status) => {
    setDeletedStatus(status);
    setShowConfirmDelete(true);
  };

  const handleUpdate = async (status) => {
    setUpdatedStatus(status);
  };

  return (
    <div id="patient-status-management">
      {loading && <Loading />}
      {error && (
        <InfoBox title="Error" message={error} onClose={() => setError(null)} />
      )}
      {info && (
        <InfoBox title="Info" message={info} onClose={() => setInfo(null)} />
      )}
      {showConfirmDelete && deletedStatus && (
        <ConfirmBox
          title="Confirm Delete"
          message={`Are you sure you want to delete status "${deletedStatus.patientStatusCode}"?`}
          onConfirm={async () => {
            const result = await deletePatientStatus({
              code: deletedStatus.patientStatusCode,
            });
            if (result) setReload((prev) => prev + 1);
            setShowConfirmDelete(false);
          }}
          onCancel={() => setShowConfirmDelete(false)}
        />
      )}

      {/* Header */}
      <div className="header">
        <div>Patient Status Management</div>
        <div className="filter">
          <input
            type="text"
            placeholder="Search by code or description..."
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

      {/* List */}
      {statuses?.length > 0 ? (
        <>
          <div className="status-list">
            <div className="status-list-header">
              <div>Code</div>
              <div>Name</div>
              <div>Description</div>
              <div>Actions</div>
            </div>

            {statuses.map((s, idx) => (
              <div key={idx} className="status-card">
                <div>{s.patientStatusCode}</div>
                <div>{s.name}</div>
                <div>{s.description}</div>
                <div className="status-actions">
                  <button className="btn delete" onClick={() => handleDelete(s)}>
                    Delete
                  </button>
                  <button className="btn update" onClick={() => handleUpdate(s)}>
                    Update
                  </button>
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
        <div className="empty-list">No patient statuses found</div>
      )}

      {/* Modals */}
      <CreatePatientStatusForm
        visible={openCreateForm}
        onClose={() => setOpenCreateForm(false)}
        addPatientStatus={addPatientStatus}
      />
      <UpdatePatientStatusForm
        visible={!!updatedStatus}
        onClose={() => setUpdatedStatus(null)}
        patientStatus={updatedStatus}
        updatePatientStatus={updatePatientStatus}
        getPatientStatusByCode={getPatientStatusByCode}
      />
    </div>
  );
}

export default PatientStatusManagement;
