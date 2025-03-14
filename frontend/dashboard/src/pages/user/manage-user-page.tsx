import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Form, Button, Alert } from 'react-bootstrap';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import api, { getCurrentTenantId } from '../../api';
import './manage-user-page.css';
import { useParams } from 'react-router-dom';
import SearchBar from '../../components/search-bar/search-bar-component';
import { Tenant } from '../../models/tenant';
import ManageAttributesComponent from '../../components/attributes/manage-attributes-component';
import { UserFormData } from 'models/user';

const ManageUserPage: React.FC = () => {
  const [user, setUser] = useState<UserFormData>({
    name:'',
    email:'',
    role:'',
    tenantId: 0
  });
  const [selectedTenant, setSelectedTenant] = useState<Tenant | null>(null);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const { tenantId: urlTenantId, userId: userId } = useParams<{ tenantId: string; userId: string }>();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);
  const tenantId = Number(urlTenantId || getCurrentTenantId())

  useEffect(()=>{
    if(userId && !Number.isNaN(Number(userId))){
      const fetchUser = async () => {
        try {
          const data = await api.admin.user.get(tenantId, Number(userId));
          setUser(data);
          setSelectedTenant({ id: data.tenantId, name: data.tenantName, shortId:'', theme: ''});
        } catch (err) {
          setSubmitError('Error fetching user data');
        }
      };
      fetchUser();
    }
  },[urlTenantId, userId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setUser((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const validateForm = () => {
    const newErrors: { [key: string]: string } = {};
    if (!user?.username?.trim()) newErrors.username = 'Username is required';
    if (!urlTenantId && !selectedTenant) newErrors.tenantId = 'Tenant selection is required';
    if (user?.email && !/\S+@\S+\.\S+/.test(user.email)) newErrors.email = 'Invalid email format';
    if (!user?.name?.trim()) newErrors.name = 'Name is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    try {
      let newUserId;
      const payload = {
        username: user?.username,
        tenantId: Number(selectedTenant?.id || tenantId),
        email: user?.email || undefined,
        name: user?.name || undefined,
        attributes : user?.attributes, // Include attributes in the payload
      };
      if (!userId || Number.isNaN(Number(userId))) {
        const response = await api.auth.signup(payload);
        newUserId = response.id;
        toast.success(<div>User created successfully! <a href={`/user/${newUserId}`}>View User</a></div>);
      } else {
        await api.admin.user.edit(Number(userId), payload);
        toast.success(<div>User updated successfully! <a href={`/user/${userId}`}>View User</a></div>);
      }
      setSuccess(true);
      setSubmitError(null);
      setTimeout(() => {
        setUser({ username: '', email: '', name: '' });
        setSelectedTenant(null);
        setSuccess(false);
      }, 5000);
    } catch (err) {
      toast.error('Error adding user');
      setSubmitError('Error adding user');
      setSuccess(false);
    }
  };

  return (
    <Container fluid className="signup-container">
      <ToastContainer />
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={4} className="signup-form">
          <h2 className="text-center mb-4">Sign Up</h2>
          {submitError && <Alert variant="danger">{submitError}</Alert>}
          {success && <Alert variant="success">User added successfully! Tell them to login.</Alert>}
          <Form onSubmit={handleSave}>
            <Form.Group controlId="formUsername" className="mb-3">
              <Form.Label>Username <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="text"
                name="username"
                placeholder="Enter username"
                value={user?.username}
                onChange={handleChange}
                isInvalid={!!errors.username}
              />
              <Form.Control.Feedback type="invalid">{errors.username}</Form.Control.Feedback>
            </Form.Group>

            <Form.Group controlId="formName" className="mb-4">
              <Form.Label>Name <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="text"
                name="name"
                placeholder="Enter name"
                value={user?.name}
                onChange={handleChange}
                isInvalid={!!errors.name}
              />
              <Form.Control.Feedback type="invalid">{errors.name}</Form.Control.Feedback>
            </Form.Group>

            {!tenantId && (
              <Form.Group controlId="formTenantId" className="mb-3">
                <Form.Label>Tenant <span className="text-danger">*</span></Form.Label>
                <SearchBar<Tenant>
                  searchApi={api.admin.tenant.searchByName}
                  placeholder="Search..."
                  displayField="name"
                  onSelect={(tenant) => setSelectedTenant(tenant)}
                />
                {errors.tenantId && <div className="invalid-feedback d-block">{errors.tenantId}</div>}
              </Form.Group>
            )}

            <Form.Group controlId="formEmail" className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                name="email"
                placeholder="Enter email"
                value={user?.email}
                onChange={handleChange}
                isInvalid={!!errors.email}
              />
              <Form.Control.Feedback type="invalid">{errors.email}</Form.Control.Feedback>
            </Form.Group>

            {tenantId || user?.attributes && (
              <ManageAttributesComponent
                target={user}
                onAttributesChange={(c)=>setUser({...user, attributes:{...c.attributes}})}
              />
            )}

            <Button variant="primary" type="submit" className="w-100">
              Sign Up
            </Button>
          </Form>
        </Col>
      </Row>
    </Container>
  );
};

export default ManageUserPage;