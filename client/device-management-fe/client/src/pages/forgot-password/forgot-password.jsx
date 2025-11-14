import "./forgot-password.css";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useForgotPassword } from "../../api/useForgotPassword";

import InfoBox from "../../component/info-box/info-box";
import Loading from "../../component/loading/loading";

function ForgotPasswordPage() {
    const [info, setInfo] = useState(null);
    const [email, setEmail] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const { 
        forgotPassword
    } = useForgotPassword({ setLoading, setError, setInfo });

    const handleSubmit = async (e) => {
        e.preventDefault();
        forgotPassword({ email });
    }

    return (
        <div id="forgot-password">
            <h2>Forgot password</h2>
            <form onSubmit={handleSubmit}>

                <label>Email</label>
                <input
                    type="text"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
                <button type="submit">Request forgot password</button>
            </form>

            {loading && (<Loading />)}
            {error && (<InfoBox message={error} onClose={() => setError(null)} />)}
            {info && <InfoBox title="Information" message={info} onClose={() => setInfo(null)} />}
        </div>
    );
}

export default ForgotPasswordPage;
