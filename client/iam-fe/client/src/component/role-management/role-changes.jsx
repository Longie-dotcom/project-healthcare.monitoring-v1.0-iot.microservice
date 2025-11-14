import React, { useEffect, useState } from "react";
import { Modal, Form, Input, Checkbox, Button, message } from "antd";
import './role-changes.css';

function RoleChanges({ 
  visible, 
  onClose, 
  roleData, 
  isEdit, 
  setError, 
  setInfo, 
  getRolePrivileges, 
  createRole, 
  updateRole }) {
  const [form] = Form.useForm();
  const [privileges, setPrivileges] = useState([]);

  useEffect(() => {
    const fetchPrivileges = async () => {
      try {
        const data = await getRolePrivileges();
        setPrivileges(data);
      } catch {
        setError("Can not load privileges list");
      }
    };
    fetchPrivileges();
  }, []);

  useEffect(() => {
    if (roleData) {
      form.setFieldsValue({
        name: roleData.name,
        description: roleData.description,
        privilegeID: roleData.privilegeID,
      });
    } else {
      form.resetFields();
    }
  }, [roleData]);

  const handleSubmit = async (values) => {
    try {
      if (isEdit) {
        await updateRole(roleData.roleID, values);
        setInfo("Update role successfully");
      } else {
        await createRole(values);
        setInfo("Create role successfully");
      }
      onClose();
    } catch (err) {
      setError(err.response?.data?.message);
    }
  };

  return (
    <Modal
      title={isEdit ? "Update role information" : "Create new role"}
      open={visible}
      onCancel={onClose}
      footer={null}
    >
      <Form form={form} layout="vertical" onFinish={handleSubmit}>
        <Form.Item
          name="name"
          label="Role name"
        >
          <Input />
        </Form.Item>

        <Form.Item name="description" label="Description">
          <Input.TextArea rows={3} />
        </Form.Item>

        {!isEdit && (
          <Form.Item
            name="roleCode"
            label="Role code"
          >
            <Input />
          </Form.Item>
        )}

        <Form.Item
          name="privilegeID"
          label="Privileges list"
        >
          {privileges ? ( 
            <Checkbox.Group style={{ width: "100%" }}>
            {privileges.map((p) => (
              <div key={p.privilegeID} style={{ marginBottom: 4 }}>
                <Checkbox value={p.privilegeID}>
                  {p.name} — <small>{p.description}</small>
                </Checkbox>
              </div>
            ))}
          </Checkbox.Group>
          ) : (
            <div className="empty">
              The privilege list is empty
            </div>
          )}
        </Form.Item>

        <Button type="primary" htmlType="submit" block>
          {isEdit ? "Update" : "Create"}
        </Button>
      </Form>
    </Modal>
  );
}

export default RoleChanges;
