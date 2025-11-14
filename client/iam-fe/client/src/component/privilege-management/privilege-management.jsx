import React, { useState, useEffect } from "react";
import { usePrivileges } from "../../api/usePrivileges";
import InfoBox from "../info-box/info-box";
import Loading from "../loading/loading";
import ConfirmBox from "../confirm-box/confirm-box";
import CreatePrivilegeForm from "./create-privilege-form";
import UpdatePrivilegeForm from "./update-privilege-form";
import PrivilegeDetail from "./privilege-detail";
import "./privilege-management.css";

function PrivilegeManagement() {
  const [privileges, setPrivileges] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [reload, setReload] = useState(0);

  const [openForm, setOpenForm] = useState(false);
  const [updatedPrivilege, setUpdatedPrivilege] = useState(null);
  const [detailedPrivilege, setDetailedPrivilege] = useState(null);
  const [deletedPrivilege, setDeletedPrivilege] = useState(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);

  const { getPrivilegeList, addPrivilege, updatePrivilege, deletePrivilege, getPrivilegeById } =
    usePrivileges({ setLoading, setError, setInfo, setReload });

  useEffect(() => {
    const fetchPrivileges = async () => {
      const data = await getPrivilegeList();
      if (data) setPrivileges(data);
    };
    fetchPrivileges();
  }, [reload]);

  const handleDelete = (p) => {
    setDeletedPrivilege(p);
    setShowConfirmDelete(true);
  };
  const handleUpdate = (p) => setUpdatedPrivilege(p);
  const handleDetail = async (p) => {
    const detail = await getPrivilegeById({ id: p.privilegeID });
    setDetailedPrivilege(detail);
  };

  return (
    <div id="privilege-management">
      {loading && <Loading />}
      {error && <InfoBox title="Error" message={error} onClose={() => setError(null)} />}
      {info && <InfoBox title="Information" message={info} onClose={() => setInfo(null)} />}

      {showConfirmDelete && (
        <ConfirmBox
          title="Delete Confirmation"
          message={`Delete privilege "${deletedPrivilege.name}"?`}
          onConfirm={() => {
            deletePrivilege({ id: deletedPrivilege.privilegeID });
            setShowConfirmDelete(false);
          }}
          onCancel={() => setShowConfirmDelete(false)}
        />
      )}

      <div className="header">
        <div>Privilege Management</div>
        <button onClick={() => setOpenForm(true)}>Create</button>
      </div>

      {privileges.length > 0 ? (
        <div className="privilege-list">
          <div className="privilege-list-header">
            <div>Name</div>
            <div>Description</div>
            <div>Action</div>
          </div>
          {privileges.map((p, index) => (
            <div key={index} className="privilege-card">
              <div>{p.name}</div>
              <div>{p.description}</div>
              <div className="privilege-actions">
                <button className="btn delete" onClick={() => handleDelete(p)}>Delete</button>
                <button className="btn update" onClick={() => handleUpdate(p)}>Update</button>
                <button className="btn detail" onClick={() => handleDetail(p)}>Detail</button>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="empty-list">No privileges found</div>
      )}

      <CreatePrivilegeForm
        visible={openForm}
        onClose={() => setOpenForm(false)}
        addPrivilege={addPrivilege}
      />

      <UpdatePrivilegeForm
        visible={updatedPrivilege}
        onClose={() => setUpdatedPrivilege(null)}
        privilegeData={updatedPrivilege}
        updatePrivilege={updatePrivilege}
      />

      <PrivilegeDetail
        visible={detailedPrivilege}
        onClose={() => setDetailedPrivilege(null)}
        privilege={detailedPrivilege}
      />
    </div>
  );
}

export default PrivilegeManagement;
