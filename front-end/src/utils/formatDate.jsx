import { Tooltip } from 'antd';
import dayjs from 'dayjs';

export const formatDate = (dateString) => {
  const date = dayjs(dateString);
  return (
    <Tooltip title={date.format('YYYY-MM-DD HH:mm')}>
      <span>{date.format('MMM D, YYYY')}</span>
    </Tooltip>
  );
};
