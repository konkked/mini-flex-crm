// src/pages/user/add-user-page.tsx
import React, { useState } from 'react';
import { Container, Row, Col, Form, Button, Alert } from 'react-bootstrap';
import api from '../../api';
import './add-user-page.css';
import { useParams } from 'react-router-dom';
import SearchBar from '../../components/search-bar/search-bar-component';
import { Tenant } from 'models/tenant';

const AddUserPage: React.FC = () => {
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    email: '',
    name: '',
  });
  const [selectedTenant, setSelectedTenant] = useState<{ id: number; name: string } | null>(null);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const { tenantId: urlTenantId } = useParams<{ tenantId: string }>();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const validateForm = () => {
    const newErrors: { [key: string]: string } = {};
    if (!formData.username.trim()) newErrors.username = 'Username is required';
    if (!formData.password.trim()) newErrors.password = 'Password is required';
    else if (formData.password.length < 6) newErrors.password = 'Password must be at least 6 characters';
    if (!urlTenantId && !selectedTenant) newErrors.tenantId = 'Company selection is required';
    if (formData.email && !/\S+@\S+\.\S+/.test(formData.email)) newErrors.email = 'Invalid email format';
    if (!formData.name.trim()) newErrors.name = 'Name is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSignup = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      await api.auth.signup({
        username: formData.username,
        password: formData.password,
        tenantId: Number(urlTenantId || selectedTenant?.id),
        email: formData.email || undefined,
        name: formData.name || undefined,
      });
      setSuccess(true);
      setSubmitError(null);
      setFormData({ username: '', password: '', email: '', name: '' });
    } catch (err) {
      setSubmitError('Error adding user');
      setSuccess(false);
    }
  };

  return (
    <Container fluid className="signup-container">
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={4} className="signup-form">
          <h2 className="text-center mb-4">Sign Up</h2>
          {submitError && <Alert variant="danger">{submitError}</Alert>}
          {success && <Alert variant="success">User added successfully! Tell them to login.</Alert>}
          <Form onSubmit={handleSignup}>
            <Form.Group controlId="formUsername" className="mb-3">
              <Form.Label>Username <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="text"
                name="username"
                placeholder="Enter username"
                value={formData.username}
                onChange={handleChange}
                isInvalid={!!errors.username}
              />
              <Form.Control.Feedback type="invalid">{errors.username}</Form.Control.Feedback>
            </Form.Group>

            <Form.Group controlId="formPassword" className="mb-3">
              <Form.Label>Password <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="password"
                name="password"
                placeholder="Enter password"
                value={formData.password}
                onChange={handleChange}
                isInvalid={!!errors.password}
              />
              <Form.Control.Feedback type="invalid">{errors.password}</Form.Control.Feedback>
            </Form.Group>

            <Form.Group controlId="formName" className="mb-4">
              <Form.Label>Name <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="text"
                name="name"
                placeholder="Enter name"
                value={formData.name}
                onChange={handleChange}
                isInvalid={!!errors.name}
              />
              <Form.Control.Feedback type="invalid">{errors.name}</Form.Control.Feedback>
            </Form.Group>

            {!urlTenantId &&<Form.Group controlId="formTenantId" className="mb-3">
                <Form.Label>Company <span className="text-danger">*</span></Form.Label>
                <SearchBar<Tenant>
                  searchApi={api.admin.tenant.searchByName}
                  placeholder="Search..."
                  displayField="name"
                  onSelect={(tenant) => setSelectedTenant(tenant)}
                />
                {errors.tenantId && <div className="invalid-feedback d-block">{errors.tenantId}</div>}
              </Form.Group>}

            <Form.Group controlId="formEmail" className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                name="email"
                placeholder="Enter email (optional)"
                value={formData.email}
                onChange={handleChange}
                isInvalid={!!errors.email}
              />
              <Form.Control.Feedback type="invalid">{errors.email}</Form.Control.Feedback>
            </Form.Group>

            <Button variant="primary" type="submit" className="w-100">
              Sign Up
            </Button>
          </Form>
        </Col>
      </Row>
    </Container>
  );
};

export default AddUserPage;