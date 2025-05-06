import axiosInstance from '~/config/axiosInstance';

export const getListBookRequestFilters = async (filters) => {
  const response = await axiosInstance.get(`/bookBorrowingRequests/`, {
    params: {
      ...filters,
    },
  });
  return response.data.value;
};

export const getUserListBookRequest = async (userId) => {
  const response = await axiosInstance.get(
    `/bookBorrowingRequests/requestor/${userId}`
  );
  return response.data.value;
};

export const addNewBookRequest = async (bookRequest) => {
  const response = await axiosInstance.post(
    `/bookBorrowingRequests/`,
    bookRequest
  );
  return response.data;
};

export const getUserRequestInMonth = async (userId) => {
  const response = await axiosInstance.get(
    `/bookBorrowingRequests/month-request`,
    {
      params: {
        requestorId: userId,
      },
    }
  );
  return response.data;
};

export const updateRequestStatus = async (id, status, approverId) => {
  const response = await axiosInstance.put(
    `/bookBorrowingRequests/${id}/update-status`,
    {
      status,
      approverId,
    }
  );
  return response;
};
