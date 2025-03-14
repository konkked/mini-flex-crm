import React, { useState, useEffect, useRef } from 'react';
import { Navbar, Nav } from 'react-bootstrap';
import { getCurrentUser, setAuthToken } from '../../api';
import './navbar-component.css';

const AppNavbar = () => {
  const user = getCurrentUser();
  const [isOpen, setIsOpen] = useState(false);
  const [isSmallScreen, setIsSmallScreen] = useState(window.innerWidth < 768); // Breakpoint at 768px
  const [brandWidth, setBrandWidth] = useState(0);
  const brandRef = useRef<HTMLAnchorElement>(null);

  const handleToggle = () => {
    setIsOpen(!isOpen);
  };

  // Measure the width of the Navbar.Brand for small screens
  useEffect(() => {
    const updateBrandWidth = () => {
      if (brandRef.current) {
        setBrandWidth(brandRef.current.offsetWidth);
      }
    };

    updateBrandWidth();
    window.addEventListener('resize', updateBrandWidth);
    return () => window.removeEventListener('resize', updateBrandWidth);
  }, []);

  // Detect screen size changes
  useEffect(() => {
    const handleResize = () => {
      setIsSmallScreen(window.innerWidth < 990);
      if (window.innerWidth >= 990) {
        setIsOpen(false); // Close dropdown on large screens
      }
    };

    handleResize(); // Initial check
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return (
    <div className="navbar-wrapper">
      <Navbar bg="dark" variant="dark" expand="lg" className="custom-navbar">
        <Navbar.Brand href="#home" className="navbar-brand" ref={brandRef}>
          <span style={{ cursor: 'pointer' }}>
            MiniFlexCRM{' '}
            {isSmallScreen && (
              <span onClick={handleToggle} className={`caret ${isOpen ? 'open' : ''}`}>
                ❯
              </span>
            )}
          </span>
        </Navbar.Brand>
        <Nav className="me-auto">
          {user && !isSmallScreen && (
            // Horizontal layout for large screens
            <>
              <Nav.Link href="/home">Home</Nav.Link>
              {user?.role.includes('admin') && <Nav.Link href="/tenants">Tenants</Nav.Link>}
              {user?.role.includes('admin') && <Nav.Link href="/users">Users</Nav.Link>}
              <Nav.Link href="/companies">Companies</Nav.Link>
              <Nav.Link href="/customers">Customers</Nav.Link>
              <Nav.Link
                onClick={() => {
                  setAuthToken(null);
                  window.location.href = '/login';
                }}
              >
                Logout
              </Nav.Link>
            </>
          )}
        </Nav>
      </Navbar>
      {user && isSmallScreen && isOpen && (
        // Vertical dropdown for small screens
        <div className="menu-row" style={{ width: `${brandWidth}px` }}>
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