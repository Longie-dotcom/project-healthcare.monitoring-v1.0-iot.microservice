import { jwtDecode } from "jwt-decode";
import api from "../helper/refreshTokenInstance";

export function useStaffs({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  // All Staffs with search + paging
  const getSortedStaffList = async ({
    search = "",
    pageIndex = 1,
    pageLength = 10,
    sortBy = "" }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/staff/staff`, {
        params: {
          Search: search || undefined,
          PageIndex: pageIndex || 1,
          PageLength: pageLength || 5,
          sortBy: sortBy || "SORT_BY_ID", // or any default backend expects
        },
        headers: { Authorization: `Bearer ${token}` },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to load staff list");
      return null;
    } finally {
      setLoading(false);
    }
  };


  // Add Staff (basic only, no list entities)
  const addStaff = async ({
    identityNumber,
    professionalTitle,
    specialization,
    avatarUrl,
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/staff/staff`,
        {
          performedBy,
          identityNumber,
          professionalTitle,
          specialization,
          avatarUrl,
        },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Staff created successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to create staff");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Update Staff (includes list entities)
  const updateStaff = async ({
    id,
    professionalTitle,
    specialization,
    avatarUrl,
    isActive,

    gender,
    address,
    fullName,
    dateOfBirth,

    staffLicenses = [],
    staffSchedules = [],
    staffAssignments = [],
    staffExperiences = [],
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.put(
        `${gatewayUrl}/hcm/staff/staff/${id}`,
        {
          performedBy,
          professionalTitle,
          specialization,
          avatarUrl,
          isActive,
          
          gender,
          address,
          fullName,
          dateOfBirth,

          staffLicenses,
          staffSchedules,
          staffAssignments,
          staffExperiences,
        },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Staff updated successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to update staff");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Remove Staff
  const deleteStaff = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decoded = jwtDecode(token);
      const performedBy = decoded.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/staff/staff/${id}`,
        {
          data: { performedBy },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setInfo(response?.data?.message || "Staff deleted successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to delete staff");
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Staff by ID
  const getStaffById = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/staff/staff/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else setError(err.response?.data?.message || "Unable to load staff detail");
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { getSortedStaffList, addStaff, updateStaff, deleteStaff, getStaffById };
}
