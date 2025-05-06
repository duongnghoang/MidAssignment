import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import UserLayout from '~/layouts/UserLayout';
import SignInPage from '~/pages/Admin/Auth/SignInPage';
import ProtectedRoute from './ProtectedRoute';
import BookPage from '~/pages/Admin/Books/BookPage';
import CategoryPage from '~/pages/Admin/Categories/CategoryPage';
import BookBorrowingRequestsPage from '~/pages/Admin/Borrowings/BookBorrowingRequestsPage';
import RegisterPage from '~/pages/Admin/Auth/RegisterPage';
import { userRoles } from '~/constants/role';
import UserBooksPage from '~/pages/Client/Books/UserBooksPage';
import UserBorrowingsPage from '~/pages/Client/UserBorrowings/UserBorrowingsPage';

export default function Routes() {
  let routes = [];

  const routeItems = [
    {
      element: <ProtectedRoute requiredRole={userRoles.SUPER_USER} />,
      children: [
        {
          element: <UserLayout />,
          children: [
            {
              path: '/books',
              element: <BookPage />,
            },
            {
              path: '/categories',
              element: <CategoryPage />,
            },
            {
              path: '/borrowing-requests',
              element: <BookBorrowingRequestsPage />,
            },
          ],
        },
      ],
    },
    {
      element: <ProtectedRoute requiredRole={userRoles.NORMAL_USER} />,
      children: [
        {
          element: <UserLayout />,
          children: [
            {
              path: '/home',
              element: <UserBooksPage />,
            },
            {
              path: 'my-borrowings',
              element: <UserBorrowingsPage />,
            },
          ],
        },
      ],
    },
    {
      path: '/sign-in',
      element: <SignInPage />,
    },
    {
      path: '/register',
      element: <RegisterPage />,
    },
  ];

  routes = routeItems;
  const router = createBrowserRouter([...routes]);

  return <RouterProvider router={router} />;
}
