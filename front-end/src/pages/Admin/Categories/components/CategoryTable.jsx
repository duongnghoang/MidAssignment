import { Table, Button, Space, Tooltip, Popconfirm } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import CustomPagination from '~/components/CustomPagination';
import { toast } from 'react-toastify';
import { deleteCategory } from '~/services/category.service';

export default function CategoryTable({
  categories,
  pagination,
  onEditCategory,
  setFilters,
}) {
  const handleDeleteCategory = async (categoryId) => {
    try {
      await deleteCategory(categoryId);
      toast.success('Category deleted successfully!');
      setFilters((prev) => ({ ...prev }));
    } catch (error) {
      toast.error(error.response?.data?.error || 'Failed to delete category');
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
      width: 150,
    },
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
      render: (text) => <span className="font-medium">{text}</span>,
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 150,
      render: (_, record) => (
        <Space size="small">
          <Tooltip title="Edit Category">
            <Button
              icon={<EditOutlined />}
              size="small"
              onClick={() => onEditCategory(record)}
              className="text-green-600"
            />
          </Tooltip>
          <Tooltip title="Delete Category">
            <Popconfirm
              title="Are you sure you want to delete this category?"
              onConfirm={() => handleDeleteCategory(record.id)}
              okText="Yes"
              cancelText="No"
            >
              <Button icon={<DeleteOutlined />} size="small" danger />
            </Popconfirm>
          </Tooltip>
        </Space>
      ),
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={categories}
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
