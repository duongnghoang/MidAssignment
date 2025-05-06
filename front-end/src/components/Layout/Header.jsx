import { useState } from 'react';
import { Button, Input, Badge, Avatar, Dropdown, Menu } from 'antd';
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  BookOutlined,
  UserOutlined,
  BellOutlined,
  LogoutOutlined,
  SearchOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthContext } from '~/contexts/authContext';

export default function Header({ collapsed, setCollapsed, user }) {
  const navigate = useNavigate();
  const { logout } = useAuthContext();

  const userMenuItems = [
    {
      key: 'logout',
      label: 'Logout',
      icon: <LogoutOutlined />,
      danger: true,
      onClick: logout,
    },
  ];

  return (
    <header className="p-0 bg-white flex items-center justify-between h-16 fixed top-0 left-0 right-0 z-10">
      <div className="flex items-center mx-4">
        <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={() => setCollapsed(!collapsed)}
          className="mx-3 p-5"
        />
        <div className="flex items-center">
          <BookOutlined className="text-2xl text-blue-600 mr-2" />
          <span className="text-lg font-semibold">Library System</span>
        </div>
      </div>
      <div className="flex items-center pr-4 mx-4">
        <Dropdown
          menu={{ items: userMenuItems }}
          trigger={['click']}
          placement="bottomRight"
        >
          <div className="flex items-center px-2 py-1 hover:bg-gray-100 rounded-md transition-colors">
            <Avatar icon={<UserOutlined />} className="mr-2" />
            <div className="hidden md:flex flex-col">
              <span className="text-sm font-medium">{user.username}</span>
              <span className="text-xs text-gray-500">{user.role}</span>
            </div>
          </div>
        </Dropdown>
      </div>
    </header>
  );
}
