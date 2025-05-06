import { Table, Space, Button, Tooltip, Popconfirm } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import CustomPagination from '~/components/CustomPagination';
import { getCategoryTag } from '~/utils/getCategoryTag';
import { deleteBook } from '~/services/book.service';
import { toast } from 'react-toastify';

export default function BookTable({
  books,
  categories,
  pagination,
  onEditBook,
  setFilters,
}) {
  const handleDeleteBook = async (bookId) => {
    try {
      const response = await deleteBook(bookId);
      toast.success('Book deleted successfully!');
      setFilters((prev) => ({ ...prev }));
    } catch (error) {
      toast.error(error.response?.data?.error || 'Failed to delete book');
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
      title: 'Title',
      dataIndex: 'title',
      key: 'title',
      render: (text) => (
        <a className="text-blue-600 hover:text-blue-800 font-medium">{text}</a>
      ),
    },
    {
      title: 'Author',
      dataIndex: 'author',
      key: 'author',
    },
    {
      title: 'ISBN',
      dataIndex: 'isbn',
      key: 'isbn',
    },
    {
      title: 'Category',
      dataIndex: 'category',
      key: 'category',
      render: (tag) => getCategoryTag(tag, categories),
    },
    {
      title: 'Quantity',
      dataIndex: 'quantity',
      key: 'quantity',
      width: 80,
    },
    {
      title: 'Available',
      dataIndex: 'available',
      key: 'available',
      width: 80,
    },
    {
      title: 'Publication Date',
      dataIndex: 'publicationDate',
      key: 'publicationDate',
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 180,
      render: (_, record) => (
        <Space size="small">
          <Tooltip title="Edit Book">
            <Button
              icon={<EditOutlined />}
              size="small"
              onClick={() => onEditBook(record)}
              className="text-green-600"
            />
          </Tooltip>
          <Tooltip title="Delete Book">
            <Popconfirm
              title="Are you sure you want to delete this book?"
              onConfirm={() => handleDeleteBook(record.id)}
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
      dataSource={books}
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
