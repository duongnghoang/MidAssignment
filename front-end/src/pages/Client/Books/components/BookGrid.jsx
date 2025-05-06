import { Empty } from 'antd';
import Loading from '~/components/Loading';
import BookCard from './BookCard';

export default function BookGrid({
  isLoading,
  books,
  categories,
  isSelected,
  onAddToSelection,
  onRemoveFromSelection,
}) {
  if (isLoading) return <Loading />;
  if (books.length === 0)
    return <Empty description="No books found matching your criteria" />;

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
      {books.map((book) => (
        <BookCard
          key={book.id}
          book={book}
          categories={categories}
          isSelected={isSelected(book)}
          onAddToSelection={() => onAddToSelection(book)}
          onRemoveFromSelection={() => onRemoveFromSelection(book.id)}
        />
      ))}
    </div>
  );
}
