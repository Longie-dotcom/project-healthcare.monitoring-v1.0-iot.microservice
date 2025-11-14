import "./reset-password.css";
import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useForgotPassword } from "../../api/useForgotPassword";

import InfoBox from "../../component/info-box/info-box";
import Loading from "../../component/loading/loading";

function ResetPassword() {
    const [info, setInfo] = useState(null);
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const resetToken = searchParams.get("token");
    console.log(resetToken);
    const {
        resetPassword
    } = useForgotPassword({ setLoading, setError, setInfo });

    const handleSubmit = async (e) => {
        e.preventDefault();
        resetPassword({ resetToken, confirmPassword, newPassword });
    }

    return (
        <div id="reset-password">
            <h2>Reset password</h2>
            <form onSubmit={handleSubmit}>

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
                <button type="submit">Request reset password</button>
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

export default ResetPassword;
