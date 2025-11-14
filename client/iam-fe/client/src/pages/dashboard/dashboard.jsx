import React, { useState } from "react";
import "./dashboard.css";
import UserManagement from "../../component/user-management/user-management";
import RoleManagement from "../../component/role-management/role-management";
import PrivilegeManagement from "../../component/privilege-management/privilege-management";
import ChangePassword from "../../component/change-password/change-password";
import Loading from "../../component/loading/loading";
import InfoBox from "../../component/info-box/info-box";
import ConfirmBox from "../../component/confirm-box/confirm-box";
import { useNavigate } from "react-router-dom";
import { useLogout } from "../../api/useLogout";

function Dashboard() {
  const tabs = [
    { id: "user-management", label: "User management", component: <UserManagement /> },
    { id: "role-management", label: "Role management", component: <RoleManagement /> },
    { id: "privilege-management", label: "Privilege management", component: <PrivilegeManagement /> },
    { id: "change-password", label: "Change password", component: <ChangePassword /> }
  ];

  const [activeTab, setActiveTab] = useState(tabs[0].id);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [confirm, setConfirm] = useState(null);
  const navigate = useNavigate('');
  const { logout } = useLogout({ setError, setLoading });

  const currentTab = tabs.find(tab => tab.id === activeTab);

  return (
    <div id="dashboard">
      {loading && <Loading />}
      {error && <InfoBox message={error} onClose={() => setError(null)} title="Error" />}
      {confirm && (
        <ConfirmBox
          message={confirm}
          onConfirm={() => {
            logout();
            setConfirm(null);
            navigate('/login');
          }}
          onCancel={() => setConfirm(null)}
        />
      )}

      <div className="dashboard-sidebar">
        <h2 className="sidebar-logo">IAM Service</h2>
        <ul className="sidebar-tabs">
          {tabs.map(tab => (
            <li
              key={tab.id}
              className={activeTab === tab.id ? "active" : ""}
              onClick={() => setActiveTab(tab.id)}
            >
              {tab.label}
            </li>
          ))}
          <li onClick={() => setConfirm("Do you want to logout?")}>
            Logout
          </li>
        </ul>
      </div>

      <div className="dashboard-main">{currentTab?.component}</div>
    </div>
  );
}

export default Dashboard;
