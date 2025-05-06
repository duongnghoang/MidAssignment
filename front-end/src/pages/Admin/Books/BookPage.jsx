import { useState, useEffect } from 'react';
import { BookOutlined, PlusOutlined } from '@ant-design/icons';
import PageTitle from '~/components/Layout/PageTitle';
import BookFilters from './components/BookFilters';
import BookTable from './components/BookTable';
import BookFormModal from './components/BookFormModal';
import { getAllCategories } from '~/services/category.service';
import {
  getListBooksFilter,
  addNewBook,
  updateBook,
} from '~/services/book.service';
import { toast } from 'react-toastify';
import { Button } from 'antd';

export default function BookPage() {
  const defaultFilters = {
    searchString: '',
    categoryId: null,
    pageSize: 10,
    pageIndex: 1,
  };

  const [filters, setFilters] = useState(defaultFilters);
  const [categories, setCategories] = useState([]);
  const [books, setBooks] = useState([]);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [currentBook, setCurrentBook] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const categories = await getAllCategories();
        setCategories(categories);
      } catch (error) {
        toast.error('Failed to fetch categories');
      }
    };
    fetchCategories();
  }, []);

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const bookResponse = await getListBooksFilter(filters);
        setBooks(bookResponse.items);
        setPagination((prev) => ({
          ...prev,
          current: bookResponse.pageIndex,
          pageSize: bookResponse.pageSize,
          total: bookResponse.totalCount,
        }));
      } catch (error) {
        toast.error('Failed to fetch books');
      }
    };
    fetchBooks();
  }, [filters]);

  const handleAddBook = () => {
    setCurrentBook(null);
    setIsModalOpen(true);
  };

  const handleEditBook = (book) => {
    setCurrentBook(book);
    setIsModalOpen(true);
  };

  const handleModalOk = async (values) => {
    try {
      const formattedValues = {
        ...values,
        publicationDate: values.publicationDate.format('YYYY-MM-DD'),
      };
      if (currentBook) {
        await updateBook(currentBook.id, formattedValues);
        toast.success('Book updated successfully');
        setFilters((prev) => ({ ...prev }));
      } else {
        await addNewBook(formattedValues);
        toast.success('Book added successfully');
        setFilters((prev) => ({ ...prev, pageIndex: 1 }));
      }
      setIsModalOpen(false);
    } catch (error) {
      toast.error(error.message || 'An error occurred while adding the book');
    }
  };

  return (
    <>
      <PageTitle
        title="Book Management"
        icon={<BookOutlined className="mr-2" />}
        subtitle="Manage your library books"
        extraContent={
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={handleAddBook}
          >
            Add Book
          </Button>
        }
      />
      <BookFilters
        filters={filters}
        setFilters={setFilters}
        categories={categories}
        defaultFilters={defaultFilters}
      />
      <BookTable
        books={books}
        categories={categories}
        pagination={pagination}
        onEditBook={handleEditBook}
        setFilters={setFilters}
      />
      <BookFormModal
        isOpen={isModalOpen}
        currentBook={currentBook}
        categories={categories}
        onOk={handleModalOk}
        onCancel={() => {
          setCurrentBook(null);
          setIsModalOpen(false);
        }}
      />
    </>
  );
}
