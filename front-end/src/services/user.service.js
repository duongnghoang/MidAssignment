import axiosInstance from '~/config/axiosInstance';

export const getUserById = async (id) => {
  const response = await axiosInstance.get(`/users/${id}`);
  return response.data.value;
};
