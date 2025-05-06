import { Menu } from 'antd';
import {
  DashboardOutlined,
  BookOutlined,
  TeamOutlined,
  CalendarOutlined,
  BarChartOutlined,
  SettingOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthContext } from '~/contexts/authContext';
import { navigation } from '~/constants/navigation';

export default function Navbar({ collapsed }) {
  const navigate = useNavigate();
  const { user } = useAuthContext();

  const handleNavigate = (link) => {
    navigate(link);
  };

  return (
    <nav
      className="bg-white fixed top-16 bottom-0 overflow-auto"
      style={{
        height: 'calc(100vh - 64px)',
        width: collapsed ? 80 : 200,
      }}
    >
      <Menu
        mode="inline"
        defaultSelectedKeys={['1']}
        className="border-r-0"
        items={navigation
          .filter((item) => item.requiredRole === user.role)
          .map((item) => ({
            key: item.key,
            icon: item.icon,
            label: item.label,
            onClick: () => handleNavigate(item.link),
          }))}
      />
    </nav>
  );
}
