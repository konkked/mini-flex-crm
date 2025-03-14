import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { Button, Form } from 'react-bootstrap';
import api, { getCurrentTenantId } from '../../api';
import { CompanyFormData } from '../../models/company';
import SearchBar from 'components/search-bar/search-bar-component';
import { Tenant } from 'models/tenant';
import ManageAttributesComponent from 'components/attributes/manage-attributes-component';
import { Record } from 'react-bootstrap-icons';
import { toast } from 'react-toastify';

const ManageCompanyPage: React.FC = () => {
  const { companyId } = useParams<{ companyId: string }>();
  const { tenantId: urlTenantId } = useParams<{ tenantId: string }>();
  const [tenant, setTenant] = useState<Tenant | null>(null);
  const [company, setCompany] = useState<CompanyFormData | null>({
    name: '',
    tenantId: 0,
    attributes: new Map<string, any>(),
  });
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const tenantId = Number(urlTenantId || getCurrentTenantId())
  

  useEffect(() => {
    const fetchCompany = async () => {
      try {
        const data = await api.admin.company.get(tenantId, companyId);
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
          company.tenantId =  tenant?.id || tenantId;
          let companyId = 0;
          if (companyId) {
            await api.std.company.edit(companyId, company);
            companyId = Number(companyId);
          } else {
            var id = await api.std.company.create(company);
            companyId = id;
          }
          toast.success(
            <div>
              Customer updated successfully! <a href={`/company/${companyId}`}>View Customer</a>
            </div>,
            {
              onClose: () => {
                setCompany(null);
                setTimeout(() => {
                  setCompany({
                    name: '',
                    tenantId: tenantId,
                    attributes: {}
                  });
                  setTenant(null);
                  setError(null);
                }, 3000);
              }
            }
          );
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
      <h2>Manage Company</h2>
        {company && <Form>
        {companyId && <Form.Group controlId="formCompanyId">
            <Form.Label>ID</Form.Label>
            <Form.Control type="text" value={companyId} readOnly />
          </Form.Group>}
        {!tenantId &&<Form.Group controlId="formTenantId" className="mb-3">
            <Form.Label>Tenant <span className="text-danger">*</span></Form.Label>
            <SearchBar<Tenant>
              searchApi={api.admin.tenant.searchByName}
              placeholder="Search..."
              displayField="name"
              onSelect={(tenant) => setTenant(tenant)}
            />
          </Form.Group>}
          <Form.Group controlId="formName">
            <Form.Label>Name</Form.Label>
            <Form.Control type="text" name="name" value={company.name} onChange={handleInputChange} />
          </Form.Group>
          <ManageAttributesComponent 
            target={company} 
            onAttributesChange={(c)=>setCompany({...company, attributes:{...c.attributes}})} />
          <Button variant="primary" onClick={handleSave}>Save</Button>
        </Form>}
    </div>
  );
};

export default ManageCompanyPage;