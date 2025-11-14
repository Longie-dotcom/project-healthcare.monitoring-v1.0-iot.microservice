import axios from "axios";
import { jwtDecode } from "jwt-decode";

const GATEWAY_URL = import.meta.env.VITE_GATEWAY_URL;

const api = axios.create({
  baseURL: GATEWAY_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

export const getRoleList = async () => {
  try {
    const token = localStorage.getItem("accessToken");
    const response = await api.get("/hcm/iam/role", {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data.payload;
  } catch (error) {
    console.error("Lỗi khi lấy danh sách roles:", error);
    throw error;
  }
};

export const getRoleDetail = async (roleId) => {
  try {
    const token = localStorage.getItem("accessToken");
    const response = await api.get(`/hcm/iam/role/${roleId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data.payload;
  } catch (error) {
    console.error("Lỗi khi lấy chi tiết:", error);
    throw error;
  }
};

export const createRole = async (roleData) => {
  try {
    const token = localStorage.getItem("accessToken");
    const decodedToken = jwtDecode(token);
    const performedBy = decodedToken.FullName || "Unknown User";

    const body = {
      performedBy,
      name: roleData.name,
      description: roleData.description,
      roleCode: roleData.roleCode,
      privilegeID: roleData.privilegeID,
    };

    const response = await api.post("/hcm/iam/role", body, {
      headers: { Authorization: `Bearer ${token}` },
    });

    return response.data.payload;
  } catch (error) {
    console.error("Lỗi khi tạo role:", error);
    throw error;
  }
};

export const updateRole = async (roleId, roleData) => {
  try {
    const token = localStorage.getItem("accessToken");
    const decodedToken = jwtDecode(token);
    const performedBy = decodedToken.FullName || "Unknown User";

    const body = {
      performedBy,
      name: roleData.name,
      description: roleData.description,
      privilegeID: roleData.privilegeID,
    };

    const response = await api.put(`/hcm/iam/role/${roleId}`, body, {
      headers: { Authorization: `Bearer ${token}` },
    });

    return response.data.payload;
  } catch (error) {
    throw error;
  }
};

export const deleteRole = async (roleId) => {
  try {
    const token = localStorage.getItem("accessToken");
    const decodedToken = jwtDecode(token);
    const performedBy = decodedToken.FullName || "Unknown User";

    const response = await api.delete(`/hcm/iam/role/${roleId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      data: {
        performedBy,
      },
    });

    return response.data.payload;
  } catch (error) {
    console.error("Lỗi khi xóa role:", error);
    throw error;
  }
};
