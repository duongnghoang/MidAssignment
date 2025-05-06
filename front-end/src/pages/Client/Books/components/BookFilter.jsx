import { Input, Select } from 'antd';
import { SearchOutlined } from '@ant-design/icons';

const { Option } = Select;

export default function BookFilter({ filters, setFilters, categories }) {
  const handleFilterChange = (key, value) => {
    setFilters((prev) => ({
      ...prev,
      [key]: value,
      pageIndex: 1,
    }));
    console.log(filters);
  };

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <Input
        placeholder="Search by title, author, or ISBN"
        prefix={<SearchOutlined className="text-gray-400" />}
        value={filters.searchString}
        onChange={(e) => handleFilterChange('searchString', e.target.value)}
        allowClear
      />
      <Select
        placeholder="Filter by Category"
        className="w-full"
        allowClear
        value={filters.categoryId}
        onChange={(value) => handleFilterChange('categoryId', value)}
      >
        {categories.map((category) => (
          <Option key={category.id} value={category.id}>
            {category.name}
          </Option>
        ))}
      </Select>
      <Select
        placeholder="Filter by Status"
        className="w-full"
        value={filters.isAvailable}
        onChange={(value) => handleFilterChange('isAvailable', value)}
      >
        <Option value={null}>All</Option>
        <Option value={true}>Available</Option>
      </Select>
    </div>
  );
}
