import { Input, Select, Tag, Button, Space } from 'antd';
import {
  SearchOutlined,
  FilterOutlined,
  ReloadOutlined,
} from '@ant-design/icons';

export default function BookFilters({
  filters,
  setFilters,
  categories,
  defaultFilters,
}) {
  const handleSearchChange = (event) => {
    setFilters((prev) => ({
      ...prev,
      searchString: event.target.value,
      pageIndex: 1,
    }));
  };

  const handleCategoryChange = (value) => {
    setFilters((prev) => ({
      ...prev,
      categoryId: value,
      pageIndex: 1,
    }));
  };

  const handleClearSearch = () => {
    setFilters((prev) => ({
      ...prev,
      searchString: '',
      pageIndex: 1,
    }));
  };

  const handleClearCategory = () => {
    setFilters((prev) => ({
      ...prev,
      categoryId: null,
      pageIndex: 1,
    }));
  };

  const resetFilters = () => {
    setFilters(defaultFilters);
  };

  const selectedCategoryName = categories.find(
    (category) => category.id === filters.categoryId
  )?.name;

  return (
    <div className="mb-6">
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
        <div className="col-span-1 md:col-span-2">
          <Input
            placeholder="Search by title, author, or ISBN"
            prefix={<SearchOutlined className="text-gray-400" />}
            value={filters.searchString}
            onChange={handleSearchChange}
            allowClear
          />
        </div>
        <div>
          <Select
            placeholder="Filter by Category"
            className="w-full"
            allowClear
            value={filters.categoryId}
            onChange={handleCategoryChange}
          >
            {categories.map((category) => (
              <Select.Option key={category.id} value={category.id}>
                {category.name}
              </Select.Option>
            ))}
          </Select>
        </div>
      </div>
      {(filters.searchString || filters.categoryId) && (
        <div className="flex items-center">
          <FilterOutlined className="mr-2 text-gray-500" />
          <div className="text-sm text-gray-500 mr-2">Filters:</div>
          <div className="flex flex-wrap gap-2">
            {filters.searchString && (
              <Tag closable onClose={handleClearSearch}>
                Search: {filters.searchString}
              </Tag>
            )}
            {filters.categoryId && (
              <Tag closable onClose={handleClearCategory}>
                Category: {selectedCategoryName}
              </Tag>
            )}
            <Button
              type="text"
              size="small"
              icon={<ReloadOutlined />}
              onClick={resetFilters}
            >
              Reset
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
