import { Alert, Progress, Tooltip } from 'antd';
import { InfoCircleOutlined } from '@ant-design/icons';

export default function MonthlyRequestIndicator({ requestsThisMonth }) {
  if (requestsThisMonth === null) return null;

  return (
    <div className="mb-6">
      <div className="flex justify-between items-center mb-2">
        <span className="font-medium">Monthly Borrow Requests</span>
        <span className="text-sm">
          {requestsThisMonth} of 3 used{' '}
          <Tooltip title="You can make up to 3 borrow requests per month, with up to 5 books per request.">
            <InfoCircleOutlined />
          </Tooltip>
        </span>
      </div>
      <Progress
        percent={(requestsThisMonth / 3) * 100}
        showInfo={false}
        status={requestsThisMonth >= 3 ? 'exception' : 'active'}
        strokeColor={
          requestsThisMonth === 1
            ? '#52c41a'
            : requestsThisMonth === 2
              ? '#faad14'
              : requestsThisMonth >= 3
                ? '#f5222d'
                : undefined
        }
      />
      {requestsThisMonth >= 3 && (
        <Alert
          message="Monthly limit reached"
          description="You have used all 3 borrow requests for this month. New requests will be available next month."
          type="warning"
          showIcon
          className="mt-2"
        />
      )}
    </div>
  );
}
