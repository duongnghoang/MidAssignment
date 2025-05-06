import { useEffect, useState } from 'react';
import { Layout } from 'antd';
import { Content } from 'antd/es/layout/layout';
import Sider from 'antd/es/layout/Sider';
import Navbar from '~/components/Layout/Navbar';
import Header from '~/components/Layout/Header';
import { Outlet } from 'react-router-dom';
import { useAuthContext } from '~/contexts/authContext';

export default function UserLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const { user } = useAuthContext();

  useEffect(() => {
    console.log('User:', user);
  }, []);

  return (
    <Layout className="min-h-screen">
      <Header collapsed={collapsed} setCollapsed={setCollapsed} user={user} />

      {/* Main Layout */}
      <Layout className="mt-16">
        <Sider
          trigger={null}
          collapsible
          collapsed={collapsed}
          className="bg-white fixed top-16 bottom-0 overflow-auto"
          style={{
            height: 'calc(100vh - 64px)',
          }}
          width={collapsed ? 80 : 200}
        >
          <Navbar collapsed={collapsed} />
        </Sider>
        {/* Content */}
        <Content
          className="mr-6 my-6 p-6 bg-white rounded-lg shadow-md flex-1 overflow-y-auto"
          style={{
            marginLeft: collapsed ? 104 : 224,
          }}
        >
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
