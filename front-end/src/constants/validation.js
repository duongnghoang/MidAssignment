export const requiredRule = (fieldName) => ({
  required: true,
  message: `Please input your ${fieldName}!`,
});

export const emailRule = {
  type: 'email',
  message: 'Please enter a valid email address!',
};

export const passwordRule = [
  {
    min: 8,
    message: 'Password must be at least 8 characters!',
  },
  {
    pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
    message:
      'Password must include uppercase, lowercase, number and special character!',
  },
];
