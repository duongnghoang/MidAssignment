import dayjs from 'dayjs';
import FormModal from '~/components/FormModal';
import { requiredRule } from '~/constants/validation';

export default function BookFormModal({
  isOpen,
  currentBook,
  categories,
  onOk,
  onCancel,
}) {
  const getInitialFormValues = currentBook
    ? {
        ...currentBook,
        publicationDate: currentBook.publicationDate
          ? dayjs(currentBook.publicationDate)
          : null,
      }
    : {};

  const bookFields = [
    {
      name: 'id',
      label: 'ID',
      type: currentBook ? 'number' : 'hidden',
      disabled: true,
    },
    {
      name: 'title',
      label: 'Title',
      type: 'text',
      rules: [requiredRule('title')],
    },
    {
      name: 'author',
      label: 'Author',
      type: 'text',
      rules: [requiredRule('author')],
    },
    {
      name: 'isbn',
      label: 'ISBN',
      type: 'text',
      rules: [requiredRule('ISBN')],
    },
    {
      name: 'categoryId',
      label: 'Category',
      type: 'select',
      rules: [requiredRule('category')],
      options: categories.map((category) => ({
        id: category.id,
        name: category.name,
      })),
    },
    {
      name: 'publicationDate',
      label: 'Publication Date',
      type: 'date',
      rules: [requiredRule('publication date')],
    },
    {
      name: 'quantity',
      label: 'Quantity',
      type: 'number',
      rules: [requiredRule('quantity')],
      inputProps: { min: 0 },
    },
  ];

  return (
    <FormModal
      title={currentBook ? 'Edit Book' : 'Add New Book'}
      fields={bookFields}
      initialValues={getInitialFormValues}
      isOpen={isOpen}
      onOk={onOk}
      onCancel={onCancel}
    />
  );
}
