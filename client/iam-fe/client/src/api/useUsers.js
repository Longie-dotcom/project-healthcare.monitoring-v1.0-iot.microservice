import { jwtDecode } from "jwt-decode";
import api from "../helper/refreshTokenInstance";

export function useUsers({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const getSortedUserList = async ({
    sortBy = "SORT_BY_IDENTITY",
    pageIndex = 1,
    pageLength = 10,
    search = "",
    gender = "",
    isActive = null,
    dateOfBirthFrom = null,
    dateOfBirthTo = null,
  }) => {

    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/iam/user`, {
        params: {
          sortBy: sortBy,
          PageIndex: pageIndex,
          PageLength: pageLength,
          Search: search || undefined,
          Gender: gender || undefined,
          IsActive: isActive,
          DateOfBirthFrom: dateOfBirthFrom || undefined,
          DateOfBirthTo: dateOfBirthTo || undefined,
        },
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to load user list");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  const addUser = async ({
    roleCodes,
    email,
    phoneNumber,
    fullName,
    identityNumber,
    gender,
    address,
    dateOfBirth,
    password
  }) => {

    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/iam/user`,
        {
          performedBy,
          roleCodes,
          email,
          phoneNumber,
          fullName,
          identityNumber,
          gender,
          address,
          dateOfBirth,
          password
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setInfo(response?.data?.message || "");
      setReload(prev => prev + 1);
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to create user");
      } return null;
    } finally {
      setLoading(false);
    }
  };

  const updateUser = async ({
    id,
    fullName,
    dateOfBirth,
    gender,
    address,
    email,
    phoneNumber,
    userRoleUpdateDTOs = [],
    userPrivilegeUpdateDTOs = [],
  }) => {

    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.put(
        `${gatewayUrl}/hcm/iam/user/${id}`,
        {
          performedBy,
          fullName,
          dateOfBirth,
          gender,
          address,
          email,
          phoneNumber,
          userRoleUpdateDTOs,
          userPrivilegeUpdateDTOs,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setInfo(response?.data?.message || "User updated successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to update user");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  const deleteUser = async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/iam/user/${id}`,
        {
          data: { performedBy },
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setInfo(response?.data?.message || "Deletion is successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to delete user");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  const getUserById = async ({ id }) => {

    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(`${gatewayUrl}/hcm/iam/user/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to load user detail");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { getSortedUserList, addUser, deleteUser, updateUser, getUserById };
}
