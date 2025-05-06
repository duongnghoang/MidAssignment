import { useState, useEffect } from 'react';
import { Empty, Typography } from 'antd';
import { BookOutlined, CalendarOutlined } from '@ant-design/icons';
import { useAuthContext } from '~/contexts/authContext';
import {
  getUserListBookRequest,
  getUserRequestInMonth,
} from '~/services/bookBorrowRequest.service';
import PageTitle from '~/components/Layout/PageTitle';
import MonthlyRequestIndicator from '../Books/components/MonthlyRequestIndicator';
import RequestCard from './components/RequestCard';
import RequestDetailsModal from './components/RequestDetailsModal';
import dayjs from 'dayjs';
import { getAllCategories } from '~/services/category.service';
import Loading from '~/components/Loading';
import { toast } from 'react-toastify';

const { Text } = Typography;

export default function UserBorrowingsPage() {
  const [loading, setLoading] = useState(true);
  const [requests, setRequests] = useState([]);
  const [requestThisMonths, setRequestsThisMonth] = useState(null);
  const [categories, setCategories] = useState([]);
  const { userId } = useAuthContext();
  const [selectedRequest, setSelectedRequest] = useState(null);
  const [detailModalVisible, setDetailModalVisible] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [requestsResponse, categoriesResponse] = await Promise.all([
        getUserListBookRequest(userId),
        getAllCategories(),
      ]);
      setRequests(requestsResponse);
      setCategories(categoriesResponse);
    } catch (error) {
      toast.error('Error fetching data:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserMonthRequest();
    fetchData();
  }, [userId]);

  const handleViewDetails = (request) => {
    setSelectedRequest(request);
    setDetailModalVisible(true);
  };

  const fetchUserMonthRequest = async () => {
    try {
      const response = await getUserRequestInMonth(userId);
      setRequestsThisMonth(response.value);
    } catch (err) {
      toast.error('Failed to load monthly requests');
    }
  };

  return (
    <>
      <PageTitle
        title="My Book Requests"
        icon={<BookOutlined className="mr-2" />}
        subtitle="Track and manage your book requests for this month"
        extraContent={
          <div className="flex items-center">
            <CalendarOutlined className="mr-2" />
            <Text strong>Current Month: {dayjs().format('MMMM YYYY')}</Text>
          </div>
        }
      />

      <MonthlyRequestIndicator requestsThisMonth={requestThisMonths} />

      {loading ? (
        <Loading />
      ) : requests.length > 0 ? (
        <div className="space-y-4">
          {requests.map((request) => (
            <RequestCard
              key={request.id}
              request={request}
              categories={categories}
              onViewDetails={handleViewDetails}
            />
          ))}
        </div>
      ) : (
        <Empty
          description="You haven't made any book requests this month"
          image={Empty.PRESENTED_IMAGE_SIMPLE}
        />
      )}

      <RequestDetailsModal
        visible={detailModalVisible}
        request={selectedRequest}
        categories={categories}
        onClose={() => setDetailModalVisible(false)}
      />
    </>
  );
}
