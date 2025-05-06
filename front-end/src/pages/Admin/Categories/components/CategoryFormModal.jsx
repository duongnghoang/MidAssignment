import { Modal, Form, Input } from 'antd';

export default function CategoryFormModal({
  isOpen,
  currentCategory,
  onOk,
  onCancel,
}) {
  const [form] = Form.useForm();

  const handleOk = async () => {
    try {
      const values = await form.validateFields();
      await onOk(values);
      form.resetFields();
    } catch (error) {
      console.error('Validation failed:', error);
    }
  };

  return (
    <Modal
      title={currentCategory ? 'Edit Category' : 'Add New Category'}
      open={isOpen}
      onOk={handleOk}
      onCancel={() => {
        form.resetFields();
        onCancel();
      }}
    >
      <Form form={form} layout="vertical" initialValues={currentCategory || {}}>
        <Form.Item name="id" label="ID" hidden={!currentCategory}>
          <Input disabled />
        </Form.Item>
        <Form.Item
          name="name"
          label="Name"
          rules={[
            { required: true, message: 'Please enter the category name' },
          ]}
        >
          <Input placeholder="Enter category name" autoFocus />
        </Form.Item>
      </Form>
    </Modal>
  );
}
