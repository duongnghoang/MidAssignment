import {
  DashboardOutlined,
  BookOutlined,
  TeamOutlined,
  CalendarOutlined,
  BarChartOutlined,
  SettingOutlined,
  TagsOutlined,
  FieldTimeOutlined,
} from '@ant-design/icons';
import { userRoles } from './role';

export const navigation = [
  {
    key: '2',
    icon: <BookOutlined />,
    label: 'Books',
    link: '/books',
    requiredRole: userRoles.SUPER_USER,
  },
  {
    key: '3',
    icon: <TagsOutlined />,
    label: 'Categories',
    link: '/categories',
    requiredRole: userRoles.SUPER_USER,
  },
  {
    key: '5',
    icon: <CalendarOutlined />,
    label: 'Borrowings',
    link: '/borrowing-requests',
    requiredRole: userRoles.SUPER_USER,
  },
  {
    key: '8',
    icon: <BookOutlined />,
    label: 'Books',
    link: '/home',
    requiredRole: userRoles.NORMAL_USER,
  },
  {
    key: '9',
    icon: <FieldTimeOutlined />,
    label: 'My Borrowings',
    link: '/my-borrowings',
    requiredRole: userRoles.NORMAL_USER,
  },
];
