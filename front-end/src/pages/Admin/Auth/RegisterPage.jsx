import { Form, Input, Button, Typography, Select } from 'antd';
import { UserOutlined, MailOutlined, LockOutlined } from '@ant-design/icons';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { register } from '~/services/auth.service';
import { useAuthContext } from '~/contexts/authContext';
import { emailRule, passwordRule, requiredRule } from '~/constants/validation';
import { userRolesEnum } from '~/constants/role';

const { Title, Text } = Typography;

export default function RegisterPage() {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  async function onFinish(values) {
    setLoading(true);
    try {
      const token = await register({
        username: values.username,
        email: values.email,
        password: values.password,
        roleId: values.role,
      });
      toast.success('Registration successful!');
      navigate('/sign-in');
    } catch (error) {
      toast.error(error.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-lg w-full max-w-md">
        <Title level={2} className="text-center mb-6">
          Register
        </Title>
        <Form
          form={form}
          name="register"
          layout="vertical"
          onFinish={onFinish}
          scrollToFirstError
          className="space-y-4"
        >
          <Form.Item
            name="username"
            label="Username"
            rules={[requiredRule('username')]}
          >
            <Input
              prefix={<UserOutlined className="text-gray-400" />}
              placeholder="Username"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="email"
            label="Email Address"
            rules={[requiredRule('email'), emailRule]}
          >
            <Input
              prefix={<MailOutlined className="text-gray-400" />}
              placeholder="Email Address"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="password"
            label="Password"
            rules={[requiredRule('password'), ...passwordRule]}
            hasFeedback
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Password"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            label="Confirm Password"
            dependencies={['password']}
            rules={[
              requiredRule('confirm password'),
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('password') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(
                    new Error('The two passwords do not match!')
                  );
                },
              }),
            ]}
            hasFeedback
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Confirm Password"
              size="large"
            />
          </Form.Item>

          <Form.Item name="role" label="Role" rules={[requiredRule('role')]}>
            <Select placeholder="Select your role" size="large">
              <Select.Option value={userRolesEnum.SUPER_USER}>
                Super User
              </Select.Option>
              <Select.Option value={userRolesEnum.NORMAL_USER}>
                Normal User
              </Select.Option>
            </Select>
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="w-full bg-blue-600 hover:bg-blue-700"
              size="large"
            >
              Register
            </Button>
          </Form.Item>

          <div className="text-center">
            <Text>Already have an account? </Text>
            <Button
              type="link"
              onClick={() => navigate('/sign-in')}
              className="text-blue-600 hover:text-blue-800"
            >
              Sign In
            </Button>
          </div>
        </Form>
      </div>
    </div>
  );
}
