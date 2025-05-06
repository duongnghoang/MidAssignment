import { Input, Select, Button } from 'antd';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons';
import { requestStatus } from '~/constants/requestStatus';

export default function BookBorrowingRequestFilters({
  filters,
  setFilters,
  defaultFilters,
}) {
  const handleSearchChange = (e) => {
    setFilters((prev) => ({
      ...prev,
      searchRequestor: e.target.value,
      pageIndex: 1,
    }));
  };

  const handleStatusChange = (value) => {
    setFilters((prev) => ({
      ...prev,
      searchStatus: value || null,
      pageIndex: 1,
    }));
  };

  const handleResetFilters = () => {
    setFilters(defaultFilters);
  };

  return (
    <div className="mb-6 flex space-x-4">
      <Input
        placeholder="Search by requestor"
        prefix={<SearchOutlined className="text-gray-400" />}
        value={filters.searchRequestor}
        onChange={handleSearchChange}
        allowClear
        className="max-w-md"
      />
      <Select
        placeholder="Filter by status"
        value={filters.searchStatus}
        onChange={handleStatusChange}
        allowClear
        className="w-40"
        options={[
          { value: requestStatus.WAITING, label: requestStatus.WAITING },
          { value: requestStatus.APPROVED, label: requestStatus.APPROVED },
          { value: requestStatus.REJECTED, label: requestStatus.REJECTED },
        ]}
      />
      <Button
        icon={<ReloadOutlined />}
        onClick={handleResetFilters}
        className="text-gray-600"
      >
        Reset
      </Button>
    </div>
  );
}
