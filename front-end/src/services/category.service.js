import { toast } from 'react-toastify';
import axiosInstance from '~/config/axiosInstance';

export const getAllCategories = async () => {
  const response = await axiosInstance.get(`/categories`);
  return response.data.value;
};

export const getListCategoriesFilter = async (filters) => {
  const response = await axiosInstance.get(`/categories/filter`, {
    params: {
      ...filters,
    },
  });
  return response.data.value;
};

export const addNewCategory = async (category) => {
  const response = await axiosInstance.post(`/categories`, category);
  return response;
};

export const updateCategory = async (category) => {
  const response = await axiosInstance.put(`/categories/${category.id}`, {
    name: category.name,
  });
  return response;
};

export const deleteCategory = async (id) => {
  const response = await axiosInstance.delete(`/categories/${id}`);
  return response;
};
