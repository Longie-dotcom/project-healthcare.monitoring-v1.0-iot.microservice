import "./login.css";
import { useNavigate } from "react-router-dom";
import { useLogin } from "../../api/useLogin";
import { useState } from "react";

import InfoBox from "../../component/info-box/info-box";
import Loading from "../../component/loading/loading";

function LoginPage() {
  const ROLE_PATIENT_FAMILY = import.meta.env.VITE_ROLE_PATIENT_FAMILY;
  const ROLE_ADMIN = import.meta.env.VITE_ROLE_ADMIN;
  const ROLE_DOCTOR = import.meta.env.VITE_ROLE_DOCTOR;
  const ROLE_NURSE = import.meta.env.VITE_ROLE_NURSE;

  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [roleCode, setRoleCode] = useState(ROLE_ADMIN);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const {
    login
  } = useLogin({ setLoading, setError });

  const handleSubmit = async (e) => {
    e.preventDefault();
    const result = await login({ email, password, roleCode });
    if (result) {
      navigate("/dashboard");
    }
  }

  return (
    <div id="login">
      <h2>Sign in</h2>
      <form onSubmit={handleSubmit}>
        <label>Email</label>
        <input
          type="text"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <label>Password</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <label>Sign in with role permission</label>
        <select value={roleCode} onChange={(e) => setRoleCode(e.target.value)}>
          <option value={ROLE_ADMIN}>
            Admin
          </option>
          <option value={ROLE_DOCTOR}>
            Doctor
          </option>
          <option value={ROLE_NURSE}>
            Nurse
          </option>
          <option value={ROLE_PATIENT_FAMILY}>
            Patient Family
          </option>
        </select>
        <button type="submit">Sign in</button>
        <a href="/forgot-password">Forgot password</a>
      </form>

      {loading && (<Loading />)}
      {error && (<InfoBox message={error} onClose={() => setError(null)} />)}
    </div>
  );
}

export default LoginPage;
