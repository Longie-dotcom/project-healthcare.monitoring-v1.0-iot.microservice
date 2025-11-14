import React, { useState, useEffect } from "react";
import "./role-management.css";
import { getRoleList, deleteRole, getRoleDetail, createRole, updateRole } from "../../api/useRoles";
import RoleForm from "./role-changes";
import { Modal, Spin, List } from "antd";
import Loading from "../loading/loading";
import InfoBox from "../info-box/info-box";
import ConfirmBox from "../confirm-box/confirm-box";
import { usePrivileges } from "../../api/usePrivileges";

const RoleManagementPage = () => {
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [reload, setReload] = useState(null);
  const [confirm, setConfirm] = useState(null);
  const [openForm, setOpenForm] = useState(false);
  const [isEdit, setIsEdit] = useState(false);
  const [selectedRole, setSelectedRole] = useState(null);
  const [deletedRole, setDeletedRole] = useState(null);

  const [detailVisible, setDetailVisible] = useState(false);
  const [detailData, setDetailData] = useState(null);
  const [detailLoading, setDetailLoading] = useState(false);

  const {
    getPrivilegeList
  } = usePrivileges({setError, setLoading, setReload, setInfo});

  const fetchRoles = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getRoleList();
      setRoles(data);
    } catch (err) {
      setError("Can not get role list.");
    } finally {
      setLoading(false);
    }
  };

  const handleUpdate = (role) => {
    setSelectedRole(role);
    setIsEdit(true);
    setOpenForm(true);
  };

  const handleDelete = async (roleID) => {
    setDeletedRole(roleID);
    setConfirm("Confirm deleting this role?");
  };

  const handleAddRole = () => {
    setSelectedRole(null);
    setIsEdit(false);
    setOpenForm(true);
  };

  const handleFormClose = () => {
    setOpenForm(false);
    fetchRoles();
  };

  const handleDetail = async (roleID) => {
    setDetailVisible(true);
    setDetailLoading(true);
    try {
      const detail = await getRoleDetail(roleID);
      setDetailData(detail);
    } catch (err) {
      setError("Role detail can not be loaded");
      setDetailData(null);
    } finally {
      setDetailLoading(false);
    }
  };

  useEffect(() => {
    fetchRoles();
  }, [reload]);

  return (
    <div id="role-management">
      <div className="header">
        <div>Role Management</div>
        <div className="function">
          <button onClick={fetchRoles}>Refresh</button>
          <button onClick={handleAddRole}>Create</button>
        </div>
      </div>

      {loading && (<Loading />)}
      {error && (<InfoBox message={error} onClose={() => setError(null)} title={'Error'} />)}
      {info && (<InfoBox message={info} onClose={() => setInfo(null)} title={'Info'} />)}

      <div className="role-list">
        <div className="role-list-header">
          <div>Role Code</div>
          <div>Role Name</div>
          <div>Description</div>
          <div>Actions</div>
        </div>

        {roles.map((role) => (
          <div key={role.roleID} className="role-card">
            <div>{role.roleCode}</div>
            <div>{role.name}</div>
            <div>{role.description}</div>
            <div className="role-actions">
              <button
                className="btn detail"
                onClick={() => handleDetail(role.roleID)}
              >
                Detail
              </button>
              <button
                className="btn update"
                onClick={() => handleUpdate(role)}
              >
                Update
              </button>
              <button
                className="btn delete"
                onClick={() => handleDelete(role.roleID)}
              >
                Delete
              </button>
            </div>
          </div>
        ))}

        {!loading && roles.length === 0 && (
          <p style={{ textAlign: "center" }}>The role list is empty</p>
        )}
      </div>

      {openForm && (
        <RoleForm
          visible={openForm}
          onClose={handleFormClose}
          roleData={selectedRole}
          isEdit={isEdit}
          setError={setError}
          setInfo={setInfo}
          createRole={createRole}
          getRolePrivileges={getPrivilegeList}
          updateRole={updateRole}
        />
      )}

      {confirm && (
        <ConfirmBox
          message={confirm}
          onConfirm={async () => {
            try {
              await deleteRole(deletedRole);
              setInfo("Deletion is successfully");
              setConfirm(null);
              fetchRoles();
            } catch (err) {
              setConfirm(null);
              setError("Lỗi khi xóa role:", err);
            }
          }}
          onCancel={() => setConfirm(null)}
        />
      )}

      <Modal
        title="Role details"
        open={detailVisible}
        onCancel={() => setDetailVisible(false)}
        footer={null}
      >
        {detailLoading ? (
          <Spin tip="The detail is loading." />
        ) : detailData ? (
          <div>
            <p><strong>Role code:</strong> {detailData.roleCode}</p>
            <p><strong>Role name:</strong> {detailData.name}</p>
            <p><strong>Description:</strong> {detailData.description}</p>

            <p><strong>Privileges list:</strong></p>
            {detailData.privileges && detailData.privileges.length > 0 ? (
              <List
                size="small"
                bordered
                dataSource={detailData.privileges}
                renderItem={(item) => (
                  <List.Item key={item.privilegeID}>
                    <div>
                      <strong>{item.name}</strong> – {item.description}
                    </div>
                  </List.Item>
                )}
              />
            ) : (
              <p>There is no privilege for this role</p>
            )}
          </div>
        ) : (
          <p>Can not load this role detail</p>
        )}
      </Modal>

    </div>
  );
};

export default RoleManagementPage;
