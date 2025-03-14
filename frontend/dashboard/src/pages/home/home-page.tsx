// src/pages/home/home-page.tsx
import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import { Building, People, Person, Gear } from 'react-bootstrap-icons';
import SearchBar from '../../components/search-bar/search-bar-component';
import PaginatedList from '../../components/paginated-list/paginated-list-component';
import api, { isSuperAdmin } from '../../api';
import { useTheme } from '../../context/theme-context';
import './home-page.css';

const HomePage: React.FC = () => {
  const theme = useTheme(); // Access theme from context
  const [metrics, setMetrics] = useState({ companies: 0, customers: 0, users: 0, tenants: 0 });
  const [recentActivity, setRecentActivity] = useState<any[]>([]);

  useEffect(() => {
    const fetchMetrics = async () => {
      try {
        const [companies, customers, users, tenants] = await Promise.all([
          api.std.company.list(0, 1), // Fetch count
          api.std.customer.list(0, 1),
          api.std.user.list(0, 1),
          api.admin.tenant.list(0, 1),
        ]);
        setMetrics({
          companies: companies.length,
          customers: customers.length,
          users: users.length,
          tenants: tenants.length,
        });
      } catch (err) {
        console.error('Error fetching metrics:', err);
      }
    };

    const fetchRecentActivity = async () => {
      // Mock API call for recent activity (replace with actual API)
      const activity = [
        { id: 1, description: 'Added Company X', link: '/company/1' },
        { id: 2, description: 'Updated Customer Y', link: '/customer/2' },
      ];
      setRecentActivity(activity);
    };

    fetchMetrics();
    fetchRecentActivity();
  }, []);

  const searchApi = async (term: string) => {
    // Mock search across entities (replace with actual API)
    const results = [
      { id: 1, name: `Company: ${term}` },
      { id: 2, name: `Customer: ${term}` },
    ];
    return results;
  };

  return (
    <Container fluid className={`home-page ${theme}`}>
      {/* Hero Section */}
      <div className="hero-section text-center py-5 mb-4">
        <h1 className="display-4">Welcome to MiniFlexCRM</h1>
        <p className="lead">Manage Your Business Relationships Seamlessly</p>
        <Row className="justify-content-center">
          <Col md={6}>
            <SearchBar searchApi={searchApi} placeholder="Search Companies, Customers, or Users..." />
          </Col>
        </Row>
      </div>

      {/* Metrics Cards */}
      <Row className="mb-4">
        <Col md={3}>
          <Card className="metric-card">
            <Card.Body className="text-center">
              <Building size={40} className="mb-2" />
              <h3>{metrics.companies}</h3>
              <Card.Title>Total Companies</Card.Title>
            </Card.Body>
          </Card>
        </Col>
        <Col md={3}>
          <Card className="metric-card">
            <Card.Body className="text-center">
              <People size={40} className="mb-2" />
              <h3>{metrics.customers}</h3>
              <Card.Title>Total Customers</Card.Title>
            </Card.Body>
          </Card>
        </Col>
        <Col md={3}>
          <Card className="metric-card">
            <Card.Body className="text-center">
              <Person size={40} className="mb-2" />
              <h3>{metrics.users}</h3>
              <Card.Title>Total Users</Card.Title>
            </Card.Body>
          </Card>
        </Col>
        <Col md={3}>
          <Card className="metric-card">
            <Card.Body className="text-center">
              <Gear size={40} className="mb-2" />
              <h3>{metrics.tenants}</h3>
              <Card.Title>Active Tenants</Card.Title>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {/* Quick Actions */}
      <Row className="mb-4">
        <Col md={3}>
          <Button variant="primary" href="/company/add" className="w-100 quick-action-btn">
            <Building className="me-2" /> Add New Company
          </Button>
        </Col>
        <Col md={3}>
          <Button variant="primary" href="/customer/add" className="w-100 quick-action-btn">
            <People className="me-2" /> Add New Customer
          </Button>
        </Col>
        <Col md={3}>
          <Button variant="primary" href="/relationships" className="w-100 quick-action-btn">
        <Person className="me-2" /> View All Relationships
          </Button>
        </Col>
        {isSuperAdmin() && <Col md={3}>
          <Button variant="primary" href="/tenants" className="w-100 quick-action-btn">
            <Gear className="me-2" /> Manage Tenants
          </Button>
        </Col>}
      </Row>

      {/* Recent Activity */}
      <h3>Recent Activity</h3>
      <PaginatedList
        fetch={async () => recentActivity}
        columns={[
          { key: 'description', label: 'Action', linkTo: (id: number) => recentActivity.find(item => item.id === id)?.link || '#' },
        ]}
      />
    </Container>
  );
};

export default HomePage;