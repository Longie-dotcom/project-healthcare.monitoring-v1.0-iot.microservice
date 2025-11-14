import { jwtDecode } from "jwt-decode";
import api from "../helper/refreshTokenInstance";

export function usePatients({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  // Get all patients
  const getSortedPatientList = async ({
    search = "",
    pageIndex = 1,
    pageLength = 10,
    sortBy = "",
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/patient/patient`, {
        params: {
          Search: search || undefined,
          PageIndex: pageIndex || 1,
          PageLength: pageLength || 10,
          sortBy: sortBy || "SORT_BY_ID",
        },
        headers: { Authorization: `Bearer ${token}` },
      });
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(err.response?.data?.message || "Unable to load patient list");
      return null;
    } finally {
      setLoading(false);
    }
  };
  
  // Create patient
  const addPatient = async ({
    identityNumber,
    admissionDate,
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/patient/patient`,
        {
          performedBy,
          identityNumber,
          admissionDate,
        },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Patient created successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to create patient");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Update patient
  const updatePatient = async ({ 
    id, 
    status, 
    admissionDate, 
    dischargeDate,
    fullName,
    dateOfBirth,
    address,
    gender
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.put(
        `${gatewayUrl}/hcm/patient/patient/${id}`,
        {
          performedBy,
          patientStatusCode: status,
          admissionDate,
          dischargeDate,
          fullName,
          dateOfBirth,
          address,
          gender,
        },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Patient updated successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to update patient");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Delete patient
  const deletePatient = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/patient/patient/${id}`,
        {
          data: { performedBy },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setInfo(response?.data?.message || "Patient deleted successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to delete patient");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Get patient by ID
  const getPatientById = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/patient/patient/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to load patient detail");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Assign bed
  const assignBed = async ({ patientId, controllerKey, assignedAt }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/patient/patient/${patientId}/assign-bed`,
        { performedBy, controllerKey, assignedAt },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Bed assigned successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to assign bed");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Release bed
  const releaseBed = async ({ patientId, patientBedAssignmentID, releasedAt }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/patient/patient/${patientId}/release-bed`,
        { performedBy, patientBedAssignmentID, releasedAt },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Bed released successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to release bed");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Assign staff
  const assignStaff = async ({ patientId, staffIdentityNumber, assignedAt }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/patient/patient/${patientId}/assign-staff`,
        { performedBy, staffIdentityNumber, assignedAt },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Staff assigned successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to assign staff");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Unassign staff
  const unassignStaff = async ({
    patientId,
    patientStaffAssignmentID,
    unassignedAt,
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/patient/patient/${patientId}/unassign-staff`,
        { performedBy, patientStaffAssignmentID, unassignedAt },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Staff unassigned successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to unassign staff");
      return null;
    } finally {
      setLoading(false);
    }
  };

  return {
    getSortedPatientList,
    addPatient,
    updatePatient,
    deletePatient,
    getPatientById,
    assignBed,
    releaseBed,
    assignStaff,
    unassignStaff,
  };
}
