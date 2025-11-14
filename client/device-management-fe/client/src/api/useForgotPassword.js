import { useState } from "react";
import axios from "axios";

export function useForgotPassword({ setLoading, setError, setInfo }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const forgotPassword = async ({ email }) => {
    setLoading(true);
    setError(null);

    try {
      const response = await axios.post(`${gatewayUrl}/hcm/iam/auth/forgot-password`, {
        email
      });

      setInfo(response?.data?.message);
    } catch (err) {
      setError(err.response?.data?.message || "Request failed");
      setLoading(false);
      return null;
    } finally {
      setLoading(false);
    }
  };

  const resetPassword = async ({ resetToken, newPassword, confirmPassword }) => {
    setLoading(true);
    setError(null);

    try {
      const response = await axios.post(`${gatewayUrl}/hcm/iam/auth/reset-password`, {
        resetToken,
        newPassword,
        confirmPassword
      });

      setInfo(response?.data?.message);
    } catch (err) {
      setError(err.response?.data?.message || "Reset failed");
      setLoading(false);
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { forgotPassword, resetPassword };
}
