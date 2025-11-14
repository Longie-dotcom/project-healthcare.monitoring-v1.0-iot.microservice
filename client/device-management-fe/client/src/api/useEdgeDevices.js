import api from "../helper/refreshTokenInstance";
import jwtDecode from "jwt-decode";

export function useEdgeDevices({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  const getTokenAndUser = () => {
    const token = localStorage.getItem("accessToken");
    const decoded = jwtDecode(token);
    const performedBy = decoded.FullName || "Unknown User";
    return { token, performedBy };
  };

  // ------------------------
  // EdgeDevice endpoints
  // ------------------------
  const getDevices = async ({ sort = "", pageIndex = 1, pageLength = 10, search = "" }) => {
    setLoading(true);
    setError(null);
    try {
      const { token } = getTokenAndUser();
      const response = await api.get(`${gatewayUrl}/api/EdgeDevice`, {
        params: { sort, PageIndex: pageIndex, PageLength: pageLength, Search: search || undefined },
        headers: { Authorization: `Bearer ${token}` },
      });
      return response.data;
    } catch (err) {
      setError(err.response?.data?.message || "Unable to load devices");
      return null;
    } finally { setLoading(false); }
  };

  const getDeviceById = async (edgeDeviceID) => {
    setLoading(true);
    setError(null);
    try {
      const { token } = getTokenAndUser();
      const response = await api.get(`${gatewayUrl}/api/EdgeDevice/${edgeDeviceID}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      return response.data;
    } catch (err) {
      setError(err.response?.data?.message || "Unable to load device detail");
      return null;
    } finally { setLoading(false); }
  };

  const createDevice = async ({ roomName, ipAddress, description }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      const response = await api.post(`${gatewayUrl}/api/EdgeDevice`, { performedBy, roomName, ipAddress, description },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Device created successfully");
      setReload(prev => prev + 1);
      return response.data;
    } catch (err) {
      setError(err.response?.data?.message || "Unable to create device");
      return null;
    } finally { setLoading(false); }
  };

  const updateDevice = async ({ edgeDeviceID, roomName, ipAddress, description }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      const response = await api.put(`${gatewayUrl}/api/EdgeDevice/${edgeDeviceID}`, { performedBy, roomName, ipAddress, description },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Device updated successfully");
      setReload(prev => prev + 1);
      return response.data;
    } catch (err) {
      setError(err.response?.data?.message || "Unable to update device");
      return null;
    } finally { setLoading(false); }
  };

  const deactivateDevice = async ({ edgeDeviceID }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      await api.post(`${gatewayUrl}/api/EdgeDevice/${edgeDeviceID}/deactivate`, { performedBy },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Edge device deactivated successfully");
      setReload(prev => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to deactivate device");
    } finally { setLoading(false); }
  };

  // ------------------------
  // Controller endpoints
  // ------------------------
  const assignController = async ({ edgeKey, bedNumber, ipAddress, firmwareVersion }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      await api.post(`${gatewayUrl}/api/EdgeDevice/controller`, { edgeKey, bedNumber, ipAddress, firmwareVersion, performedBy },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Controller assigned successfully");
      setReload(prev => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to assign controller");
    } finally { setLoading(false); }
  };

  const updateController = async ({ edgeKey, controllerKey, bedNumber, ipAddress, firmwareVersion, isActive }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      const response = await api.put(`${gatewayUrl}/api/EdgeDevice/controller`,
        { edgeKey, controllerKey, bedNumber, ipAddress, firmwareVersion, IsActive: isActive, PerformedBy: performedBy },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Controller updated successfully");
      setReload(prev => prev + 1);
      return response.data;
    } catch (err) {
      setError(err.response?.data?.message || "Unable to update controller");
      return null;
    } finally { setLoading(false); }
  };

  const unassignController = async ({ edgeKey, controllerKey }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      await api.delete(`${gatewayUrl}/api/EdgeDevice/controller`,
        { data: { edgeKey, controllerKey, performedBy }, headers: { Authorization: `Bearer ${token}` } });
      setInfo("Controller unassigned successfully");
      setReload(prev => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to unassign controller");
    } finally { setLoading(false); }
  };

  // ------------------------
  // Sensor endpoints
  // ------------------------
  const assignSensor = async ({ edgeKey, controllerKey, type, unit, description }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      await api.post(`${gatewayUrl}/api/EdgeDevice/sensor`,
        { edgeKey, controllerKey, type, unit, description, performedBy },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Sensor assigned successfully");
      setReload(prev => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to assign sensor");
    } finally { setLoading(false); }
  };

  const updateSensor = async ({ edgeKey, controllerKey, sensorKey, type, unit, description, isActive }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      const response = await api.put(`${gatewayUrl}/api/EdgeDevice/sensor`,
        { edgeKey, controllerKey, sensorKey, type, unit, description, IsActive: isActive, PerformedBy: performedBy },
        { headers: { Authorization: `Bearer ${token}` } });
      setInfo("Sensor updated successfully");
      setReload(prev => prev + 1);
      return response.data;
    } catch (err) {
      setError(err.response?.data?.message || "Unable to update sensor");
      return null;
    } finally { setLoading(false); }
  };

  const unassignSensor = async ({ edgeKey, controllerKey, sensorKey }) => {
    setLoading(true);
    setError(null);
    try {
      const { token, performedBy } = getTokenAndUser();
      await api.delete(`${gatewayUrl}/api/EdgeDevice/sensor`,
        { data: { edgeKey, controllerKey, sensorKey, performedBy }, headers: { Authorization: `Bearer ${token}` } });
      setInfo("Sensor unassigned successfully");
      setReload(prev => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to unassign sensor");
    } finally { setLoading(false); }
  };

  return {
    getDevices,
    getDeviceById,
    createDevice,
    updateDevice,
    deactivateDevice,
    assignController,
    updateController,
    unassignController,
    assignSensor,
    updateSensor,
    unassignSensor
  };
}
