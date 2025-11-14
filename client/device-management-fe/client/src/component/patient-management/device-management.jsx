import React, { useState, useEffect } from "react";
import { useEdgeDevices } from "../../api/useEdgeDevices";
import Loading from "../loading/loading";
import InfoBox from "../info-box/info-box";
import ConfirmBox from "../confirm-box/confirm-box";
import CreateDeviceForm from "./create-device-form";
import UpdateDeviceForm from "./update-device-form";
import DeviceDetail from "./device-detail";
import "./device-management.css";

function DeviceManagement() {
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);
  const [reload, setReload] = useState(0);
  const [reloadDetail, setReloadDetail] = useState(0);

  const [openCreateForm, setOpenCreateForm] = useState(false);
  const [updatedDevice, setUpdatedDevice] = useState(null);
  const [detailedDevice, setDetailedDevice] = useState(null);
  const [deletedDevice, setDeletedDevice] = useState(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);
  const [reactiveDevice, setReactiveDevice] = useState(null);
  const [showConfirmReactive, setShowConfirmReactive] = useState(false);

  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 5;
  const [isLastPage, setIsLastPage] = useState(false);
  const [search, setSearch] = useState("");

  const {
    getDevices,
    addDevice,
    updateDevice,
    deleteDevice,
    getDeviceById,
    assignController,
    assignSensor,
    unassignSensor,
    unassignController,
    reactivateController,
    reactivateEdge,
    reactivateSensor
  } = useEdgeDevices({ setLoading, setError, setInfo, setReload });

  useEffect(() => {
    const fetchDevices = async () => {
      const data = await getDevices({
        search,
        pageIndex: currentPage,
        pageLength: pageSize,
        sortBy: ""
      });
      if (data) setIsLastPage(data.length < pageSize);
      setDevices(data);
    };
    fetchDevices();
  }, [reload, currentPage, search]);

  useEffect(() => {
    const fetchDeviceDetail = async () => {
      if (error) return;
      if (!detailedDevice) return;
      const data = await getDeviceById({ edgeDeviceID: detailedDevice.edgeDeviceID });
      setDetailedDevice(data);
    };
    fetchDeviceDetail();
  }, [reloadDetail]);

  const handleDelete = (device) => {
    setDeletedDevice(device);
    setShowConfirmDelete(true);
  };

  const handleUpdate = async (device) => {
    const data = await getDeviceById({ edgeDeviceID: device.edgeDeviceID });
    setUpdatedDevice(data);
  };

  const handleDetail = async (device) => {
    const data = await getDeviceById({ edgeDeviceID: device.edgeDeviceID });
    setDetailedDevice(data);
  };

  const handleReactive = async (device) => {
    setReactiveDevice(device);
    setShowConfirmReactive(true);
  };

  return (
    <div id="device-management">
      {loading && <Loading />}
      {error && <InfoBox title="Error" message={error} onClose={() => setError(null)} />}
      {info && <InfoBox title="Info" message={info} onClose={() => setInfo(null)} />}
      {showConfirmDelete && deletedDevice && (
        <ConfirmBox
          title="Confirm Delete"
          message={`Are you sure you want to delete ${deletedDevice.deviceKey}?`}
          onConfirm={async () => {
            const result = await deleteDevice({ edgeDeviceID: deletedDevice.edgeDeviceID });
            if (result) setReload((prev) => prev + 1);
            setShowConfirmDelete(false);
          }}
          onCancel={() => setShowConfirmDelete(false)}
        />
      )}

      {showConfirmReactive && reactiveDevice && (
        <ConfirmBox
          title="Confirm Reactive"
          message={`Are you sure you want to reactive ${reactiveDevice.edgeKey}?`}
          onConfirm={async () => {
            const result = await reactivateEdge({ edgeKey: reactiveDevice.edgeKey });
            if (result) setReload((prev) => prev + 1);
            setShowConfirmReactive(false);
          }}
          onCancel={() => setShowConfirmReactive(false)}
        />
      )}

      {/* Header */}
      <div className="header">
        <div>Device Management</div>
        <div className="filter">
          <input
            type="text"
            placeholder="Search by device key or name..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button onClick={() => setCurrentPage(1)}>Search</button>
        </div>
        <button onClick={() => setOpenCreateForm(true)}>Create</button>
      </div>

      {/* Device list */}
      {devices?.length > 0 ? (
        <div className="main">
          <div className={`page  ${detailedDevice ? "less" : ""}`}>
            <div className={`device-list  ${detailedDevice ? "less" : ""}`}>
              <div className={`device-list-header ${detailedDevice ? "less" : ""}`}>
                <div>IP Address</div>
                {!detailedDevice && (<div>Room Name</div>)}
                {!detailedDevice && (<div>Edge Key</div>)}
                <div>Actions</div>
              </div>

              {devices.map((d, idx) => (
                <div
                  key={idx}
                  className={`device-card ${d.isActive ? "" : "gray-out"} ${detailedDevice ? "less" : ""}`}
                >
                  <div>{d.ipAddress}</div>
                  {!detailedDevice && (<div>{d.roomName}</div>)}
                  {!detailedDevice && (<div>{d.edgeKey}</div>)}
                  <div className="device-actions">
                    {!detailedDevice && (
                      <button className="btn delete" onClick={() => handleDelete(d)}>Delete</button>
                    )}
                    {!detailedDevice && (
                      <button className="btn update" onClick={() => handleUpdate(d)}>Update</button>
                    )}
                    <button className="btn detail" onClick={() => handleDetail(d)}>Detail</button>
                    {!detailedDevice && (
                      <button className="btn detail non-gray-out" onClick={() => handleReactive(d)}>Reactive</button>
                    )}
                  </div>
                </div>
              ))}
            </div>

            {/* Pagination */}
            <div className="paging">
              <button
                className="page-btn"
                disabled={currentPage === 1}
                onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
              >
                ‹ Prev
              </button>
              <span className="page-info">Page {currentPage}</span>
              <button
                className="page-btn"
                disabled={isLastPage}
                onClick={() => setCurrentPage((p) => p + 1)}
              >
                Next ›
              </button>
            </div>
          </div>

          {/* Device Detail */}
          <DeviceDetail
            visible={!!detailedDevice}
            onClose={() => setDetailedDevice(null)}
            device={detailedDevice}
            assignController={assignController}
            assignSensor={assignSensor}
            unassignController={unassignController}
            unassignSensor={unassignSensor}
            setReloadDetail={setReloadDetail}
            reactivateController={reactivateController}
            reactivateSensor={reactivateSensor}
          />
        </div>
      ) : (
        <div className="empty-list">No devices found</div>
      )}

      {/* Forms */}
      <CreateDeviceForm
        visible={openCreateForm}
        onClose={() => setOpenCreateForm(false)}
        addDevice={addDevice}
      />

      {updatedDevice && (
        <UpdateDeviceForm
          visible={!!updatedDevice}
          deviceData={updatedDevice}
          onClose={() => setUpdatedDevice(null)}
          updateDevice={updateDevice}
        />
      )}
    </div>
  );
}

export default DeviceManagement;
