import axios from "axios";
import { jwtDecode } from "jwt-decode";

export function useLogout({ setLoading, setError }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const logout = async () => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      if (!token) throw new Error("No access token found");

      const decoded = jwtDecode(token);
      const email =
        decoded.email ||
        decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];

      if (!email) throw new Error("Email not found in token");

      const response = await axios.get(`${gatewayUrl}/hcm/iam/auth/logout/${email}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");

      return response.data.payload;
    } catch (err) {
      setError(err.response?.data?.message || err.message || "Logout failed");
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { logout };
}
