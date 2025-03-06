// src/pages/tenant/create-tenant-page.tsx
import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import api from '../../api';

const CreateTenantPage: React.FC = () => {
  const [formData, setFormData] = useState({
    name: '',
    shortid: '',
    theme: 'professional' as 'professional' | 'social_light' | 'social_dark' | 'enterprise',
  });
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleCreateTenant = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.admin.tenant.create({
        name: formData.name,
        shortid: formData.shortid,
        theme: formData.theme,
      });
      setSuccess(true);
      setError(null);
      setFormData({ name: '', shortid: '', theme: 'professional' });
    } catch (err) {
      setError('Error creating tenant');
      setSuccess(false);
    }
  };

  return (
    <div>
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
            value={formData.name}
            onChange={handleChange}
          />
        </Form.Group>
        <Form.Group controlId="formShortId" className="mb-3">
          <Form.Label>Short ID</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter short ID (e.g., abc123)"
            name="shortid"
            value={formData.shortid}
            onChange={handleChange}
          />
        </Form.Group>
        <Form.Group controlId="formTheme" className="mb-3">
          <Form.Label>Theme</Form.Label>
          <Form.Select name="theme" value={formData.theme} onChange={handleChange}>
            <option value="professional">Professional </option>
            <option value="social_light">Social Light </option>
            <option value="social_dark">Social Dark</option>
            <option value="enterprise">Enterprise</option>
          </Form.Select>
        </Form.Group>
        <Button variant="primary" type="submit">
          Create
        </Button>
      </Form>
    </div>
  );
};

export default CreateTenantPage;