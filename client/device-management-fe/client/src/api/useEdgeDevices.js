import api from "../helper/refreshTokenInstance";
import { jwtDecode } from "jwt-decode";

export function useEdgeDevices({ setLoading, setError, setInfo, setReload }) {
  const gatewayUrl = import.meta.env.VITE_GATEWAY_URL;

  // Get device list with query and sort
  const getDevices = async ({
    sort = "SORT_BY_ID",
    pageIndex = 1,
    pageLength = 10,
    search = ""
  }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(
        `${gatewayUrl}/hcm/device-management/edge-device`, {
        params: {
          sort,
          PageIndex: pageIndex,
          PageLength: pageLength,
          Search: search || undefined,
        },
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to load devices");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Get device by ID
  const getDeviceById = async ({ edgeDeviceID }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");

      const response = await api.get(
        `${gatewayUrl}/hcm/device-management/edge-device/${edgeDeviceID}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );

      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to load device detail");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Create a new device
  const addDevice = async ({ roomName, ipAddress, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/device-management/edge-device`,
        { performedBy, roomName, ipAddress, description },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Device created successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to create device");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Update device
  const updateDevice = async ({ edgeDeviceID, roomName, ipAddress, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.put(
        `${gatewayUrl}/hcm/device-management/edge-device/${edgeDeviceID}`,
        { performedBy, roomName, ipAddress, description },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Device updated successfully");
      setReload((prev) => prev + 1);
      return response.data.payload;
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to update device");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Delete device
  const deleteDevice = async ({ edgeDeviceID }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/device-management/edge-device/${edgeDeviceID}`,
        { data: { performedBy }, headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Device deleted successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      if (err.response?.status === 401) {
        setError("User has no permission to access this function");
      } else {
        setError(err.response?.data?.message || "Unable to delete device");
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Assign controller
  const assignController = async ({ edgeKey, bedNumber, ipAddress, firmwareVersion }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/device-management/edge-device/controller`,
        { edgeKey, bedNumber, ipAddress, firmwareVersion, performedBy },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Controller assigned successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to assign controller");
    } finally {
      setLoading(false);
    }
  };

  // Assign sensor
  const assignSensor = async (
    { controllerKey, type, unit, description }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/device-management/edge-device/sensor`,
        { controllerKey, type, unit, description, performedBy },
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Sensor assigned successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to assign sensor");
    } finally {
      setLoading(false);
    }
  };

  // Assign controller
  const unassignController = async ({ controllerKey }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/device-management/edge-device/controller`,
        { 
          data: { controllerKey, performedBy },
          headers: { Authorization: `Bearer ${token}` } 
        }
      );

      setInfo(response?.data?.message || "Controller unassigned successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to unassign controller");
    } finally {
      setLoading(false);
    }
  };

  // Assign sensor
  const unassignSensor = async (
    { sensorKey }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.delete(
        `${gatewayUrl}/hcm/device-management/edge-device/sensor`,
        { 
          data: { sensorKey, performedBy },
          headers: { Authorization: `Bearer ${token}` } 
        }
      );

      setInfo(response?.data?.message || "Sensor unassigned successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to unassign sensor");
    } finally {
      setLoading(false);
    }
  };

    // Reactivate EdgeDevice
  const reactivateEdge = async ({ edgeKey }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/device-management/edge-device/reactivate/edge/${edgeKey}`,
        null,
        { params: { performedBy }, headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "EdgeDevice reactivated successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to reactivate EdgeDevice");
    } finally {
      setLoading(false);
    }
  };

  // Reactivate Controller
  const reactivateController = async ({ controllerKey }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/device-management/edge-device/reactivate/controller/${controllerKey}`,
        null,
        { params: { performedBy }, headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Controller reactivated successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to reactivate Controller");
    } finally {
      setLoading(false);
    }
  };

  // Reactivate Sensor
  const reactivateSensor = async ({ sensorKey }) => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("accessToken");
      const decodedToken = jwtDecode(token);
      const performedBy = decodedToken.FullName || "Unknown User";

      const response = await api.post(
        `${gatewayUrl}/hcm/device-management/edge-device/reactivate/sensor/${sensorKey}`,
        null,
        { params: { performedBy }, headers: { Authorization: `Bearer ${token}` } }
      );

      setInfo(response?.data?.message || "Sensor reactivated successfully");
      setReload((prev) => prev + 1);
    } catch (err) {
      setError(err.response?.data?.message || "Unable to reactivate Sensor");
    } finally {
      setLoading(false);
    }
  };

  return {
    getDevices,
    getDeviceById,
    addDevice,
    updateDevice,
    deleteDevice,
    assignController,
    assignSensor,
    unassignSensor,
    unassignController,
    reactivateEdge,
    reactivateController,
    reactivateSensor
  };
}
