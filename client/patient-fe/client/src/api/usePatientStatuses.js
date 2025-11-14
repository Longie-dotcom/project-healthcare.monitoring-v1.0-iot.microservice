import { jwtDecode } from "jwt-decode";
import api from "../helper/refreshTokenInstance";

export function usePatientStatuses({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  // Get all patient statuses
  const getAllPatientStatuses = async () => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/patient/patient-status`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(
          err.response?.data?.message || "Unable to load patient status list"
        );
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Get patient status by code
  const getPatientStatusByCode = async ({ code }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(
        `${gatewayUrl}/hcm/patient/patient-status/${code}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(
          err.response?.data?.message || "Unable to load patient status detail"
        );
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Create new patient status
  const addPatientStatus = async ({ patientStatusCode, name, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/patient/patient-status`,
        { performedBy, patientStatusCode, name, description },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Patient status created successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(
          err.response?.data?.message || "Unable to create patient status"
        );
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Update patient status
  const updatePatientStatus = async ({ code, name, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.put(
        `${gatewayUrl}/hcm/patient/patient-status/${code}`,
        { performedBy, name, description },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Patient status updated successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(
          err.response?.data?.message || "Unable to update patient status"
        );
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Delete patient status
  const deletePatientStatus = async ({ code }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/patient/patient-status/${code}`,
        {
          data: { performedBy },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setInfo(response?.data?.message || "Patient status deleted successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(
          err.response?.data?.message || "Unable to delete patient status"
        );
      return null;
    } finally {
      setLoading(false);
    }
  };

  return {
    getAllPatientStatuses,
    getPatientStatusByCode,
    addPatientStatus,
    updatePatientStatus,
    deletePatientStatus,
  };
}
