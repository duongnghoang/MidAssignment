import { List, Modal } from 'antd';

export default function ConfirmModal({
  visible,
  onConfirm,
  onCancel,
  selectedBooks,
  isSubmitting,
}) {
  return (
    <Modal
      title="Confirm Borrow Request"
      open={visible}
      onOk={onConfirm}
      onCancel={onCancel}
      okText="Confirm Request"
      cancelText="Cancel"
      confirmLoading={isSubmitting}
    >
      <p>You are about to submit a borrow request for the following books:</p>
      <List
        size="small"
        bordered
        className="mt-2 mb-4"
        dataSource={selectedBooks}
        renderItem={(book) => (
          <List.Item>
            <div className="truncate">
              <span className="font-medium">{book.title}</span> - {book.author}
            </div>
          </List.Item>
        )}
      />
      <p>This will use 1 of your 3 monthly borrow requests.</p>
      <p>Are you sure you want to proceed with this request?</p>
    </Modal>
  );
}
