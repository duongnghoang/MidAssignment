import axiosInstance from '~/config/axiosInstance';

export const getListBooksFilter = async (filters) => {
  const response = await axiosInstance.get(`/books/`, {
    params: {
      ...filters,
    },
  });
  return response.data.value;
};

export const addNewBook = async (book) => {
  const response = await axiosInstance.post(`/books/`, book);
  return response;
};

export const updateBook = async (id, book) => {
  const response = await axiosInstance.put(`/books/${id}`, book);
  return response;
};

export const deleteBook = async (id) => {
  const response = await axiosInstance.delete(`/books/${id}`);
  return response;
};
