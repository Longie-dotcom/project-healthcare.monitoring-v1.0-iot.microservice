import api from "../helper/refreshTokenInstance";
import { jwtDecode } from "jwt-decode";

export function useRoomProfile({ setLoading, setError }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const getTokenAndUser = () => {
    const token = localStorage.getItem("accessToken");
    const decoded = jwtDecode(token);
    const performedBy = decoded.FullName || "Unknown User";
    const identityNumber = decoded.IdentityNumber;
    return { token, performedBy, identityNumber };
  };

  // ------------------------
  // Get controllers handled by a staff
  // ------------------------
  const getPatientsOfStaff = async () => {
    setLoading(true);
    setError(null);

    try {
      const { token } = getTokenAndUser();

      const response = await api.get(
        `${gatewayUrl}/hcm/data-collection/room-profile/my-patients`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      return response.data.payload; // backend already returns array
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Unable to load patient data");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // ------------------------
  // Get detail of a patient handled by a staff
  // ------------------------
  const getPatientDetailOfStaff = async ({ patientIdentityNumber }) => {
    setLoading(true);
    setError(null);

    try {
      const { token } = getTokenAndUser();

      const response = await api.get(
        `${gatewayUrl}/hcm/data-collection/room-profile/patient/${patientIdentityNumber}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      return response.data.payload; // backend already returns array
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Unable to load patient data");
      return null;
    } finally {
      setLoading(false);
    }
  };

  return {
    getPatientsOfStaff, getPatientDetailOfStaff
  };
}
