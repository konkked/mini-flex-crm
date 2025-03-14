import React, { useState } from 'react';
import { Navbar, Nav } from 'react-bootstrap';
import { getCurrentUser, setAuthToken } from '../../api';
import './navbar-component.css';

const AppNavbar = () => {
  const user = getCurrentUser();
  const [isOpen, setIsOpen] = useState(false);

  const handleToggle = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div className="navbar-wrapper">
      <Navbar bg="dark" variant="dark" expand="lg" className="custom-navbar">
        <Navbar.Brand href="#home" className="navbar-brand">
          <span onClick={handleToggle} style={{ cursor: 'pointer' }}>
            MiniFlexCRM{' '}
            <span className={`caret ${isOpen ? 'open' : ''}`}>❯</span>
          </span>
        </Navbar.Brand>
        <Nav className="me-auto" />
      </Navbar>
      {user && isOpen && (
        <div className="menu-row">
          <div className="menu-items">
            <Nav.Link href="/home" className="menu-item">Home</Nav.Link>
            {user?.role.includes('admin') && (
              <Nav.Link href="/tenants" className="menu-item">Tenants</Nav.Link>
            )}
            {user?.role.includes('admin') && (
              <Nav.Link href="/users" className="menu-item">Users</Nav.Link>
            )}
            <Nav.Link href="/companies" className="menu-item">Companies</Nav.Link>
            <Nav.Link href="/customers" className="menu-item">Customers</Nav.Link>
            <Nav.Link
              onClick={() => {
                setAuthToken(null);
                window.location.href = '/login';
              }}
              className="menu-item"
            >
              Logout
            </Nav.Link>
          </div>
        </div>
      )}
    </div>
  );
};

export default AppNavbar;