import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import api from '../../lib/api';

const CreateTenantPage: React.FC = () => {
  const [name, setName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  const handleCreateTenant = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.admin.tenant.create({ name });
      setSuccess(true);
      setError(null);
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
        <Form.Group controlId="formName">
          <Form.Label>Name</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter tenant name"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </Form.Group>
        <Button variant="primary" type="submit">
          Create
        </Button>
      </Form>
    </div>
  );
};

export default CreateTenantPage;