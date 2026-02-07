import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import * as signalR from "@microsoft/signalr";

export const useSensorHub = () => {
  const [sensorHistory, setSensorHistory] = useState({});
  const signalrUrl = import.meta.env.VITE_SIGNALR_URL;

  const getTokenAndUser = () => {
    const token = localStorage.getItem("accessToken");
    if (!token) return { token: null, identityNumber: null };

    const decoded = jwtDecode(token);
    const identityNumber =
      decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

    return { token, identityNumber };
  };

  useEffect(() => {
    const { token, identityNumber } = getTokenAndUser();
    if (!token || !identityNumber) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(signalrUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    const startConnection = async () => {
      try {
        await connection.start();
        console.log("✅ SignalR connected");

        await connection.invoke("JoinGroup", identityNumber);
        console.log(`📡 Joined group: ${identityNumber}`);
      } catch (err) {
        console.error("❌ SignalR connection failed:", err);
      }
    };

    connection.on("Receive", (payload) => {
      if (!payload?.patientIdentityNumber || !payload?.sensorKey || !payload?.dataType) return;

      const patientId = payload.patientIdentityNumber;
      const sensorKey = payload.sensorKey;
      const type = payload.dataType.toLowerCase();
      const value = Number(payload.dataValue);

      setSensorHistory(prev => {
        const updated = structuredClone(prev);
        console.log(updated);
        if (!updated[patientId]) {
          updated[patientId] = {
            // ✅ always update metadata if provided
            identityNumber: patientId,
            fullName: payload.fullName || "",
            roomNumber: payload.roomNumber || "",
            bedNumber: payload.bedNumber || "",
            age: payload.age || "",
            gender: payload.gender || "",

            sensors: {},
          };
        } else {
          // update metadata if new values arrive
          updated[patientId].fullName = payload.fullName || updated[patientId].fullName || "";
          updated[patientId].roomNumber = payload.roomNumber || updated[patientId].roomNumber || "";
          updated[patientId].bedNumber = payload.bedNumber || updated[patientId].bedNumber || "";
          updated[patientId].age = payload.age || updated[patientId].age || "";
          updated[patientId].gender = payload.gender || updated[patientId].gender || "";
        }

        // --- sensor update section ---
        if (!updated[patientId].sensors[type]) {
          updated[patientId].sensors[type] = [];
        }

        updated[patientId].sensors[type].push({
          timestamp: new Date().toISOString(),
          value,
          key: sensorKey
        });

        updated[patientId].sensors[type] =
          updated[patientId].sensors[type].slice(-50);

        return updated;
      });
    });


    startConnection();

    return () => connection.stop();
  }, []);

  return sensorHistory;
};
