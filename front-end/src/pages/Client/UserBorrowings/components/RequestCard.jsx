import { Card, List, Typography, Divider, Button, Badge } from 'antd';
import { CalendarOutlined, EyeOutlined } from '@ant-design/icons';
import { getStatusTag } from '~/utils/getStatusTag';
import { getCategoryTag } from '~/utils/getCategoryTag';
import { formatDate } from '~/utils/formatDate';
import { requestStatus } from '~/constants/requestStatus';

const { Text } = Typography;

export default function RequestCard({ request, categories, onViewDetails }) {
  const statusColor = {
    [requestStatus.APPROVED]: 'success',
    [requestStatus.WAITING]: 'processing',
    [requestStatus.REJECTED]: 'error',
    partial: 'warning',
  };

  return (
    <Card className="mb-4 shadow-sm hover:shadow-md transition-shadow">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-4">
        <div>
          <div className="flex items-center">
            <Badge status={statusColor[request.status] || 'default'} />
            <Text strong className="text-lg ml-2">
              Request
            </Text>
            <div className="ml-2">{getStatusTag(request.status)}</div>
          </div>
          <div className="mt-1 text-gray-500 flex items-center">
            <CalendarOutlined className="mr-1" /> Requested:
            <span className="mx-2">{formatDate(request.dateRequested)}</span>
          </div>
        </div>
        <div className="mt-2 md:mt-0 flex gap-2">
          <Button
            type="primary"
            icon={<EyeOutlined />}
            onClick={() => onViewDetails(request)}
            className="bg-blue-600 hover:bg-blue-700"
          >
            Details
          </Button>
        </div>
      </div>

      <Divider className="my-3" />

      <div>
        <div className="flex justify-between mb-2">
          <Text strong>
            Books in this request ({request.bookRequested.length}):
          </Text>
          <Text type="secondary">{request.status}</Text>
        </div>

        <List
          size="small"
          dataSource={request.bookRequested}
          renderItem={(book) => (
            <List.Item className="py-2">
              <div className="flex flex-col sm:flex-row sm:items-center w-full">
                <div className="flex-1">
                  <div className="font-medium">{book.title}</div>
                  <div className="text-gray-500 text-sm">
                    {book.author} • ISBN: {book.isbn}
                  </div>
                </div>
                <div className="mt-1 sm:mt-0 flex items-center">
                  {getCategoryTag(book.category, categories)}
                </div>
              </div>
            </List.Item>
          )}
        />
      </div>
    </Card>
  );
}
