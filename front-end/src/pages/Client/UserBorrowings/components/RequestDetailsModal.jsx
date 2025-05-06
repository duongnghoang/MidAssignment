import { Modal, Typography, Divider, Alert, Button } from 'antd';
import { FileTextOutlined, UserOutlined } from '@ant-design/icons';
import { getStatusTag } from '~/utils/getStatusTag';
import RequestTimeline from './RequestTimeline';
import RequestBookList from './RequestBookList';
import { requestStatus } from '~/constants/requestStatus';
import { formatDate } from '~/utils/formatDate';

const { Text } = Typography;

export default function RequestDetailsModal({
  visible,
  request,
  categories,
  onClose,
}) {
  if (!request) return null;

  return (
    <Modal
      title={
        <div className="flex items-center">
          <FileTextOutlined className="mr-2" /> Request Details
        </div>
      }
      open={visible}
      onCancel={onClose}
      footer={[
        <Button key="close" type="primary" onClick={onClose}>
          Close
        </Button>,
      ]}
      width={700}
    >
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
        <div>
          <Text type="secondary">Request ID</Text>
          <div className="font-medium">{request.id}</div>
        </div>
        <div>
          <Text type="secondary">Status</Text>
          <div>{getStatusTag(request.status)}</div>
        </div>
        <div>
          <Text type="secondary">Requested By</Text>
          <div className="font-medium flex items-center">
            <UserOutlined className="mr-1" /> {request.requestor}
          </div>
        </div>
        <div>
          <Text type="secondary">Date Requested</Text>
          <div className="font-medium">{formatDate(request.dateRequested)}</div>
        </div>
        <div>
          <Text type="secondary">Books Requested</Text>
          <div className="font-medium">{request.bookRequested.length}</div>
        </div>
        <div>
          <Text type="secondary">Approver</Text>
          <div className="font-medium">
            {request.approver || <Text type="secondary">Not yet approved</Text>}
          </div>
        </div>
      </div>

      <Divider orientation="left">Request Timeline</Divider>
      <RequestTimeline request={request} />

      <Divider orientation="left">Books</Divider>
      <RequestBookList books={request.bookRequested} categories={categories} />

      {request.status === requestStatus.APPROVED && (
        <Alert
          message="Ready for Pickup"
          description="Your books are ready for pickup at the library. Please bring your ID card when you visit."
          type="success"
          showIcon
          className="mt-4"
        />
      )}
    </Modal>
  );
}
