import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { Button, Form } from 'react-bootstrap';
import api from '../../api';
import { Company } from '../../models/company';

const EditCompanyPage: React.FC = () => {
  const { companyId } = useParams<{ companyId: string }>();
  const [company, setCompany] = useState<Company | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCompany = async () => {
      try {
        const data = await api.std.company.get(companyId);
        setCompany(data);
      } catch (err) {
        setError('Error fetching company data');
      } finally {
        setLoading(false);
      }
    };

    fetchCompany();
  }, [companyId]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setCompany((prevCompany) => prevCompany ? { ...prevCompany, [name]: value } : null);
  };

  const handleSave = async () => {
    if (company) {
      try {
        await api.std.company.update(companyId, company);
        alert('Company updated successfully');
      } catch (err) {
        setError('Error updating company');
      }
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  return (
    <div>
      <h2>Edit Company</h2>
      {company && (
        <Form>
          <Form.Group controlId="formCompanyId">
            <Form.Label>ID</Form.Label>
            <Form.Control type="text" value={company.id} readOnly />
          </Form.Group>
          <Form.Group controlId="formTenantId">
            <Form.Label>Tenant ID</Form.Label>
            <Form.Control type="text" name="tenantId" value={company.tenantId} onChange={handleInputChange} />
          </Form.Group>
          <Form.Group controlId="formName">
            <Form.Label>Name</Form.Label>
            <Form.Control type="text" name="name" value={company.name} onChange={handleInputChange} />
          </Form.Group>
          <Button variant="primary" onClick={handleSave}>Save</Button>
        </Form>
      )}
    </div>
  );
};

export default EditCompanyPage;