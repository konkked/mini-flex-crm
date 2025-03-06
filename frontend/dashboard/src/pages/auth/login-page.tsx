import React, { useState } from 'react';
import { Container, Row, Col, Form, Button, Alert } from 'react-bootstrap';
import api from '../../api';
import './login-page.css';

console.log('login-page root api:>>', api);

const LoginPage: React.FC = () => {
  console.log('LoginPage api:>>', api);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      console.log('LoginPage handleLogin api :>> ', api);
      await api.auth.login(username, password);
      setSuccess(true);
      setError(null);
      window.location.href = '/home';
    } catch (err) {
      console.error(err);
      setError('Invalid username or password');
      setSuccess(false);
    }
  };

  return (
    <Container fluid className="login-container">
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={4} className="login-form">
          <h2 className="text-center mb-4">Login</h2>
          {error && <Alert variant="danger">{error}</Alert>}
          {success && <Alert variant="success">Login successful!</Alert>}
          <Form onSubmit={handleLogin}>
            <Form.Group controlId="formUsername" className="mb-3">
              <Form.Label>Username</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </Form.Group>
            <Form.Group controlId="formPassword" className="mb-4">
              <Form.Label>Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="Enter password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </Form.Group>
            <Button variant="primary" type="submit" className="w-100">
              Login
            </Button>
          </Form>
        </Col>
      </Row>
      <Row>
          <a href="/signup">Don't have an account? Sign up</a>
      </Row>
    </Container>
  );
};

export default LoginPage;