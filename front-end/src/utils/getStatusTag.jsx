import { Tag } from 'antd';
import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';
import { requestStatus } from '~/constants/requestStatus';

export const getStatusTag = (status) => {
  switch (status) {
    case requestStatus.APPROVED:
      return (
        <Tag icon={<CheckCircleOutlined />} color="success">
          Approved
        </Tag>
      );
    case requestStatus.WAITING:
      return (
        <Tag icon={<ClockCircleOutlined />} color="processing">
          Waiting
        </Tag>
      );
    case requestStatus.REJECTED:
      return (
        <Tag icon={<CloseCircleOutlined />} color="error">
          Rejected
        </Tag>
      );
    case 'partial':
      return <Tag color="warning">Partially Approved</Tag>;
    default:
      return null;
  }
};
