import { Table, Space, Button, Tooltip, Tag } from 'antd';
import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  EyeOutlined,
} from '@ant-design/icons';
import CustomPagination from '~/components/CustomPagination';
import { toast } from 'react-toastify';
import { getStatusTag } from '~/utils/getStatusTag';
import { updateRequestStatus } from '~/services/bookBorrowRequest.service';
import dayjs from 'dayjs';
import { requestStatus, requestStatusEnum } from '~/constants/requestStatus';
import { useAuthContext } from '~/contexts/authContext';

export default function BookBorrowingRequestTable({
  requests,
  pagination,
  setFilters,
}) {
  const { userId } = useAuthContext();

  const handleUpdateStatus = async (id, status) => {
    try {
      const response = await updateRequestStatus(id, status, userId);
      toast.success(`Book request ${status.toLowerCase()} successfully!`);
      setFilters((prev) => ({ ...prev }));
    } catch (error) {
      toast.error(
        error.response?.data?.error ||
          `Failed to ${status.toLowerCase()} book request`
      );
    }
  };

  const handlePaginationChange = (page, pageSize) => {
    setFilters((prev) => ({
      ...prev,
      pageIndex: page,
      pageSize,
    }));
  };

  const columns = [
    {
      title: 'ID',
      dataIndex: 'id',
      key: 'id',
      width: 80,
    },
    {
      title: 'Requestor',
      dataIndex: 'requestor',
      key: 'requestor',
      render: (text) => <span className="font-medium">{text}</span>,
    },
    {
      title: 'Approver',
      dataIndex: 'approver',
      key: 'approver',
      render: (text) => (
        <span className="font-medium">{text || 'Not Yet Approved'}</span>
      ),
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status) => getStatusTag(status),
    },
    {
      title: 'Date Requested',
      dataIndex: 'dateRequested',
      key: 'dateRequested',
      width: 150,
      render: (date) => dayjs(date).format('YYYY-MM-DD'),
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 150,
      render: (_, record) => {
        if (record.status === requestStatus.WAITING) {
          return (
            <Space size="small">
              <Tooltip title="Approve Request">
                <Button
                  type="primary"
                  size="small"
                  icon={<CheckCircleOutlined />}
                  onClick={() =>
                    handleUpdateStatus(record.id, requestStatusEnum.APPROVED)
                  }
                  className="bg-green-600 hover:bg-green-700"
                >
                  Approve
                </Button>
              </Tooltip>
              <Tooltip title="Reject Request">
                <Button
                  danger
                  size="small"
                  icon={<CloseCircleOutlined />}
                  onClick={() =>
                    handleUpdateStatus(record.id, requestStatusEnum.REJECTED)
                  }
                >
                  Reject
                </Button>
              </Tooltip>
              {/* <Tooltip title="View Details">
                <Button
                  size="small"
                  icon={<EyeOutlined />}
                  onClick={() => handleViewRequest(record)}
                >
                  View
                </Button>
              </Tooltip> */}
            </Space>
          );
        } else {
          // return (
          //   <Tooltip title="View Details">
          //     <Button
          //       size="small"
          //       icon={<EyeOutlined />}
          //       onClick={() => handleViewRequest(record)}
          //     >
          //       View
          //     </Button>
          //   </Tooltip>
          // );
        }
      },
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={requests}
      rowKey="id"
      pagination={false}
      className="shadow-sm overflow-auto bg-white rounded-lg"
      bordered
      footer={() => (
        <CustomPagination
          pagination={pagination}
          onPaginationChange={handlePaginationChange}
        />
      )}
    />
  );
}
