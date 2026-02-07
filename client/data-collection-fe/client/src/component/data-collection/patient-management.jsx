// PatientManagement.jsx
import React, { useState, useEffect } from "react";
import { useRoomProfile } from "../../api/useRoomProfile";
import Loading from "../loading/loading";
import InfoBox from "../info-box/info-box";
import PatientDetail from "./patient-detail";
import "./patient-management.css";

export default function PatientManagement() {
  const [patients, setPatients] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [detailedPatient, setDetailedPatient] = useState(null);

  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 5;
  const [isLastPage, setIsLastPage] = useState(false);
  const [search, setSearch] = useState("");

  const { getPatientsOfStaff, getPatientDetailOfStaff } = useRoomProfile({
    setLoading,
    setError,
  });

  // Fetch patients
  useEffect(() => {
    const fetchPatients = async () => {
      setLoading(true);
      try {
        const data = await getPatientsOfStaff();
        if (data) {
          // Search filter
          const filtered = search
            ? data.filter(
                (p) =>
                  p.patientName?.toLowerCase().includes(search.toLowerCase()) ||
                  p.patientIdentityNumber
                    ?.toLowerCase()
                    .includes(search.toLowerCase())
              )
            : data;

          setIsLastPage(filtered.length <= pageSize * currentPage);
          setPatients(
            filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize)
          );
        }
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchPatients();
  }, [currentPage, search]);

  // View patient detail
  const handleDetail = async (patient) => {
    setLoading(true);
    try {
      const data = await getPatientDetailOfStaff({
        patientIdentityNumber: patient.patientIdentityNumber,
      });
      setDetailedPatient(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div id="patient-management">
      {loading && <Loading />}
      {error && <InfoBox title="Error" message={error} onClose={() => setError(null)} />}
      {info && <InfoBox title="Info" message={info} onClose={() => setInfo(null)} />}

      {/* Header */}
      <div className="header">
        <div>Patient List</div>
        <div className="filter">
          <input
            type="text"
            placeholder="Search by name or ID..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button onClick={() => setCurrentPage(1)}>Search</button>
        </div>
      </div>

      {/* Patient list */}
      {patients?.length > 0 ? (
        <div className="main">
          <div className="page">
            <div className="patient-list">
              <div className="patient-list-header">
                <div>Name</div>
                <div>ID</div>
                <div>Bed</div>
                <div>Room</div>
                <div>Actions</div>
              </div>

              {patients.map((p) => (
                <div key={p.patientIdentityNumber} className="patient-card">
                  <div>{p.patientName}</div>
                  <div>{p.patientIdentityNumber}</div>
                  <div>{p.bedNumber}</div>
                  <div>{p.roomName || "-"}</div>
                  <div className="patient-actions">
                    <button className="btn detail" onClick={() => handleDetail(p)}>
                      Detail
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
          </div>

          {/* Patient Detail */}
          {detailedPatient && (
            <PatientDetail
              patient={detailedPatient}
              onClose={() => setDetailedPatient(null)}
            />
          )}
        </div>
      ) : (
        <div className="empty-list">No patients found</div>
      )}
    </div>
  );
}
