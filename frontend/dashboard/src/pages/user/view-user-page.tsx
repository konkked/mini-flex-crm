import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Alert, Button } from 'react-bootstrap';
import { Link, useParams } from 'react-router-dom';
import api from '../../api';
import ViewAttributesComponent from '../../components/attributes/view-attributes-component'; // Import the read-only attributes component
import './view-user-page.css'; // Import the CSS file
import { User } from 'models/user';

const ViewUserPage: React.FC = () => {
  const [userData, setUserData] = useState<User | null>(null);
  const { userId } = useParams<{ tenantId: string; userId: string }>();
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (userId && !Number.isNaN(Number(userId))) {
      const fetchUser = async () => {
        try {
          const data = await api.std.user.get(Number(userId));
          setUserData(data);
        } catch (err) {
          setError('Error fetching user data');
        }
      };
      fetchUser();
    }
  }, [userId]);

  return (
    <Container fluid className="view-user-container">
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={6} className="view-user-details">
          <h2 className="text-center mb-4">User Details</h2>
          {error && <Alert variant="danger">{error}</Alert>}
          <Row className="mb-3">
            <Col><strong>Username:</strong></Col>
            <Col>{userData?.username}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Email:</strong></Col>
            <Col>{userData?.email}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Name:</strong></Col>
            <Col>{userData?.name}</Col>
          </Row>
          <Row className="mb-3">
            <Col><strong>Company:</strong></Col>
            <Col>{userData?.tenantName}</Col>
          </Row>
          <ViewAttributesComponent target={userData ? userData : null} />
          <div className="text-center mt-4">
            <Link to={`/user/${userId}/manage`}>
              <Button variant="primary">Manage User</Button>
            </Link>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default ViewUserPage;