import React, { useEffect } from "react";
import "./confirm-box.css";

function ConfirmBox({
  title = "Confirm Action",
  message = "Are you sure you want to continue?",
  onConfirm,
  onCancel,
  autoClose = false,
  duration = 5000,
}) {
  // Optional auto-close after duration
  useEffect(() => {
    if (!autoClose) return;
    const timer = setTimeout(() => {
      onCancel?.();
    }, duration);
    return () => clearTimeout(timer);
  }, [autoClose, duration, onCancel]);

  return (
    <div className="confirm-overlay">
      <div className="confirm-box">
        <div className="confirm-header">
          <span className="confirm-title">{title}</span>
          <button className="confirm-close" onClick={onCancel}>
            ×
          </button>
        </div>

        <div className="confirm-body">{message}</div>

        <div className="confirm-actions">
          <button className="confirm-btn confirm-yes" onClick={onConfirm}>
            Yes
          </button>
          <button className="confirm-btn confirm-no" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

export default ConfirmBox;
