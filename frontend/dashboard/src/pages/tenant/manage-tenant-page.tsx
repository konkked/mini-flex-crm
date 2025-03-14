import React, { useEffect, useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import api from '../../api';
import { useParams } from 'react-router-dom';
import './manage-tenant-page.css'; // Import the new CSS file
import { TenantFormData } from 'models/tenant';

const ManageTenantPage: React.FC = () => {
  const [tenantData, setTenantData] = useState<TenantFormData>({
    name: '',
    shortId: '',
    theme: '',
  });
  const { tenantId } = useParams<{ tenantId: string }>();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  useEffect(() => {
    if (tenantId && !Number.isNaN(Number(tenantId))) {
      const fetchTenant = async () => {
        try {
          const data = await api.admin.tenant.get(tenantId);
          setTenantData({
            name: data.name,
            shortId: data.attributes?.shortId || data.shortId || '',
            theme: data.attributes?.theme || '',
          });
        } catch (err) {
          setError('Error fetching tenant data');
        }
      };
      fetchTenant();
    }
  }, [tenantId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setTenantData((prev) => ({ ...prev, [name]: value }));
  };

  const handleCreateTenant = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.admin.tenant.create({
        name: tenantData.name,
        shortId: tenantData.shortId,
        attributes: { 
          shortid: tenantData.shortId, 
          theme: tenantData.theme 
        }
      });
      setSuccess(true);
      setError(null);
      setTenantData({ name: '', shortId: '', theme: 'professional' });
    } catch (err) {
      setError('Error creating tenant');
      setSuccess(false);
    }
  };

  return (
    <div className="manage-tenant-container">
      <div className="manage-tenant-form">
        <h2>Create Tenant</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        {success && <Alert variant="success">Tenant created successfully!</Alert>}
        <Form onSubmit={handleCreateTenant}>
          <Form.Group controlId="formName" className="mb-3">
            <Form.Label>Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter name"
              name="name"
              value={tenantData.name}
              onChange={handleChange}
            />
          </Form.Group>
          <Form.Group controlId="formShortId" className="mb-3">
            <Form.Label>Short ID</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter short ID (e.g., abc123)"
              name="shortid"
              value={tenantData.shortId}
              onChange={handleChange}
            />
          </Form.Group>
          <Form.Group controlId="formTheme" className="mb-3">
            <Form.Label>Theme</Form.Label>
            <Form.Select name="theme" value={tenantData.theme} onChange={handleChange}>
              <option value="professional">Professional</option>
              <option value="social_light">Social Light</option>
              <option value="social_dark">Social Dark</option>
              <option value="enterprise">Enterprise</option>
            </Form.Select>
          </Form.Group>
          <Button variant="primary" type="submit">
            Create
          </Button>
        </Form>
      </div>
    </div>
  );
};

export default ManageTenantPage;