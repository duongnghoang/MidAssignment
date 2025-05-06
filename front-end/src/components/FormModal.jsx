import { Button, Form, Input, Modal, Select, DatePicker } from 'antd';
import { useEffect } from 'react';

export default function FormModal({
  title,
  fields,
  initialValues,
  isOpen,
  onOk,
  onCancel,
}) {
  const [formInstance] = Form.useForm();

  useEffect(() => {
    if (isOpen) {
      formInstance.setFieldsValue(initialValues);
    } else {
      formInstance.resetFields();
    }
  }, [isOpen, initialValues, formInstance]);

  const handleOk = async () => {
    try {
      const values = await formInstance.validateFields();
      await onOk(values);
    } catch (error) {
      return Promise.reject(error);
    }
  };

  // Map field types to corresponding Ant Design components
  const renderField = (field) => {
    switch (field.type) {
      case 'select':
        return (
          <Select placeholder={field.placeholder} {...field.inputProps}>
            {field.options?.map((option) => (
              <Select.Option key={option.id} value={option.id}>
                {option.name}
              </Select.Option>
            ))}
          </Select>
        );
      case 'date':
        return <DatePicker style={{ width: '100%' }} {...field.inputProps} />;
      case 'number':
        return (
          <Input
            type="number"
            placeholder={field.placeholder}
            disabled={field.disabled}
            {...field.inputProps}
          />
        );
      default:
        return <Input placeholder={field.placeholder} {...field.inputProps} />;
    }
  };

  return (
    <Modal
      title={title}
      open={isOpen}
      onOk={handleOk}
      onCancel={onCancel}
      width={500}
      footer={[
        <Button key="cancel" onClick={onCancel}>
          Cancel
        </Button>,
        <Button key="submit" type="primary" onClick={handleOk}>
          Save
        </Button>,
      ]}
      centered
      destroyOnClose
    >
      <Form form={formInstance} layout="vertical">
        {fields.map(
          (field) =>
            field.type !== 'hidden' && (
              <Form.Item
                key={field.name}
                name={field.name}
                label={field.label}
                rules={field.rules}
                {...field.extraProps}
              >
                {renderField(field)}
              </Form.Item>
            )
        )}
      </Form>
    </Modal>
  );
}
