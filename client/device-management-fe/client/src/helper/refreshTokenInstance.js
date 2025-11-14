import axios from "axios";
import { jwtDecode } from "jwt-decode";

const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

const api = axios.create({
  baseURL: gatewayUrl,
});

// Check if token expired
function isTokenExpired(token) {
    try {
        const { exp } = jwtDecode(token);
        return Date.now() >= exp * 1000;
    } catch {
        return true;
    }
}

// Decode needed info (email + role) from token
function decodeTokenInfo(token) {
  try {
    if (!token) return {};
    const decoded = jwtDecode(token);

    const email =
      decoded.email ||
      decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];

    const roleCode =
      decoded.role ||
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    return { email, roleCode };
  } catch (e) {
    console.error("Failed to decode:", e);
    return {};
  }
}


// Refresh token API call
async function refreshAccessToken() {
  const refreshToken = localStorage.getItem("refreshToken");
  const accessToken = localStorage.getItem("accessToken");
  
  if (!refreshToken) return null;
  // Decode both email & role from the refresh token
  const { email, roleCode } = decodeTokenInfo(accessToken);

  if (!email || !roleCode) {
    console.error("Missing email or role in access token");
    return null;
  }
  
  try {
    const response = await axios.post(`${gatewayUrl}/iam/auth/refresh-token`, {
      refreshToken,
      email,
      roleCode,
    });

    const accessToken = response.data;
    localStorage.setItem("accessToken", accessToken);

    return accessToken;
  } catch (err) {
    console.error("Failed to refresh token:", err);
    return null;
  }
}

// Axios interceptor
api.interceptors.request.use(async (config) => {
  let token = localStorage.getItem("accessToken");

  if (token && isTokenExpired(token)) {
    console.warn("Access token expired, attempting refresh...");
    
    // Refresh token
    const newToken = await refreshAccessToken();
    
    if (newToken) {
      token = newToken;
    } else {
      // Optional: redirect to login if refresh fails
      console.error("Unable to refresh token, redirect to login");
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      window.location.href = "/login";
      return Promise.reject("No valid access token");
    }
  }

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
}, (error) => {
  return Promise.reject(error);
});

export default api;
