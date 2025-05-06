import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import { ToastContainer, toast } from 'react-toastify';
import App from './App.jsx';
import { AuthProvider } from './contexts/authContext.jsx';

createRoot(document.getElementById('root')).render(
  <AuthProvider>
    <App />
    <ToastContainer />
  </AuthProvider>
);
