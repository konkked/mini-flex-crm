import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { Button, Form } from 'react-bootstrap';
import api from '../../api';
import { Company } from '../../models/company';
import SearchBar from 'components/search-bar/search-bar-component';
import { Tenant } from 'models/tenant';
import ManageAttributesComponent from 'components/attributes/manage-attributes-component';
import { Record } from 'react-bootstrap-icons';

const ManageCompanyPage: React.FC = () => {
  const { companyId } = useParams<{ companyId: string }>();
  const { tenantId: urlTenantId } = useParams<{ tenantId: string }>();
  const [selectedTenant, setSelectedTenant] = useState<Tenant | null>(null);
  const [company, setCompany] = useState<Company | null>({
    id: 0,
    name: '',
    tenantId: 0,
    attributes: new Map<string, any>(),
  });
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [attributesValue, setAttributesValue] = useState<{ [key: string]: any }>({});
  const [attributes, setAttributes] = useState<{ [key: string]: string }>({});

  const updateAttributes = (settingAttributes: { [key: string]: string }) => {
    setAttributes(attributes);
    let newAttributes : Record<string,string> = {};
    Object.keys(settingAttributes).forEach((key) => {
      if(typeof attributes[key] == "string"){
        newAttributes[key] = settingAttributes[key];
      } else {
        try {
          newAttributes[key] = JSON.parse(settingAttributes[key]);
        } catch (e) {
          newAttributes[key] = settingAttributes[key];
        }
      }
    });
    setAttributesValue(newAttributes);
  }

  useEffect(() => {
    const fetchCompany = async () => {
      try {
        const data = await api.std.company.get(companyId);
        if(data.attributes){
          let keys = Object.keys(data.attribute);
          let attributes : Record<string, string> = {};
          for(let index = 0;index < keys.length; index++){
            if(typeof data.attribute[keys[index]] == "string"){
              attributes[keys[index]] = data.attribute[keys[index]];
            } else {
              attributes[keys[index]] = JSON.stringify(data.attribute[keys[index]]);
            }
          }
          setAttributesValue(data.attributes);
        }
        setAttributes(data.attributes);
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
        if(selectedTenant) {
          company.tenantId = selectedTenant.id;
        }
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
      <h2>Manage Company</h2>
        {company && <Form>
        {companyId && <Form.Group controlId="formCompanyId">
            <Form.Label>ID</Form.Label>
            <Form.Control type="text" value={companyId} readOnly />
          </Form.Group>}
        {!urlTenantId &&<Form.Group controlId="formTenantId" className="mb-3">
            <Form.Label>Company <span className="text-danger">*</span></Form.Label>
            <SearchBar<Tenant>
              searchApi={api.admin.tenant.searchByName}
              placeholder="Search..."
              displayField="name"
              onSelect={(tenant) => setSelectedTenant(tenant)}
            />
          </Form.Group>}
          <Form.Group controlId="formName">
            <Form.Label>Name</Form.Label>
            <Form.Control type="text" name="name" value={company.name} onChange={handleInputChange} />
          </Form.Group>
          <ManageAttributesComponent initialAttributes={attributesValue} onAttributesChange={updateAttributes} />
          <Button variant="primary" onClick={handleSave}>Save</Button>
        </Form>}
    </div>
  );
};

export default ManageCompanyPage;