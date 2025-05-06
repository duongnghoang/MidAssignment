import { Drawer, Button, Form, List, Empty, Alert } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';
import { getCategoryTag } from '~/utils/getCategoryTag';

export default function RequestFormDrawer({
  open,
  onClose,
  selectedBooks,
  categories,
  requestsThisMonth,
  onRemoveFromSelection,
  onClearSelection,
  onSubmit,
}) {
  const [form] = Form.useForm();

  return (
    <Drawer
      title={
        <div className="flex justify-between items-center">
          <span>Selected Books ({selectedBooks.length}/5)</span>
          {selectedBooks.length > 0 && (
            <Button size="small" danger onClick={onClearSelection}>
              Clear All
            </Button>
          )}
        </div>
      }
      placement="right"
      onClose={onClose}
      open={open}
      width={400}
      footer={
        selectedBooks.length > 0 && (
          <div className="flex justify-end">
            <Button
              type="primary"
              onClick={() => form.submit()}
              disabled={requestsThisMonth >= 3}
              className="bg-blue-600 hover:bg-blue-700"
            >
              Submit Request
            </Button>
          </div>
        )
      }
    >
      {selectedBooks.length > 0 ? (
        <Form form={form} layout="vertical" onFinish={onSubmit}>
          <List
            itemLayout="horizontal"
            dataSource={selectedBooks}
            renderItem={(book) => (
              <List.Item
                actions={[
                  <Button
                    key="delete"
                    type="text"
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => onRemoveFromSelection(book.id)}
                  />,
                ]}
              >
                <List.Item.Meta
                  title={book.title}
                  description={
                    <div>
                      <div>{book.author}</div>
                      {getCategoryTag(book.category, categories)}
                    </div>
                  }
                />
              </List.Item>
            )}
          />
        </Form>
      ) : (
        <Empty description="No books selected" />
      )}

      {requestsThisMonth >= 3 && (
        <Alert
          message="Monthly limit reached"
          description="You have used all 3 borrow requests for this month."
          type="warning"
          showIcon
          className="mt-4"
        />
      )}
    </Drawer>
  );
}
