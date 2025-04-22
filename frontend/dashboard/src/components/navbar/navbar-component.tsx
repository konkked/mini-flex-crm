import React, { useState, useEffect, useRef } from 'react';
import { Navbar, Nav } from 'react-bootstrap';
import { getCurrentUser, setAuthToken, getCurrentTenantFromToken } from '../../api';
import './navbar-component.css';
import { FaHome, FaBuilding, FaUsers, FaUser, FaSignOutAlt, FaStream, FaHandshake } from 'react-icons/fa'; // Add icons for pipelines

const AppNavbar = () => {
  const user = getCurrentUser();
  const tenant = getCurrentTenantFromToken();
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
            {tenant?.logo && <img src={`data:image/png;base64,${tenant.logo}`} alt="Logo" className="navbar-logo" />}
            {tenant?.name || ( tenant?.name == 'root' ? 'MiniFlexCRM' : tenant?.name)}{' '}
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
              <Nav.Link href="/home" className="nav-link">
                <FaHome className="nav-icon" />
                <span className="tooltip">Home</span>
              </Nav.Link>
              {user?.role.includes('admin') && (
                <Nav.Link href="/tenants" className="nav-link">
                  <FaBuilding className="nav-icon" />
                  <span className="tooltip">Tenants</span>
                </Nav.Link>
              )}
              {user?.role.includes('admin') && (
                <Nav.Link href="/users" className="nav-link">
                  <FaUsers className="nav-icon" />
                  <span className="tooltip">Users</span>
                </Nav.Link>
              )}
              <Nav.Link href="/companies" className="nav-link">
                <FaBuilding className="nav-icon" />
                <span className="tooltip">Companies</span>
              </Nav.Link>
              <Nav.Link href="/customers" className="nav-link">
                <FaUser className="nav-icon" />
                <span className="tooltip">Customers</span>
              </Nav.Link>
              <Nav.Link href="/lead-pipeline" className="nav-link">
                <FaStream className="nav-icon" />
                <span className="tooltip">Lead Pipeline</span>
              </Nav.Link>
              <Nav.Link href="/deal-pipeline" className="nav-link">
                <FaHandshake className="nav-icon" />
                <span className="tooltip">Deal Pipeline</span>
              </Nav.Link>
              <Nav.Link
                onClick={() => {
                  setAuthToken(null);
                  window.location.href = '/login';
                }}
                className="nav-link"
              >
                <FaSignOutAlt className="nav-icon" />
                <span className="tooltip">Logout</span>
              </Nav.Link>
            </>
          )}
        </Nav>
      </Navbar>
      {user && isSmallScreen && isOpen && (
        // Vertical dropdown for small screens
        <div className="menu-row" style={{ width: `${brandWidth}px` }}>
          <div className="menu-items">
            <Nav.Link href="/home" className="menu-item">
              <FaHome className="nav-icon" />
              <span className="tooltip">Home</span>
            </Nav.Link>
            {user?.role.includes('admin') && (
              <Nav.Link href="/tenants" className="menu-item">
                <FaBuilding className="nav-icon" />
                <span className="tooltip">Tenants</span>
              </Nav.Link>
            )}
            {user?.role.includes('admin') && (
              <Nav.Link href="/users" className="menu-item">
                <FaUsers className="nav-icon" />
                <span className="tooltip">Users</span>
              </Nav.Link>
            )}
            <Nav.Link href="/companies" className="menu-item">
              <FaBuilding className="nav-icon" />
              <span className="tooltip">Companies</span>
            </Nav.Link>
            <Nav.Link href="/customers" className="menu-item">
              <FaUser className="nav-icon" />
              <span className="tooltip">Customers</span>
            </Nav.Link>
            <Nav.Link href="/lead-pipeline" className="menu-item">
              <FaStream className="nav-icon" />
              <span className="tooltip">Lead Pipeline</span>
            </Nav.Link>
            <Nav.Link href="/deal-pipeline" className="menu-item">
              <FaHandshake className="nav-icon" />
              <span className="tooltip">Deal Pipeline</span>
            </Nav.Link>
            <Nav.Link
              onClick={() => {
                setAuthToken(null);
                window.location.href = '/login';
              }}
              className="menu-item"
            >
              <FaSignOutAlt className="nav-icon" />
              <span className="tooltip">Logout</span>
            </Nav.Link>
          </div>
        </div>
      )}
    </div>
  );
};

export default AppNavbar;