import React from 'react';
import { Navbar, Nav } from 'react-bootstrap';
import { getCurrentUser, setAuthToken } from '../../api';
import './navbar-component.css';

const AppNavbar = () => {
  const user = getCurrentUser();

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Navbar.Brand href="/home">MiniFlexCRM</Navbar.Brand>
      <Nav className="me-auto">
        {user && (
          <>
            <Nav.Link href="/home">Home</Nav.Link>
            {user?.role.includes('admin') && <Nav.Link href="/tenants">Tenants</Nav.Link>}
            {user?.role.includes('admin') && <Nav.Link href="/users">Users</Nav.Link>}
            <Nav.Link href="/companies">Companies</Nav.Link>
            <Nav.Link href="/customers">Customers</Nav.Link>
            <Nav.Link onClick={() => { 
              setAuthToken(null); 
              window.location.href = '/login';
            }}>Logout</Nav.Link>
          </>
        )}
      </Nav>
    </Navbar>
  );
};

export default AppNavbar;
