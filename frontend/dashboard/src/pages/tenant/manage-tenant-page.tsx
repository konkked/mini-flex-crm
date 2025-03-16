import React, { useEffect, useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import api from '../../api';
import { useParams } from 'react-router-dom';
import { TenantFormData } from 'models/tenant';
import { toast } from 'react-toastify';


import './manage-tenant-page.css';

const ManageTenantPage: React.FC = () => {
  const [tenantData, setTenantData] = useState<TenantFormData>({
    name: '',
    shortId: '',
    theme: 'professional',
    logoBase64: '',
    bannerBase64: ''
  });
  const { tenantId } = useParams<{ tenantId: string }>();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);
  const header = tenantId && "Manage Tenant" || "Create Tenant";

  useEffect(() => {
    if (tenantId && !Number.isNaN(Number(tenantId))) {
      const fetchTenant = async () => {
        try {
          const data = await api.admin.tenant.get(tenantId);
          setTenantData({
            name: data.name,
            shortId: data.shortId,
            theme: data.theme,
            logoBase64: data.logo ? `data:image/png;base64,${data.logo}` : '',
            bannerBase64: data.banner ? `data:image/png;base64,${data.banner}` : ''
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

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, files } = e.target;
    if (files && files[0]) {
      const file = files[0];
      const reader = new FileReader();
      reader.onloadend = () => {
        const img = new Image();
        img.src = reader.result as string;
        img.onload = () => {
          const canvas = document.createElement('canvas');
          canvas.width = img.width;
          canvas.height = img.height;
          const ctx = canvas.getContext('2d');
          ctx?.drawImage(img, 0, 0);
          const dataUrl = canvas.toDataURL('image/png');
          setTenantData((prev) => ({ ...prev, [name]: dataUrl.split(',')[1] }));
        };
      };
      reader.readAsDataURL(file);
    }
  };

  const submit = async (e: React.FormEvent) => {
    let resultTenantId = tenantId;
    e.preventDefault();
    try {
      if (!tenantId || Number.isNaN(Number(tenantId))) {
        resultTenantId = await api.admin.tenant.create({
          name: tenantData?.name,
          shortId: tenantData?.shortId,
          theme: tenantData?.theme,
          logoBase64: tenantData.logoBase64,
          bannerBase64: tenantData.bannerBase64
        });
        toast.success(<div>Tenant created successfully! <a href={`/tenant/${resultTenantId}`}>View Tenant</a></div>);
      } else {
        await api.admin.tenant.edit(tenantId, {
          name: tenantData?.name,
          shortId: tenantData?.shortId,
          theme: tenantData?.theme,
          logoBase64: tenantData.logoBase64,
          bannerBase64: tenantData.bannerBase64
        })
        toast.success(<div>Tenant successfully edited! <a href={`/tenant/${tenantId}`}>View Tenant</a></div>);
      }
      setSuccess(true);
      setTimeout(() => {
        if(!tenantId || Number.isNaN(Number(tenantId))) {
          setTenantData({ name: '', shortId:'', theme:'professional', logoBase64: '', bannerBase64: '' });
        }
        setSuccess(false);
        setError(null);
      }, 5000);
    } catch (err) {
      setError('Error creating tenant');
      setSuccess(false);
    }
  };

  return (
    <div className="manage-tenant-container">
      <div className="manage-tenant-form">
        <h2>{tenantId ? 'Edit Tenant' : 'Create Tenant'}</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        {success && <Alert variant="success">Tenant {tenantId ? 'updated' : 'created'} successfully!</Alert>}
        <Form onSubmit={submit}>
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
              name="shortId"
              value={tenantData.shortId}
              onChange={handleChange}
            />
          </Form.Group>
          <Form.Group controlId="formTheme" className="mb-3">
            <Form.Label>Theme</Form.Label>
            <Form.Select name="theme" value={tenantData.theme} onChange={handleChange}>
              <option value="professional">Professional</option>
              <option value="social">Social</option>
              <option value="enterprise">Enterprise</option>
            </Form.Select>
          </Form.Group>
          <Form.Group controlId="formLogo" className="mb-3">
            <Form.Label>Logo</Form.Label>
            <Form.Control
              type="file"
              name="logoBase64"
              accept="image/*"
              onChange={handleFileChange}
            />
          </Form.Group>
          <Form.Group controlId="formBanner" className="mb-3">
            <Form.Label>Banner</Form.Label>
            <Form.Control
              type="file"
              name="bannerBase64"
              accept="image/*"
              onChange={handleFileChange}
            />
          </Form.Group>
          <Button variant="primary" type="submit">
            {tenantId ? 'Update' : 'Create'}
          </Button>
        </Form>
      </div>
    </div>
  );
};

export default ManageTenantPage;