import React from 'react';
import './App.css';
import AppRoutes from './routes';
import NavbarComponent from './components/navbar/navbar-component';
import { useAuth } from './hooks/useAuth';
import { ThemeProvider } from './context/theme-context';
import './themes/enterprise.css';
import './themes/professional.css';
import './themes/social-dark.css';
import './themes/social-light.css';



function App() {
const { isAuthenticated } = useAuth();

  return (
    <ThemeProvider>
    <div className="App">
      {isAuthenticated && <NavbarComponent />}
      <AppRoutes />
    </div>
    </ThemeProvider>
  );
}

export default App;