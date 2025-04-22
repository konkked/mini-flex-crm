import React, { useEffect, useState, useRef } from 'react';
import { Container, Row, Col, Card, Button, Alert, Spinner } from 'react-bootstrap';
import { useParams, useNavigate } from 'react-router-dom';
import api, { getCurrentTenantId } from '../../api';
import { User } from '../../models/user';
import './user-profile-page.css';

const UserProfilePage: React.FC = () => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [uploading, setUploading] = useState<boolean>(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const tenantId = getCurrentTenantId();

  useEffect(() => {
    if (userId && !Number.isNaN(Number(userId))) {
      fetchUserData();
    } else {
      setError('Invalid user ID');
      setLoading(false);
    }
  }, [userId]);

  const fetchUserData = async () => {
    try {
      setLoading(true);
      const userData = await api.std.user.get(Number(userId));
      setUser(userData);
      setError(null);
    } catch (err) {
      setError('Failed to load user data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleImageClick = () => {
    if (fileInputRef.current) {
      fileInputRef.current.click();
    }
  };

  const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    try {
      setUploading(true);
      
      // Convert image to base64
      const reader = new FileReader();
      reader.onload = async (e) => {
        if (e.target?.result) {
          const base64String = e.target.result as string;
          // Remove the data URL prefix (e.g., "data:image/jpeg;base64,")
          const base64Data = base64String.split(',')[1];
          
          // Upload the image
          await api.std.user.uploadProfileImage(Number(userId), base64Data);
          
          // Refresh user data to get the updated profile image
          await fetchUserData();
        }
      };
      reader.readAsDataURL(file);
    } catch (err) {
      setError('Failed to upload profile image');
      console.error(err);
    } finally {
      setUploading(false);
    }
  };

  const getInitials = (name: string) => {
    const parts = name.split(' ');
    if (parts.length >= 2) {
      return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase();
    }
    return name.substring(0, 2).toUpperCase();
  };

  if (loading) {
    return (
      <Container className="d-flex justify-content-center align-items-center" style={{ height: '80vh' }}>
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </Container>
    );
  }

  if (error || !user) {
    return (
      <Container className="mt-4">
        <Alert variant="danger">{error || 'User not found'}</Alert>
        <Button variant="primary" onClick={() => navigate('/users')}>
          Back to Users
        </Button>
      </Container>
    );
  }

  return (
    <Container className="user-profile-container">
      <Row className="justify-content-center">
        <Col md={8} lg={6}>
          <Card className="profile-card">
            <Card.Body className="text-center">
              <div className="profile-image-container" onClick={handleImageClick}>
                {user.profileImage ? (
                  <img 
                    src={`data:image/png;base64,${user.profileImage}`} 
                    alt={`${user.name}'s profile`} 
                    className="profile-image"
                  />
                ) : (
                  <div className="profile-image-placeholder">
                    {getInitials(user.name)}
                  </div>
                )}
                <div className="profile-image-overlay">
                  <span>Change Photo</span>
                </div>
                <input
                  type="file"
                  ref={fileInputRef}
                  onChange={handleFileChange}
                  accept="image/*"
                  style={{ display: 'none' }}
                />
                {uploading && (
                  <div className="upload-spinner">
                    <Spinner animation="border" size="sm" />
                  </div>
                )}
              </div>
              
              <h2 className="mt-3">{user.name}</h2>
              <p className="text-muted">{user.email}</p>
              
              <div className="user-details mt-4">
                <Row>
                  <Col sm={6}>
                    <div className="detail-item">
                      <strong>Username:</strong>
                      <p>{user.username}</p>
                    </div>
                  </Col>
                  <Col sm={6}>
                    <div className="detail-item">
                      <strong>Role:</strong>
                      <p>{user.role}</p>
                    </div>
                  </Col>
                </Row>
                <Row className="mt-3">
                  <Col sm={6}>
                    <div className="detail-item">
                      <strong>Status:</strong>
                      <p>{user.enabled ? 'Active' : 'Inactive'}</p>
                    </div>
                  </Col>
                  <Col sm={6}>
                    <div className="detail-item">
                      <strong>Tenant:</strong>
                      <p>{user.tenantName}</p>
                    </div>
                  </Col>
                </Row>
              </div>
              
              <div className="mt-4">
                <Button 
                  variant="outline-primary" 
                  className="me-2"
                  onClick={() => navigate(`/user/${userId}/edit`)}
                >
                  Edit Profile
                </Button>
                <Button 
                  variant="outline-secondary"
                  onClick={() => navigate('/users')}
                >
                  Back to Users
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default UserProfilePage; 