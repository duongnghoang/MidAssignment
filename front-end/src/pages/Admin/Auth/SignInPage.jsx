import { Form, Input, Button, Typography } from 'antd';
import { MailOutlined, LockOutlined } from '@ant-design/icons';
import { useEffect, useState } from 'react';
import { signIn } from '~/services/auth.service';
import { useNavigate } from 'react-router-dom';
import { useAuthContext } from '~/contexts/authContext';
import { passwordRule, requiredRule } from '~/constants/validation';
import { toast } from 'react-toastify';

const { Text, Title } = Typography;

export default function SignInPage() {
  const [loading, setLoading] = useState(false);
  const { login, user, isLoading } = useAuthContext();
  const navigate = useNavigate();

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      const token = await signIn(values.username, values.password);
      if (token) {
        login(token);
      }
    } catch (error) {
      toast.error(error.response.data.error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!isLoading && user) {
      const role = user?.role;
      if (role === 'SUPER_USER') {
        navigate('/books');
      } else if (role === 'NORMAL_USER') {
        navigate('/');
      }
    }
  }, [user, isLoading, navigate]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-lg w-full max-w-md">
        <Title level={2} className="text-center mb-6">
          Sign In
        </Title>
        <Form
          name="sign_in"
          onFinish={handleSubmit}
          layout="vertical"
          className="space-y-4"
        >
          <Form.Item
            name="username"
            label="Username"
            rules={[requiredRule('username')]}
          >
            <Input
              prefix={<MailOutlined className="text-gray-400" />}
              placeholder="Enter your username"
              className="rounded-md"
              size="large"
            />
          </Form.Item>

          <Form.Item name="password" label="Password" rules={[passwordRule]}>
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Enter your password"
              className="rounded-md"
              size="large"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="w-full bg-blue-500 hover:bg-blue-600"
              size="large"
            >
              Sign In
            </Button>
          </Form.Item>

          <div className="text-center">
            <Text>Don't have an account? </Text>
            <Button
              type="link"
              onClick={() => navigate('/register')}
              className="text-blue-600 hover:text-blue-800"
            >
              Register
            </Button>
          </div>
        </Form>
      </div>
    </div>
  );
}
