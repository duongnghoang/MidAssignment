import { Card, Tag, Tooltip } from 'antd';
import Meta from 'antd/es/card/Meta';
import {
  CloseCircleOutlined,
  InfoCircleOutlined,
  MinusOutlined,
  PlusOutlined,
} from '@ant-design/icons';
import { colorPalette } from '~/constants/colorPalette';
import { getCategoryTag } from '~/utils/getCategoryTag';

export default function BookCard({
  book,
  categories,
  isSelected,
  onAddToSelection,
  onRemoveFromSelection,
}) {
  const isAvailable = book.available > 0;

  return (
    <Card
      hoverable
      className="h-full flex flex-col"
      actions={[
        isAvailable && !isSelected ? (
          <Tooltip key="add" title="Add to Selection">
            <PlusOutlined onClick={onAddToSelection} />
          </Tooltip>
        ) : isSelected ? (
          <Tooltip key="remove" title="Remove from Selection">
            <MinusOutlined onClick={onRemoveFromSelection} />
          </Tooltip>
        ) : (
          <Tooltip key="unavailable" title="Not Available">
            <CloseCircleOutlined />
          </Tooltip>
        ),
      ]}
    >
      <Meta
        title={
          <Tooltip title={book.title}>
            <div className="truncate font-medium">{book.title}</div>
          </Tooltip>
        }
        description={
          <div className="flex flex-col">
            <div className="text-gray-500 truncate">{book.author}</div>
            <div className="mt-2 flex justify-between items-center">
              {getCategoryTag(book.category, categories)}
              <Tag color={isAvailable ? 'green' : 'red'}>
                {isAvailable ? 'Available' : 'Not Available'}
              </Tag>
            </div>
            <div className="mt-2 text-xs text-gray-500">
              Available: {book.available}/{book.quantity}
            </div>
          </div>
        }
      />
    </Card>
  );
}
