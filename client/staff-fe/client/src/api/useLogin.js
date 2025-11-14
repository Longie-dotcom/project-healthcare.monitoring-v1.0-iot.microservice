import { useState } from "react";
import axios from "axios";

export function useLogin({ setLoading, setError }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const login = async ({ email, password, roleCode }) => {
    setLoading(true);
    setError(null);

    try {
      const response = await axios.post(`${gatewayUrl}/hcm/iam/auth/login`, {
        email,
        password,
        roleCode,
      });

      const { accessToken, refreshToken } = response.data.payload;

      // Save tokens to localStorage
      localStorage.setItem("accessToken", accessToken);
      localStorage.setItem("refreshToken", refreshToken);

      return { accessToken, refreshToken };
    } catch (err) {
      setError(err.response?.data?.message || "Login failed");
      setLoading(false);
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { login };
}
