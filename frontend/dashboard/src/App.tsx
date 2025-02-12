import React from 'react';
import './App.css';
import AppRoutes from './routes';
import NavbarComponent from './components/navbar/navbar-component';
import { useAuth } from './hooks/useAuth';


function App() {
const { isAuthenticated } = useAuth();

  return (
    <div className="App">
      {isAuthenticated && <NavbarComponent />}
      <AppRoutes />
    </div>
  );
}

export default App;