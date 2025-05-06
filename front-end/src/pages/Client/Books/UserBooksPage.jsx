import { useState, useEffect } from 'react';
import { BookOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { toast } from 'react-toastify';
import { useAuthContext } from '~/contexts/authContext';
import PageTitle from '~/components/Layout/PageTitle';
import CustomPagination from '~/components/CustomPagination';
import Loading from '~/components/Loading';
import BookCard from './components/BookCard';
import BookFilter from './components/BookFilter';
import { getCategoryTag } from '~/utils/getCategoryTag';
import RequestFormDrawer from './components/RequestFormDrawer';
import { getAllCategories } from '~/services/category.service';
import { getListBooksFilter } from '~/services/book.service';
import {
  addNewBookRequest,
  getUserRequestInMonth,
} from '~/services/bookBorrowRequest.service';
import BookGrid from './components/BookGrid';
import ConfirmModal from './components/ConfirmModal';
import MonthlyRequestIndicator from './components/MonthlyRequestIndicator';
import { Badge, Button } from 'antd';

const defaultFilters = {
  searchString: '',
  isAvailable: null,
  categoryId: null,
  pageSize: 12,
  pageIndex: 1,
};

export default function UserBooksPage() {
  const { userId } = useAuthContext();
  const [filters, setFilters] = useState(defaultFilters);
  const [categories, setCategories] = useState([]);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 12,
    total: 0,
  });
  const [books, setBooks] = useState([]);
  const [selectedBooks, setSelectedBooks] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [requestFormOpen, setRequestFormOpen] = useState(false);
  const [requestsThisMonth, setRequestsThisMonth] = useState(null);
  const [confirmModalVisible, setConfirmModalVisible] = useState(false);
  const [requestSubmitting, setRequestSubmitting] = useState(false);

  const fetchCategories = async () => {
    try {
      const categories = await getAllCategories();
      setCategories(categories);
    } catch (err) {
      toast.error('Failed to load categories');
    }
  };

  const fetchBooks = async () => {
    setIsLoading(true);
    try {
      const bookResponse = await getListBooksFilter(filters);
      setBooks(bookResponse.items);
      setPagination({
        current: bookResponse.pageIndex,
        pageSize: bookResponse.pageSize,
        total: bookResponse.totalCount,
      });
    } catch (err) {
      toast.error('Failed to load books');
    } finally {
      setIsLoading(false);
    }
  };

  const fetchUserMonthRequest = async () => {
    try {
      const response = await getUserRequestInMonth(userId);
      setRequestsThisMonth(response.value);
    } catch (err) {
      toast.error('Failed to load monthly requests');
    }
  };

  useEffect(() => {
    fetchCategories();
    fetchUserMonthRequest();
  }, [userId]);

  useEffect(() => {
    fetchBooks();
  }, [filters]);

  const handleFilterChange = (key, value) => {
    setFilters((prev) => ({
      ...prev,
      [key]: value,
      pageIndex: 1,
    }));
  };

  const handlePaginationChange = (page, pageSize) => {
    setFilters((prev) => ({
      ...prev,
      pageIndex: page,
      pageSize,
    }));
  };

  const handleAddToSelection = (book) => {
    if (selectedBooks.length >= 5) {
      message.warning('You can only select up to 5 books per request.');
      return;
    }
    if (selectedBooks.some((selectedBook) => selectedBook.id === book.id)) {
      message.warning('This book is already in your selection.');
      return;
    }
    setSelectedBooks((prev) => [...prev, book]);
  };

  const handleRemoveFromSelection = (bookId) => {
    setSelectedBooks((prev) => prev.filter((book) => book.id !== bookId));
  };

  const handleClearSelection = () => {
    setSelectedBooks([]);
  };

  const handleRequestSubmit = () => {
    setConfirmModalVisible(true);
  };

  const handleConfirmRequest = async () => {
    setRequestSubmitting(true);
    try {
      const request = {
        requestorId: userId,
        requestDetails: selectedBooks.map((book) => ({
          bookId: book.id,
        })),
      };
      const response = await addNewBookRequest(request);
      if (response.isSuccess) {
        setSelectedBooks([]);
        setRequestsThisMonth((prev) => (prev || 0) + 1);
        toast.success('Your borrow request has been submitted successfully!');
        setRequestFormOpen(false);
        setConfirmModalVisible(false);
        await fetchBooks();
      } else {
        throw new Error(response.error);
      }
    } catch (error) {
      toast.error(
        error.message || 'Failed to submit request. Please try again later.'
      );
    } finally {
      setRequestSubmitting(false);
    }
  };

  const isSelected = (book) => {
    return selectedBooks.some((selectedBook) => selectedBook.id === book.id);
  };

  return (
    <>
      <PageTitle
        title="Browse Books"
        icon={<BookOutlined className="mr-2" />}
        subtitle="Find and borrow books from our library"
        extraContent={
          <Badge count={selectedBooks.length} overflowCount={5}>
            <Button
              icon={<ShoppingCartOutlined />}
              onClick={() => setRequestFormOpen(true)}
              className={
                selectedBooks.length > 0 ? 'border-blue-500 text-blue-500' : ''
              }
            >
              Selected Books
            </Button>
          </Badge>
        }
      />

      <MonthlyRequestIndicator requestsThisMonth={requestsThisMonth} />

      <BookFilter
        categories={categories}
        filters={filters}
        setFilters={setFilters}
      />

      <BookGrid
        isLoading={isLoading}
        books={books}
        categories={categories}
        isSelected={isSelected}
        onAddToSelection={handleAddToSelection}
        onRemoveFromSelection={handleRemoveFromSelection}
      />

      {books.length > 0 && (
        <CustomPagination
          pagination={pagination}
          onPaginationChange={handlePaginationChange}
          align="center"
          pageSizeOptions={['12', '24', '36']}
        />
      )}

      <RequestFormDrawer
        open={requestFormOpen}
        onClose={() => setRequestFormOpen(false)}
        selectedBooks={selectedBooks}
        categories={categories}
        requestsThisMonth={requestsThisMonth}
        onRemoveFromSelection={handleRemoveFromSelection}
        onClearSelection={handleClearSelection}
        onSubmit={handleRequestSubmit}
      />

      <ConfirmModal
        visible={confirmModalVisible}
        onConfirm={handleConfirmRequest}
        onCancel={() => setConfirmModalVisible(false)}
        selectedBooks={selectedBooks}
        isSubmitting={requestSubmitting}
      />
    </>
  );
}
