import "./change-password.css";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useChangePassword } from "../../api/useChangePassword";

import InfoBox from "../info-box/info-box";
import Loading from "../loading/loading";

function ChangePassword() {
    const [info, setInfo] = useState(null);
    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const navigate = useNavigate();
    const { 
        changePassword
    } = useChangePassword({ setLoading, setError, setInfo });

    const handleSubmit = async (e) => {
        e.preventDefault();
        changePassword({ confirmPassword, newPassword, oldPassword });
    }

    return (
        <div id="change-password">
            <h2>Change password</h2>
            <form onSubmit={handleSubmit}>
                <label>Old password</label>
                <input
                    type="text"
                    value={oldPassword}
                    onChange={(e) => setOldPassword(e.target.value)}
                    required
                />

                <label>New password</label>
                <input
                    type="text"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    required
                />

                <label>Confirm password</label>
                <input
                    type="text"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                />
                <button type="submit">Request change password</button>
            </form>

            {loading && (<Loading />)}
            {error && (<InfoBox message={error} onClose={() => setError(null)} />)}
            {info && <InfoBox title="Information" message={info} onClose={() => {
                setInfo(null);
                navigate('/login');
            }} />}
        </div>
    );
}

export default ChangePassword;
