import { List } from 'antd';
import { BookOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { getCategoryTag } from '~/utils/getCategoryTag';

export default function RequestBookList({ books, categories }) {
  return (
    <List
      itemLayout="horizontal"
      dataSource={books}
      renderItem={(book) => (
        <List.Item>
          <List.Item.Meta
            avatar={
              <div className="flex items-center justify-center w-10 h-10 bg-blue-100 rounded-full">
                <BookOutlined style={{ fontSize: '20px', color: '#1890ff' }} />
              </div>
            }
            title={
              <div className="flex justify-between">
                <span>{book.title}</span>
                {getCategoryTag(book.category, categories)}
              </div>
            }
            description={
              <div>
                <div>
                  <strong>Author:</strong> {book.author}
                </div>
                <div>
                  <strong>ISBN:</strong> {book.isbn}
                </div>
                <div>
                  <strong>Publication Date:</strong>{' '}
                  {dayjs(book.publicationDate).format('MMMM D, YYYY')}
                </div>
                <div>
                  <strong>Availability:</strong> {book.available} of{' '}
                  {book.quantity} copies available
                </div>
              </div>
            }
          />
        </List.Item>
      )}
    />
  );
}
