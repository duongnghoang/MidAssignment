import axiosInstance from '~/config/axiosInstance';

export const signIn = async (username, password) => {
  const response = await axiosInstance.post('/auth/sign-in', {
    username,
    password,
  });
  return response.data.value;
};

export const register = async (user) => {
  const response = await axiosInstance.post('/auth/sign-up', user);
  return response;
};
