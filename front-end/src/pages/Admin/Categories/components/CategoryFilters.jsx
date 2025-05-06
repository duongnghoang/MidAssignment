import { Input, Button } from 'antd';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons';

export default function CategoryFilters({
  filters,
  setFilters,
  defaultFilters,
}) {
  const handleSearchChange = (e) => {
    setFilters((prev) => ({
      ...prev,
      name: e.target.value,
      pageIndex: 1,
    }));
  };

  const handleResetFilters = () => {
    setFilters(defaultFilters);
  };

  return (
    <div className="mb-6 flex space-x-4">
      <Input
        placeholder="Search categories"
        prefix={<SearchOutlined className="text-gray-400" />}
        value={filters.name}
        onChange={handleSearchChange}
        allowClear
        className="max-w-md"
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
