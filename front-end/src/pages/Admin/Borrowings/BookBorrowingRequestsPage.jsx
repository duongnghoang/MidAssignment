import { useState, useEffect } from 'react';
import { FileTextOutlined, ReloadOutlined } from '@ant-design/icons';
import { Layout, Button } from 'antd';
import PageTitle from '~/components/Layout/PageTitle';
import BookBorrowingRequestFilters from './components/BookBorrowingRequestFilters';
import BookBorrowingRequestTable from './components/BookBorrowingRequestTable';
import { getListBookRequestFilters } from '~/services/bookBorrowRequest.service';
import { toast } from 'react-toastify';

export default function BookBorrowingRequestsPage() {
  const defaultFilters = {
    searchRequestor: '',
    searchStatus: null,
    pageSize: 10,
    pageIndex: 1,
  };

  const [filters, setFilters] = useState(defaultFilters);
  const [requests, setRequests] = useState([]);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  useEffect(() => {
    const fetchBookRequests = async () => {
      try {
        const response = await getListBookRequestFilters(filters);
        setRequests(response.items);
        setPagination((prev) => ({
          ...prev,
          current: response.pageIndex,
          pageSize: response.pageSize,
          total: response.totalCount,
        }));
      } catch (error) {
        toast.error('Failed to fetch book requests');
      }
    };
    fetchBookRequests();
  }, [filters]);

  return (
    <>
      <PageTitle
        title="Book Request Management"
        icon={<FileTextOutlined className="mr-2" />}
        subtitle="Manage user book requests"
        extraContent={
          <Button
            type="primary"
            icon={<ReloadOutlined />}
            onClick={() => setFilters(defaultFilters)}
          >
            Reset Filters
          </Button>
        }
      />
      <BookBorrowingRequestFilters
        filters={filters}
        setFilters={setFilters}
        defaultFilters={defaultFilters}
      />
      <BookBorrowingRequestTable
        requests={requests}
        pagination={pagination}
        setFilters={setFilters}
      />
    </>
  );
}
