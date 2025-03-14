// src/pages/company/view-company-page.tsx
import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Alert, Button } from 'react-bootstrap';
import { Link, useParams } from 'react-router-dom';
import api from '../../api';
import ViewAttributesComponent from '../../components/attributes/view-attributes-component'; // Import the read-only attributes component
import './view-company-page.css'; // Import the CSS file
import { Company } from '../../models/company'; // Assuming a Company model exists

const ViewCompanyPage: React.FC = () => {
  const [companyData, setCompanyData] = useState<Company>({
    id: 0,
    name: '',
    tenantId: 0,
    tenantName: '',
    attributes: new Map<string, any>(),
  });
  const { companyId } = useParams<{ companyId: string }>();
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (companyId && !Number.isNaN(Number(companyId))) {
      const fetchCompany = async () => {
        try {
          const data = await api.std.company.get(companyId); // Assuming an API method to get a company
          setCompanyData(data);
        } catch (err) {
          setError('Error fetching company data');
        }
      };
      fetchCompany();
    }
  }, [companyId]);

  return (
    <Container fluid className="view-company-container">
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={6} className="view-company-details">
          <h2 className="text-center mb-4">Company Details</h2>
          {error && <Alert variant="danger">{error}</Alert>}
          <Row className="mb-3">
            <Col><strong>ID:</strong></Col>
            <Col>{companyData.id}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Name:</strong></Col>
            <Col>{companyData.name}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Tenant ID:</strong></Col>
            <Col>{companyData.tenantId}</Col>
          </Row>
          <ViewAttributesComponent target={companyData} />
          <div className="text-center mt-4">
            <Link to={`/company/${companyId}/manage`}>
              <Button variant="primary">Manage Company</Button>
            </Link>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default ViewCompanyPage;