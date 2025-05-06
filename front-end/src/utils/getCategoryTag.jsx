import { Tag } from 'antd';
import { colorPalette } from '~/constants/colorPalette';

export const getCategoryTag = (categoryName, categories) => {
  const category = categories.find((cat) => cat.name === categoryName);
  const color = category
    ? colorPalette[category.id % colorPalette.length]
    : 'default';
  return <Tag color={color}>{category?.name}</Tag>;
};
