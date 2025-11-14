import { jwtDecode } from "jwt-decode";
import api from "../helper/refreshTokenInstance";

export function usePrivileges({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;
  
  // Get all privileges
  const getPrivilegeList = async () => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/iam/privilege`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(err.response?.data?.message || "Unable to load privilege list");
      
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Get privilege by ID
  const getPrivilegeById = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/iam/privilege/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(err.response?.data?.message || "Unable to load privilege detail");

      return null;
    } finally {
      setLoading(false);
    }
  };

  // Create privilege
  const addPrivilege = async ({ name, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/iam/privilege`,
        {
          performedBy,
          name,
          description,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setInfo(response?.data?.message || "Privilege created successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(err.response?.data?.message || "Unable to create privilege");

      return null;
    } finally {
      setLoading(false);
    }
  };

  // Update privilege
  const updatePrivilege = async ({ id, name, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.put(
        `${gatewayUrl}/hcm/iam/privilege/${id}`,
        {
          performedBy,
          name,
          description,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setInfo(response?.data?.message || "Privilege updated successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(err.response?.data?.message || "Unable to update privilege");

      return null;
    } finally {
      setLoading(false);
    }
  };

  // Delete privilege
  const deletePrivilege = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/iam/privilege/${id}`,
        {
          data: { performedBy },
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setInfo(response?.data?.message || "Privilege deleted successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401)
        setError("User has no permission to access this function");
      else
        setError(err.response?.data?.message || "Unable to delete privilege");

      return null;
    } finally {
      setLoading(false);
    }
  };

  return {
    getPrivilegeList,
    getPrivilegeById,
    addPrivilege,
    updatePrivilege,
    deletePrivilege,
  };
}
