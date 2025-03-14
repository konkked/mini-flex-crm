import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Alert, Button } from 'react-bootstrap';
import { Link, useParams } from 'react-router-dom';
import api from '../../api';
import './view-tenant-page.css'; // Import the CSS file

const ViewTenantPage: React.FC = () => {
  const [tenantData, setTenantData] = useState({
    id: 0,
    name: '',
    shortId: '',
    theme: '',
  });
  const { tenantId } = useParams<{ tenantId: string }>();
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (tenantId && !Number.isNaN(Number(tenantId))) {
      const fetchTenant = async () => {
        try {
          const data = await api.admin.tenant.get(tenantId);
          setTenantData(data);
        } catch (err) {
          setError('Error fetching tenant data');
        }
      };
      fetchTenant();
    }
  }, [tenantId]);

  return (
    <Container fluid className="view-tenant-container">
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={6} className="view-tenant-details">
          <h2 className="text-center mb-4">Tenant Details</h2>
          {error && <Alert variant="danger">{error}</Alert>}
          <Row className="mb-3">
            <Col><strong>ID:</strong></Col>
            <Col>{tenantData.id}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Name:</strong></Col>
            <Col>{tenantData.name}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Short ID:</strong></Col>
            <Col>{tenantData.shortId}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Theme:</strong></Col>
            <Col>{tenantData.theme}</Col>
          </Row>
          <div className="text-center mt-4">
            <Link to={`/tenant/${tenantId}/manage`}>
              <Button variant="primary">Manage Tenant</Button>
            </Link>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default ViewTenantPage;