import { useState, useEffect } from 'react';
import { TagsOutlined, PlusOutlined } from '@ant-design/icons';
import PageTitle from '~/components/Layout/PageTitle';
import CategoryFilters from './components/CategoryFilters';
import CategoryTable from './components/CategoryTable';
import CategoryFormModal from './components/CategoryFormModal';
import {
  getListCategoriesFilter,
  addNewCategory,
  updateCategory,
} from '~/services/category.service';
import { toast } from 'react-toastify';
import { Button } from 'antd';

export default function CategoryPage() {
  const defaultFilters = {
    name: '',
    pageSize: 10,
    pageIndex: 1,
  };

  const [filters, setFilters] = useState(defaultFilters);
  const [categories, setCategories] = useState([]);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [currentCategory, setCurrentCategory] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await getListCategoriesFilter(filters);
        setCategories(response.items);
        setPagination((prev) => ({
          ...prev,
          current: response.pageIndex,
          pageSize: response.pageSize,
          total: response.totalCount,
        }));
      } catch (error) {
        toast.error('Failed to fetch categories');
      }
    };
    fetchCategories();
  }, [filters]);

  const handleAddCategory = () => {
    setCurrentCategory(null);
    setIsModalOpen(true);
  };

  const handleEditCategory = (category) => {
    setCurrentCategory(category);
    setIsModalOpen(true);
  };

  const handleModalOk = async (values) => {
    try {
      if (currentCategory) {
        await updateCategory(currentCategory.id, { name: values.name });
        toast.success('Category updated successfully!');
        setFilters((prev) => ({ ...prev }));
      } else {
        await addNewCategory({ name: values.name });
        toast.success('Category added successfully!');
        setFilters((prev) => ({ ...prev, pageIndex: 1 }));
      }
      setIsModalOpen(false);
      setCurrentCategory(null);
    } catch (error) {
      toast.error(
        error.response?.data?.error ||
          `Failed to ${currentCategory ? 'update' : 'add'} category`
      );
    }
  };

  return (
    <>
      <PageTitle
        title="Book Categories Management"
        icon={<TagsOutlined className="mr-2" />}
        subtitle="Manage your library book categories"
        extraContent={
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={handleAddCategory}
          >
            Add Category
          </Button>
        }
      />
      <CategoryFilters
        filters={filters}
        setFilters={setFilters}
        defaultFilters={defaultFilters}
      />
      <CategoryTable
        categories={categories}
        pagination={pagination}
        onEditCategory={handleEditCategory}
        setFilters={setFilters}
      />
      <CategoryFormModal
        isOpen={isModalOpen}
        currentCategory={currentCategory}
        onOk={handleModalOk}
        onCancel={() => {
          setCurrentCategory(null);
          setIsModalOpen(false);
        }}
      />
    </>
  );
}
