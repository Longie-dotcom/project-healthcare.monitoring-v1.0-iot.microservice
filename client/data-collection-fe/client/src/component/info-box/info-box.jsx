import React, { useEffect } from "react";
import "./info-box.css";

function InfoBox({ title, message, type = "info", onClose, duration = 5000 }) {
  // Auto-close after `duration` milliseconds
  useEffect(() => {
    const timer = setTimeout(() => {
      onClose?.();
    }, duration);

    return () => clearTimeout(timer);
  }, [duration, onClose]);

  return (
    <div className={`info-box ${type}`}>
      <div className="info-box-header">
        <span className="info-box-title">{title}</span>
        <button className="info-box-close" onClick={onClose}>×</button>
      </div>
      <div className="info-box-body">
        {message}
      </div>
    </div>
  );
}

export default InfoBox;
