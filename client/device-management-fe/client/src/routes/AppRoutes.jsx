import React from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "../pages/login/login";
import Dashboard from "../pages/dashboard/dashboard";
import ForgotPasswordPage from "../pages/forgot-password/forgot-password";
import ResetPassword from "../pages/forgot-password/reset-password";

const Auth = {
  isLoggedIn: () => !!localStorage.getItem("accessToken"),
};

function ProtectedRoute({ children }) {
  return Auth.isLoggedIn() ? children : <Navigate to="/login" replace />;
}

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        }
      />

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}
