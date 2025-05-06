import { Pagination } from 'antd';

export default function CustomPagination({
  pagination,
  onPaginationChange,
  pageSizeOptions = ['5', '10', '20', '50'],
  align = 'end',
}) {
  return (
    <Pagination
      align={align}
      current={pagination.current}
      pageSize={pagination.pageSize}
      total={pagination.total}
      showTotal={(total, range) =>
        `Showing ${range[0]}-${range[1]} of ${total} items`
      }
      showSizeChanger
      pageSizeOptions={pageSizeOptions}
      onChange={onPaginationChange}
      showLessItems
      responsive
    />
  );
}
