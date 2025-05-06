import { Timeline } from 'antd';
import {
  BookOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
  CloseCircleOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { requestStatus } from '~/constants/requestStatus';
import { formatDate } from '~/utils/formatDate';

export default function RequestTimeline({ request }) {
  const getStatusDot = (status) => {
    switch (status) {
      case requestStatus.APPROVED:
        return <CheckCircleOutlined style={{ fontSize: '16px' }} />;
      case requestStatus.WAITING:
        return <ClockCircleOutlined style={{ fontSize: '16px' }} />;
      case requestStatus.REJECTED:
        return <CloseCircleOutlined style={{ fontSize: '16px' }} />;
      default:
        return null;
    }
  };

  const getStatusColor = (status) => {
    switch (status) {
      case requestStatus.APPROVED:
        return 'green';
      case requestStatus.WAITING:
        return 'blue';
      case requestStatus.REJECTED:
        return 'red';
      default:
        return 'grey';
    }
  };

  return (
    <Timeline
      mode="left"
      items={[
        {
          label: formatDate(request.dateRequested),
          children: 'Request submitted',
          dot: <BookOutlined style={{ fontSize: '16px' }} />,
          color: 'blue',
        },
        {
          label: 'Status Update',
          children: request.status,
          dot: getStatusDot(request.status),
          color: getStatusColor(request.status),
        },
        ...(request.status === requestStatus.APPROVED
          ? [
              {
                label: 'Expected',
                children: 'Ready for pickup',
                dot: <CheckCircleOutlined style={{ fontSize: '16px' }} />,
                color: 'green',
              },
            ]
          : []),
      ]}
    />
  );
}
