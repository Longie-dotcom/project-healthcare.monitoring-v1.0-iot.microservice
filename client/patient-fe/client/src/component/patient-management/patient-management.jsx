import React, { useState, useEffect } from "react";
import { usePatients } from "../../api/usePatients";
import { usePatientStatuses } from "../../api/usePatientStatuses";
import Loading from "../loading/loading";
import InfoBox from "../info-box/info-box";
import ConfirmBox from "../confirm-box/confirm-box";
import CreatePatientForm from "./create-patient-form";
import UpdatePatientForm from "./update-patient-form";
import PatientDetail from "./patient-detail";
import "./patient-management.css";

function PatientManagement() {
  const [patients, setPatients] = useState([]);
  const [statuses, setStatuses] = useState([]);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [reload, setReload] = useState(0);
  const [reloadDetail, setReloadDetail] = useState(0);

  const [openCreateForm, setOpenCreateForm] = useState(false);
  const [updatedPatient, setUpdatedPatient] = useState(null);
  const [detailedPatient, setDetailedPatient] = useState(null);
  const [deletedPatient, setDeletedPatient] = useState(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);

  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 5;
  const [isLastPage, setIsLastPage] = useState(false);

  const [search, setSearch] = useState("");

  const { 
    getSortedPatientList, addPatient, updatePatient, deletePatient, getPatientById, 
    assignBed, assignStaff, releaseBed, unassignStaff
  } =
    usePatients({ setLoading, setError, setInfo, setReload });

  const { getAllPatientStatuses } =
    usePatientStatuses({ setError, setInfo, setLoading, setReload });

  useEffect(() => {
    if (!getAllPatientStatuses) return;

    const fetchStatuses = async () => {
      const data = await getAllPatientStatuses();
      if (data)
        setStatuses(data);
    };

    fetchStatuses();
  }, []);

  useEffect(() => {
    const fetchPatients = async () => {
      const data = await getSortedPatientList({
        search,
        pageIndex: currentPage,
        pageLength: pageSize,
        sortBy: ""
      });
      if (data) setIsLastPage(data.length < pageSize);
      setPatients(data);
    };
    fetchPatients();
  }, [reload, currentPage, search]);

  useEffect(() => {
    const fetchPatients = async () => {
      if (error) return;
      if (!detailedPatient) return;
      const data = await getPatientById({ id: detailedPatient.patientID });
      setDetailedPatient(data);
    };
    fetchPatients();
  }, [reloadDetail]);


  const handleDelete = (patient) => {
    setDeletedPatient(patient);
    setShowConfirmDelete(true);
  };

  const handleUpdate = async (patient) => {
    const data = await getPatientById({ id: patient.patientID });
    setUpdatedPatient(data);
  };

  const handleDetail = async (patient) => {
    const data = await getPatientById({ id: patient.patientID });
    setDetailedPatient(data);
  };

  return (
    <div id="patient-management">
      {loading && <Loading />}
      {error && <InfoBox title="Error" message={error} onClose={() => setError(null)} />}
      {info && <InfoBox title="Info" message={info} onClose={() => setInfo(null)} />}
      {showConfirmDelete && deletedPatient && (
        <ConfirmBox
          title="Confirm Delete"
          message={`Are you sure you want to delete ${deletedPatient.patientCode}?`}
          onConfirm={async () => {
            const result = await deletePatient({ id: deletedPatient.patientID });
            if (result) setReload((prev) => prev + 1);
            setShowConfirmDelete(false);
          }}
          onCancel={() => setShowConfirmDelete(false)}
        />
      )}

      {/* Header */}
      <div className="header">
        <div>Patient Management</div>
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

      {/* Patient list */}
      {patients?.length > 0 ? (
        <div className="main">
          <div className="page">
            <div className="patient-list">
              <div className={`patient-list-header ${detailedPatient ? "less" : ""}`}>
                <div>Patient Name</div>
                {!detailedPatient && (<div>Identity Number</div>)}
                {!detailedPatient && (<div>Admission Date</div>)}
                {!detailedPatient && (<div>Status</div>)}
                <div>Actions</div>
              </div>

              {patients.map((p, idx) => (
                <div
                  key={idx}
                  className={`patient-card ${p.isActive ? "" : "gray-out"} ${detailedPatient ? "less" : ""}`}
                >
                  <div>{p.fullName}</div>
                  {!detailedPatient && (<div>{p.identityNumber}</div>)}
                  {!detailedPatient && (<div>{p.admissionDate?.split("T")[0]}</div>)}
                  {!detailedPatient && (<div>{p.status}</div>)}
                  <div className="patient-actions">
                    <button className="btn delete" onClick={() => handleDelete(p)}>Delete</button>
                    <button className="btn update" onClick={() => handleUpdate(p)}>Update</button>
                    <button className="btn detail" onClick={() => handleDetail(p)}>Detail</button>
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
          </div>

          <PatientDetail
            visible={!!detailedPatient}
            onClose={() => setDetailedPatient(null)}
            patient={detailedPatient}
            assignBed={assignBed}
            assignStaff={assignStaff}
            releaseBed={releaseBed}
            unassignStaff={unassignStaff}
            setReloadDetail={setReloadDetail}
          />
        </div>
      ) : (
        <div className="empty-list">No patients found</div>
      )}

      {/* Forms & Detail */}
      <CreatePatientForm
        visible={openCreateForm}
        onClose={() => setOpenCreateForm(false)}
        addPatient={addPatient}
      />
      <UpdatePatientForm
        visible={!!updatedPatient}
        onClose={() => setUpdatedPatient(null)}
        patientData={updatedPatient}
        updatePatient={updatePatient}
        getPatientById={getPatientById}
        statuses={statuses}
      />
    </div>
  );
}

export default PatientManagement;
