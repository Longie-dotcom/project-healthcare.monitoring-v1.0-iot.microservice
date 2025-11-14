import { jwtDecode } from "jwt-decode";
import api from "../helper/refreshTokenInstance";

export function useChangePassword({ setLoading, setError, setInfo }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const changePassword = async ({ oldPassword, newPassword, confirmPassword }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      if (!token) throw new Error("No access token found");

      const decodedToken = jwtDecode(token);

      // The claim keys depend on how you set them in your backend
      const performedBy = decodedToken.FullName || "Unknown User";
      const id =
        decodedToken.nameid || // standard .NET mapping
        decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

      const response = await api.put(
        `${gatewayUrl}/hcm/iam/user/${id}/change-password`,
        {
          oldPassword,
          newPassword,
          newConfirmedPassword: confirmPassword,
          performedBy,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      
      setInfo("Password updated successfully");
      return response.data.payload;
    } catch (err) {
      console.error("Change password error:", err);
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to change password");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { changePassword };
}
